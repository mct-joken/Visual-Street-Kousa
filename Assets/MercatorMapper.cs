using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MercatorMapper : MonoBehaviour
{
    // 画像サイズ
    int imageWidth = 800;
    int imageHeight = 580;

    // 地理的範囲（メルカトル図法）
    float lonMin = 110f;
    float lonMax = 150f;
    float latMin = 20f;
    float latMax = 50f;

    // 緯度経度からピクセル座標を取得
    public Vector2 LatLonToPixel(float lat, float lon)
    {
        // 経度 → X（線形変換）
        float x = (lon - lonMin) / (lonMax - lonMin) * imageWidth;

        // 緯度 → メルカトルY
        float mercatorMin = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + latMin * Mathf.Deg2Rad / 2f));
        float mercatorMax = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + latMax * Mathf.Deg2Rad / 2f));
        float mercatorY = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + lat * Mathf.Deg2Rad / 2f));

        // メルカトルY → Y座標（逆スケーリング）
        float y = (mercatorMax - mercatorY) / (mercatorMax - mercatorMin) * imageHeight;

        return new Vector2(x, y);
    }
}
