using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slider : MonoBehaviour
{
    Slider s;
    [SerializeField]public float slide = 0;
    // Start is called before the first frame update
    void Start()
    {
        s = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        slide=s.value;
    }
}
