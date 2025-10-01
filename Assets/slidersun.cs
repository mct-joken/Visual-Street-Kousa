using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slidersun : MonoBehaviour
{
    public void Sun(float rot) 
    {
        transform.eulerAngles=new Vector3(-rot,90 + 34.7f * Mathf.Sin(-rot / 180f * Mathf.PI), 0);
    }
}
