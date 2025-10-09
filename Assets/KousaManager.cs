using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KousaManager : MonoBehaviour
{
    public GameObject KousaSlider;
    public GameObject toggle;
    public GameObject button;
    public GameObject Kousagetter;
    public GsiTerrainFromAddress gsiScript;
    public void OnButtonClicked()
    {
        Slider s = KousaSlider.GetComponent<Slider>();
        Toggle t = toggle.GetComponent<Toggle>();
        Button b = button.GetComponent<Button>();
        KousaGetter k = Kousagetter.GetComponent<KousaGetter>();
        s.value = k.Getkousa((float)gsiScript.latitude, (float)gsiScript.longitude, k.realtime);
        Debug.Log("Kousa Amount: " + k.Getkousa((float)gsiScript.latitude, (float)gsiScript.longitude, k.realtime));
    }
}
