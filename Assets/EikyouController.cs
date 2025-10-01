using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;　//TextMeshProを使用するときに必用

public class EikyouController : MonoBehaviour
{
    [SerializeField] GameObject kousaslider;
    slider s;
    public TextMeshProUGUI textMeshPro; //型はTextMeshProUGUI

    // Start is called before the first frame update
    void Start()
    {
        s = kousaslider.GetComponent<slider>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (s.slide <= 100) {
            textMeshPro.text = "見通しに影響はありません";
        }
        else if (s.slide <= 400)
        {
            textMeshPro.text = "見通しは少し悪くなります";
        }
        else
        {
            textMeshPro.text = "見通しは非常に悪くなります";
        }
    }
}
