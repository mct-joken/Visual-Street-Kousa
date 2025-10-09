using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;�@//TextMeshPro���g�p����Ƃ��ɕK�p

public class InfoController : MonoBehaviour
{
    public Player playerScript;
    public TextMeshProUGUI IsFlying; //�^��TextMeshProUGUI
    public TextMeshProUGUI IsUsingUI; //�^��TextMeshProUGUI

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
                IsFlying.text = "��s��(Space)\nSpace2��Ő؂�ւ�";
            }
            else
            {
                IsFlying.text = "���s��(Space)\nSpace2��Ő؂�ւ�";
            }

            if (playerScript.IsUI)
            {
                IsUsingUI.text = "���_���쒆(Esc)\nEsc�������Đ؂�ւ�";
            }
            else
            {
                IsUsingUI.text = "�t�h���쒆(Esc)\nEsc�������Đ؂�ւ�";
            }
        }
        else
        {
            IsFlying.text = "����";
            IsUsingUI.text = "����";
        }
    }
}
