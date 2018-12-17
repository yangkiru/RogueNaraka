using UnityEngine;
using System.Collections;


public class FrameChecker : MonoBehaviour
{
    float deltaTime = 0.0f;

    GUIStyle style;
    Rect rect;
    float msec;
    float fps;
    float worstFps = 100f;
    string text;

    void Awake()
    {
        int w = Screen.width, h = Screen.height;

        rect = new Rect(0, 0, w, h * 4 / 100);

        style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 4 / 200;
        style.normal.textColor = Color.gray;

        StartCoroutine("worstReset");
    }


    IEnumerator worstReset() //코루틴으로 10초 간격으로 최저 프레임 리셋해줌.
    {
        while (true)
        {
            float t = 0;
            while (t < 10)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            worstFps = 100f;
        }
    }


    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()//소스로 GUI 표시.
    {

        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

        if (fps < worstFps)  //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
            worstFps = fps;
        text = string.Format("{0}ms, {1}FPS, {2}worst", msec.ToString("F1"), fps.ToString("F1"), worstFps.ToString("F1"));
        GUI.Label(rect, text, style);
    }
}



//출처: http://prosto.tistory.com/79 [Prosto]