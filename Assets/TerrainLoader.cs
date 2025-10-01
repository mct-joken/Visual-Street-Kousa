using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro; // TextMeshPro対応
using SimpleJSON; // JSONパーサー（必要に応じて導入）

public class GsiTerrainFromAddress : MonoBehaviour
{
    [Header("UI入力 (TextMeshPro)")]
    public TMP_InputField addressInput;   // 住所入力用
    public UnityEngine.UI.Button loadButton; // 実行ボタン

    [Header("Terrain設定")]
    private int zoom = 0;
    public float tileSize = 1000f;
    public float maxHeight = 3000f;

    private double latitude;
    private double longitude;
    // 公開用ゲッター
    public double Latitude => latitude;
    public double Longitude => longitude;

    private GameObject parentTerrain;

    // 生成したTerrainを保持するリスト
    private List<GameObject> generatedTerrains = new List<GameObject>();

    void Start()
    {
        // 非アクティブも含めて検索
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Terrain")
            {
                parentTerrain = obj;
                break;
            }
        }

        if (parentTerrain == null)
        {
            Debug.LogError("親オブジェクト 'Terrain' がシーンに存在しません。");
        }

        loadButton.onClick.AddListener(() =>
        {
            string addr = addressInput.text;
            if (!string.IsNullOrEmpty(addr))
                StartCoroutine(GetLatLonFromAddress(addr));
        });
    }

    IEnumerator GetLatLonFromAddress(string address)
    {
        string url = "https://msearch.gsi.go.jp/address-search/AddressSearch?q=" + UnityWebRequest.EscapeURL(address);

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("住所検索失敗: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        var parsed = JSON.Parse(json);

        if (parsed.Count == 0)
        {
            Debug.LogWarning("座標が見つかりません: " + address);
            yield break;
        }

        // GSIの返すJSONは [ { geometry: { coordinates: [lon, lat] }, ... } ]
        longitude = parsed[0]["geometry"]["coordinates"][0].AsDouble;
        latitude = parsed[0]["geometry"]["coordinates"][1].AsDouble;

        Debug.Log($"住所:{address} → 緯度経度 lat={latitude}, lon={longitude}");

        // 以前生成したTerrainを削除
        ClearOldTerrains();

        // ★ 最適なズームレベルを探索
        yield return StartCoroutine(TryFindBestZoom(latitude, longitude, 18, 14, (bestZoom) =>
        {
            if (bestZoom > 0)
            {
                zoom = bestZoom;
                Debug.Log($"選択されたズームレベル: {zoom}");
                // 新しいTerrainを生成
                StartCoroutine(LoadTerrainGrid(latitude, longitude));
            }
            else
            {
                Debug.LogError("利用可能な航空写真タイルが見つかりませんでした。");
            }
        }));
    }

    IEnumerator TryFindBestZoom(double lat, double lon, int maxZoom, int minZoom, Action<int> callback)
    {
        for (int z = maxZoom; z >= minZoom; z--)
        {
            (int x, int y) = LatLonToTile(lon, lat, z);

            // DEMと航空写真の両方が存在するか確認する
            string demTestUrl = $"https://cyberjapandata.gsi.go.jp/xyz/dem_png/{z}/{x}/{y}.png";
            string photoTestUrl = $"https://cyberjapandata.gsi.go.jp/xyz/seamlessphoto/{z}/{x}/{y}.jpg";

            // DEMの存在確認
            UnityWebRequest demTestReq = UnityWebRequestTexture.GetTexture(demTestUrl);
            yield return demTestReq.SendWebRequest();

            // 航空写真の存在確認
            UnityWebRequest photoTestReq = UnityWebRequestTexture.GetTexture(photoTestUrl);
            yield return photoTestReq.SendWebRequest();

            if (demTestReq.result == UnityWebRequest.Result.Success && photoTestReq.result == UnityWebRequest.Result.Success)
            {
                callback(z); // 両方成功したらコールバックを呼び出して終了
                yield break;
            }
        }
        callback(-1); // 見つからなかった場合
    }


    IEnumerator LoadTerrainGrid(double lat, double lon)
    {
        (int centerX, int centerY) = LatLonToTile(lon, lat, zoom);

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int tx = centerX + dx;
                int ty = centerY + dy;

                string demUrl = $"https://cyberjapandata.gsi.go.jp/xyz/dem_png/{zoom}/{tx}/{ty}.png";
                string photoUrl = $"https://cyberjapandata.gsi.go.jp/xyz/seamlessphoto/{zoom}/{tx}/{ty}.jpg";

                // DEM 取得
                UnityWebRequest wwwDem = UnityWebRequestTexture.GetTexture(demUrl);
                yield return wwwDem.SendWebRequest();
                if (wwwDem.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("DEM取得失敗: " + wwwDem.error);
                    continue;
                }
                Texture2D demTex = DownloadHandlerTexture.GetContent(wwwDem);

                int w = demTex.width;
                int h = demTex.height;

                int usableW = w;
                int usableH = h;
                float[,] heights = new float[usableH, usableW];

                for (int y = 0; y < usableH; y++)
                {
                    for (int x = 0; x < usableW; x++)
                    {
                        Color c = demTex.GetPixel(x, y);
                        int R = Mathf.RoundToInt(c.r * 255f);
                        int G = Mathf.RoundToInt(c.g * 255f);
                        int B = Mathf.RoundToInt(c.b * 255f);

                        int val = (R << 16) + (G << 8) + B;

                        float height;
                        if (val < 0x800000) height = val * 0.01f;
                        else if (val == 0x800000) height = 0f; // 欠損値
                        else height = -(0x1000000 - val) * 0.01f;

                        heights[y, x] = height / maxHeight;
                    }
                }

                // TerrainDataを作成
                TerrainData tData = new TerrainData();
                tData.heightmapResolution = usableW;
                tData.size = new Vector3(tileSize, maxHeight, tileSize);
                tData.SetHeights(0, 0, heights);

                // 航空写真
                UnityWebRequest wwwPhoto = UnityWebRequestTexture.GetTexture(photoUrl);
                yield return wwwPhoto.SendWebRequest();
                if (wwwPhoto.result == UnityWebRequest.Result.Success)
                {
                    Texture2D photoTex = DownloadHandlerTexture.GetContent(wwwPhoto);
                    TerrainLayer layer = new TerrainLayer();
                    layer.diffuseTexture = photoTex;
                    layer.tileSize = new Vector2(tileSize, tileSize);
                    tData.terrainLayers = new TerrainLayer[] { layer };
                }

                GameObject tObj = Terrain.CreateTerrainGameObject(tData);

                // 位置配置（左右逆バグ修正済み）
                float posX = (dx) * tileSize;
                float posZ = -(dy) * tileSize;
                tObj.transform.position = new Vector3(posX, 0, posZ);
                tObj.name = $"Terrain_{tx}_{ty}";

                // 親オブジェクトの子にする
                if (parentTerrain != null)
                {
                    tObj.transform.SetParent(parentTerrain.transform, worldPositionStays: true);
                }

                // リストに追加
                generatedTerrains.Add(tObj);
            }
        }

        // 隣接するTerrainを相互にリンク
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int tx = centerX + dx;
                int ty = centerY + dy;
                GameObject currentTerrainObj = generatedTerrains.Find(t => t.name == $"Terrain_{tx}_{ty}");
                if (currentTerrainObj == null) continue;

                Terrain currentTerrain = currentTerrainObj.GetComponent<Terrain>();

                Terrain left = null;
                Terrain right = null;
                Terrain top = null;
                Terrain bottom = null;

                if (dx > -1)
                {
                    GameObject leftObj = generatedTerrains.Find(t => t.name == $"Terrain_{tx - 1}_{ty}");
                    if (leftObj != null) left = leftObj.GetComponent<Terrain>();
                }
                if (dx < 1)
                {
                    GameObject rightObj = generatedTerrains.Find(t => t.name == $"Terrain_{tx + 1}_{ty}");
                    if (rightObj != null) right = rightObj.GetComponent<Terrain>();
                }
                if (dy > -1)
                {
                    GameObject bottomObj = generatedTerrains.Find(t => t.name == $"Terrain_{tx}_{ty + 1}");
                    if (bottomObj != null) bottom = bottomObj.GetComponent<Terrain>();
                }
                if (dy < 1)
                {
                    GameObject topObj = generatedTerrains.Find(t => t.name == $"Terrain_{tx}_{ty - 1}");
                    if (topObj != null) top = topObj.GetComponent<Terrain>();
                }

                currentTerrain.SetNeighbors(left, top, right, bottom);
            }
        }

        Debug.Log("3x3 Terrain生成完了");
    }

    void ClearOldTerrains()
    {
        foreach (var terrain in generatedTerrains)
        {
            if (terrain != null)
                Destroy(terrain);
        }
        generatedTerrains.Clear();
    }

    (int, int) LatLonToTile(double lon, double lat, int zoom)
    {
        double latRad = lat * Math.PI / 180.0;
        int n = 1 << zoom;
        int xtile = (int)Math.Floor((lon + 180.0) / 360.0 * n);
        int ytile = (int)Math.Floor((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n);
        return (xtile, ytile);
    }
}