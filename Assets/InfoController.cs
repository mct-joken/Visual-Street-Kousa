using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;　//TextMeshProを使用するときに必用

public class InfoController : MonoBehaviour
{
    public Player playerScript;
    public TextMeshProUGUI IsFlying; //型はTextMeshProUGUI
    public TextMeshProUGUI IsUsingUI; //型はTextMeshProUGUI

    private bool b;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript != null)
        {
            if (playerScript.IsFly)
            {
                IsFlying.text = "飛行中(Space)\nSpace2回で切り替え";
            }
            else
            {
                IsFlying.text = "歩行中(Space)\nSpace2回で切り替え";
            }

            if (playerScript.IsUI)
            {
                IsUsingUI.text = "視点操作中(Esc)\nEscを押して切り替え";
            }
            else
            {
                IsUsingUI.text = "ＵＩ操作中(Esc)\nEscを押して切り替え";
            }
        }
        else
        {
            IsFlying.text = "無効";
            IsUsingUI.text = "無効";
        }
    }
}
