using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;　//TextMeshProを使用するときに必用

public class shiteiController : MonoBehaviour
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
        if (s.slide >= 100)
        {
            textMeshPro.text = "視程: およそ " + (int)(9000 / (int)(s.slide)) * 100 + "m";
        }
        else
        {
            textMeshPro.text = "視程: 通常通り";
        }
    }
}
