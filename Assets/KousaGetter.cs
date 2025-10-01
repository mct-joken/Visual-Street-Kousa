using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using TMPro;

public class KousaGetter : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    [SerializeField]Object mapper;
    public Texture2D texture;
    public bool finish=false;
    [SerializeField] private string imageURL = "https://example.com/image.jpg";
    private void Awake()
    {
        imageURL = "https://www.data.jma.go.jp/env/kosa/fcst/img/surf/jp/" + 
            System.DateTime.Now.Year + (100+System.DateTime.Now.Month).ToString().Remove(0,1) 
            + (100+System.DateTime.Now.Day).ToString().Remove(0,1)+
            "0000" + "_kosafcst-s_jp_jp.png";
        StartCoroutine(LoadImageFromURL(imageURL));
    }

    private void Update()
    {
        if (finish) {
            int val = Getkousa(35, 115, false);
            outputText.text = $"黄砂予測値: {val}";
        }
    }

    Texture2D createReadabeTexture2D(Texture2D texture2d)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(
                    texture2d.width,
                    texture2d.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2d, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);
        readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTextur2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        return readableTextur2D;
    }
    Texture2D LoadImageFromResources()
    {
        // Resourcesフォルダから画像を読み込み
        Texture2D loadedTexture = Resources.Load<Texture2D>("kousa_202503270300");
        loadedTexture = createReadabeTexture2D(loadedTexture);
        if (loadedTexture != null)
        { 
            // 読み込み成功：RawImageに画像を設定
            Debug.Log("画像の読み込みに成功しました");
        }
        else
        {
            // 読み込み失敗：エラーログを出力
            Debug.LogError("画像が見つかりませんでした");
        }
        return loadedTexture;
    }

    IEnumerator LoadImageFromURL(string url)
    {
        // URL の妥当性チェック
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URLが指定されていません");
            yield break;
        }

        Debug.Log($"画像を読み込み中: {url}");

        // UnityWebRequestTexture を使用して画像を取得
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        // タイムアウト設定（10秒）
        request.timeout = 10;

        yield return request.SendWebRequest();

        // 結果の確認
        if (request.result == UnityWebRequest.Result.Success)
        {
            texture = createReadabeTexture2D(DownloadHandlerTexture.GetContent(request));

            if (texture != null)
            {
                Debug.Log("外部画像の読み込みに成功しました");
                finish = true;
            }
            else
            {
                Debug.LogError("テクスチャの作成に失敗しました");
            }
        }
        else
        {
            Debug.LogError($"画像の読み込みに失敗: {request.error}");

            // エラーの種類に応じた処理
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("ネットワーク接続エラー");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"HTTPエラー: {request.responseCode}");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("データ処理エラー");
                    break;
            }
        }

        // リソースの解放
        request.Dispose();
    }
    public int Getkousa(float lat,float lon,bool realtime) {
        MercatorMapper m =mapper.GetComponent<MercatorMapper>();
        int imageWidth = 800;
        int imageHeight = 580;
        var pix= LoadImageFromResources().GetPixelData<Color32>(0);
        if (realtime)
        {
            pix =texture.GetPixelData<Color32>(0);
        }
        else {
            pix=LoadImageFromResources().GetPixelData<Color32>(0);
        }
        Vector2 pivot = m.LatLonToPixel(lat, lon);
        int ind = 539 * imageWidth + 761;
        Color nowpos = pix[(int)(imageWidth * (int)pivot.y + (int)pivot.x)];
        for (int i = 0; i < 539-442+1; i++)
        {
            var taisyou = pix[ind - (i * imageWidth)];
            if (taisyou == nowpos) {
                return (int)(Mathf.Pow(2,4*i/(float)(539-442))*90);
            }
        }
        return 0;
    }
}