using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;　//TextMeshProを使用するときに必用

public class textController : MonoBehaviour
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
        textMeshPro.text = "黄砂濃度: "+(int)(s.slide)+"μg/m³";　//テキストの変更
    }
}
