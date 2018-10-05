using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfect : MonoBehaviour {

    public Vector2 targetSize = new Vector2(640, 360);
    public int ppu = 32;
    public uint zoom = 1;
    private uint _zoom;
    public Camera cam;
    public bool pixelPerfect = true;
    Resolution res;
    // Use this for initialization
    private void Awake()
    {
        SetScreenSize();
        CutScreen();
        _zoom = zoom;
    }

    void Update()
    {
        if (res.width != cam.pixelWidth || res.height != cam.pixelHeight || _zoom != zoom)
        {
            SetScreenSize();
            CutScreen();
        }
    }

    public void CutScreen()
    {
        //타겟해상도(가로 1280, 세로 720, 비율).
        float tsr = targetSize.x / targetSize.y;

        //실제 기기의 해상도(가로, 세로, 비율).
        float sw = (float)Screen.width;
        float sh = (float)Screen.height;
        float sr = sw / sh;   //화면 비율.

        //실제사이즈와 타겟사이즈의 비율을 계산함(옆으로 큰지 위로 큰지 계산함)
        float size = sr - tsr;

        //실제 기기의 화면 비율이 타겟비율과 비슷함(거의 같음).
        if (Mathf.Abs(size) <= 0.01f)
        {
            //최대 해상도 제한함.
            if (sh >= targetSize.y)
            {
                sh = targetSize.y;
                sw = targetSize.x;
            }
        }
        else
        {
            //화면이 옆으로 길다.
            if (size > 0.0f)
            {
                //최대 해상도 제한함.
                if (sh >= targetSize.y)
                {
                    sh = targetSize.y;
                    sw = sh * sr;
                }
            }
            else
            {
                //최대 해상도 제한함.
                if (sw >= targetSize.x)
                {
                    sw = targetSize.x;
                    sh = sw / sr;
                }
            }
        }

        //게임 카메라의 ViewportRect 값을 조절한다. 어떤 해상도가 된다고해도 비율이 일정하게 출력됨.
        float vh = targetSize.y * sw / targetSize.x / sh;
        float vw = targetSize.x * sh / targetSize.y / sw;

        //이단계에서 카메라 ViewportRect 에 적용한다.
        this.GetComponent<Camera>().rect = new Rect(((1.0f - vw) * 0.5f), ((1.0f - vh) * 0.5f), vw, vh);

        Screen.SetResolution((int)sw, (int)sh, Screen.fullScreen);
    }

    [ContextMenu("SetScreenSize")]
    public void SetScreenSize()
    {
        if (pixelPerfect)
        {
            res = new Resolution();
            res.width = cam.pixelWidth;
            res.height = cam.pixelHeight;
            res.refreshRate = Screen.currentResolution.refreshRate;

            if (res.width == 0 || res.height == 0)
            {
                return;
            }
            Vector2Int sub = new Vector2Int(Screen.width - (int)targetSize.x, Screen.height - (int)targetSize.y);
            int x = sub.x / (int)targetSize.x;
            int y = sub.y / (int)targetSize.y;

            if(x >= 2 && y >= 2)
            {
                int min = Mathf.Min(x, y);
                cam.orthographicSize = targetSize.y / (ppu * 2) / (zoom * min);
            }
            else cam.orthographicSize = targetSize.y / (ppu * 2) / zoom;
        }
        else
        {
            cam.orthographicSize = Screen.height / (ppu * 2) / zoom;
        }
    }
}
