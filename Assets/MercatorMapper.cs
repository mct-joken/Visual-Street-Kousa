using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MercatorMapper : MonoBehaviour
{
    // �摜�T�C�Y
    int imageWidth = 800;
    int imageHeight = 580;

    // �n���I�͈́i�����J�g���}�@�j
    float lonMin = 110f;
    float lonMax = 150f;
    float latMin = 20f;
    float latMax = 50f;

    // �ܓx�o�x����s�N�Z�����W���擾
    public Vector2 LatLonToPixel(float lat, float lon)
    {
        // �o�x �� X�i���`�ϊ��j
        float x = (lon - lonMin) / (lonMax - lonMin) * imageWidth;

        // �ܓx �� �����J�g��Y
        float mercatorMin = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + latMin * Mathf.Deg2Rad / 2f));
        float mercatorMax = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + latMax * Mathf.Deg2Rad / 2f));
        float mercatorY = Mathf.Log(Mathf.Tan(Mathf.PI / 4f + lat * Mathf.Deg2Rad / 2f));

        // �����J�g��Y �� Y���W�i�t�X�P�[�����O�j
        float y = (mercatorMax - mercatorY) / (mercatorMax - mercatorMin) * imageHeight;

        return new Vector2(x, y);
    }
}
