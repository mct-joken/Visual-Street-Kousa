using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kousachan : MonoBehaviour
{
    [SerializeField]float kousamitudo = 200;
    [SerializeField] GameObject kousaslider;
    slider s;
    [SerializeField]float time = 0;
    [SerializeField]float kira;
    // Start is called before the first frame update
    void Start()
    {
        // フォグを設定する方法
        // フォグを有効化(false で無効化)
        RenderSettings.fog = true;
        s = kousaslider.GetComponent<slider>();
        // フォグの色の変更
        RenderSettings.fogColor = Color.HSVToRGB(0.17f,0.27f,0.71f);

        // フォグの強度の変更
        RenderSettings.fogDensity = -1*kousamitudo*Mathf.Log(0.05f)*0.001f/900;

    }

    // Update is called once per frame
    void Update()
    {
        if (30 >= time)
        {
            kira = 2/150f * time + 0.6f;
        }
        else if (150 >= time)
        {
            kira = 1;
        }
        else if (210 >= time)
        {
            kira = 3-2/150f * time;
        }
        else if (330 >= time)
        {
            kira = 0.2f;
        }
        else
        {
            kira = 2/150f* time - 4.2f;
        }
        kousamitudo = s.slide;
        RenderSettings.fogColor = Color.HSVToRGB(0.17f,0.27f,0.71f*kira);
        RenderSettings.fogDensity = -1 * kousamitudo * Mathf.Log(0.05f) * 0.001f / 900;
        
    }
    public void gettime(float rot) {
        rot -= 180;
        if (rot < 0)
        {
            rot = rot + 360;
        }
        time = rot;
    }
}
