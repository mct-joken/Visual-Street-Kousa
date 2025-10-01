using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapSwitcher : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Map_Masuda;
    public GameObject Map_Fukuoka;
    public GameObject Map_Shibuya;
    public GameObject Map_Osaka;
    public GameObject Map_Oki;
    public GameObject Terrain;

    private void Start()
    {
        // äJénéû
        ShowMapMasuda();
    }

    public void ShowMapMasuda()
    {
        SetOnly(Map_Masuda);
    }

    public void ShowMapFukuoka()
    {
        SetOnly(Map_Fukuoka);
    }

    public void ShowMapShibuya()
    {
        SetOnly(Map_Shibuya);
    }

    public void ShowMapOsaka()
    {
        SetOnly(Map_Osaka);
    }

    public void ShowMapOki()
    {
        SetOnly(Map_Oki);
    }

    public void ShowTerrain()
    {
        SetOnly(Terrain);
    }

    // DropdownÇÃOnValueChanged(int)Ç©ÇÁåƒÇ—èoÇ∑
    public void SwitchMapByIndex(int index)
    {
        switch (index)
        {
            case 0:
                ShowMapMasuda();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            case 1:
                ShowMapFukuoka();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            case 2:
                ShowMapShibuya();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            case 3:
                ShowMapOsaka();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            case 4:
                ShowMapOki();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            case 5:
                ShowTerrain();
                Camera.transform.position = new Vector3(0.0f, 100.0f, 0.0f);
                break;
            default:
                break;

        }
    }

    private void SetOnly(GameObject target)
    {
        Map_Masuda.SetActive(target == Map_Masuda);
        Map_Fukuoka.SetActive(target == Map_Fukuoka);
        Map_Shibuya.SetActive(target == Map_Shibuya);
        Map_Osaka.SetActive(target == Map_Osaka);
        Map_Oki.SetActive(target == Map_Oki);
        Terrain.SetActive(target == Terrain);
    }
}
