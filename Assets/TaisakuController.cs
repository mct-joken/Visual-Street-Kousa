using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;　//TextMeshProを使用するときに必用

public class TaisakuController : MonoBehaviour
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
            textMeshPro.text = "身体への影響は少ないです";
        }
        else if (s.slide <= 400)
        {
            textMeshPro.text = "呼吸器・アレルギーなどの疾患がある方はマスクやうがいなどの対策をするとよいでしょう";
        }
        else
        {
            textMeshPro.text = "マスクやうがいなどの対策をしましょう\n呼吸器・アレルギーなどの疾患がある方は屋外での激しい運動は控えましょう";
        }
    }
}
