using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using TMPro;

public class KousaGetter : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    [SerializeField]Object mapper;
    public Texture2D texture;
    public bool finish=false;
    [SerializeField] private string imageURL = "https://example.com/image.jpg";
    private void Awake()
    {
        imageURL = "https://www.data.jma.go.jp/env/kosa/fcst/img/surf/jp/" + 
            System.DateTime.Now.Year + (100+System.DateTime.Now.Month).ToString().Remove(0,1) 
            + (100+System.DateTime.Now.Day).ToString().Remove(0,1)+
            "0000" + "_kosafcst-s_jp_jp.png";
        StartCoroutine(LoadImageFromURL(imageURL));
    }

    private void Update()
    {
        if (finish) {
            int val = Getkousa(35, 115, false);
            outputText.text = $"�����\���l: {val}";
        }
    }

    Texture2D createReadabeTexture2D(Texture2D texture2d)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(
                    texture2d.width,
                    texture2d.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(texture2d, renderTexture);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTexture;
        Texture2D readableTextur2D = new Texture2D(texture2d.width, texture2d.height);
        readableTextur2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        readableTextur2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);
        return readableTextur2D;
    }
    Texture2D LoadImageFromResources()
    {
        // Resources�t�H���_����摜��ǂݍ���
        Texture2D loadedTexture = Resources.Load<Texture2D>("kousa_202503270300");
        loadedTexture = createReadabeTexture2D(loadedTexture);
        if (loadedTexture != null)
        { 
            // �ǂݍ��ݐ����FRawImage�ɉ摜��ݒ�
            Debug.Log("�摜�̓ǂݍ��݂ɐ������܂���");
        }
        else
        {
            // �ǂݍ��ݎ��s�F�G���[���O���o��
            Debug.LogError("�摜��������܂���ł���");
        }
        return loadedTexture;
    }

    IEnumerator LoadImageFromURL(string url)
    {
        // URL �̑Ó����`�F�b�N
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL���w�肳��Ă��܂���");
            yield break;
        }

        Debug.Log($"�摜��ǂݍ��ݒ�: {url}");

        // UnityWebRequestTexture ���g�p���ĉ摜���擾
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        // �^�C���A�E�g�ݒ�i10�b�j
        request.timeout = 10;

        yield return request.SendWebRequest();

        // ���ʂ̊m�F
        if (request.result == UnityWebRequest.Result.Success)
        {
            texture = createReadabeTexture2D(DownloadHandlerTexture.GetContent(request));

            if (texture != null)
            {
                Debug.Log("�O���摜�̓ǂݍ��݂ɐ������܂���");
                finish = true;
            }
            else
            {
                Debug.LogError("�e�N�X�`���̍쐬�Ɏ��s���܂���");
            }
        }
        else
        {
            Debug.LogError($"�摜�̓ǂݍ��݂Ɏ��s: {request.error}");

            // �G���[�̎�ނɉ���������
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("�l�b�g���[�N�ڑ��G���[");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError($"HTTP�G���[: {request.responseCode}");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("�f�[�^�����G���[");
                    break;
            }
        }

        // ���\�[�X�̉��
        request.Dispose();
    }
    public int Getkousa(float lat,float lon,bool realtime) {
        MercatorMapper m =mapper.GetComponent<MercatorMapper>();
        int imageWidth = 800;
        int imageHeight = 580;
        var pix= LoadImageFromResources().GetPixelData<Color32>(0);
        if (realtime)
        {
            pix =texture.GetPixelData<Color32>(0);
        }
        else {
            pix=LoadImageFromResources().GetPixelData<Color32>(0);
        }
        Vector2 pivot = m.LatLonToPixel(lat, lon);
        int ind = 539 * imageWidth + 761;
        Color nowpos = pix[(int)(imageWidth * (int)pivot.y + (int)pivot.x)];
        for (int i = 0; i < 539-442+1; i++)
        {
            var taisyou = pix[ind - (i * imageWidth)];
            if (taisyou == nowpos) {
                return (int)(Mathf.Pow(2,4*i/(float)(539-442))*90);
            }
        }
        return 0;
    }
}