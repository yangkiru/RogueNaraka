using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeManager : MonoBehaviour
{
    public Button pauseBtn;
    public Button settingBtn;
#if UNITY_ANDROID
    
    bool isExit;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DeathManager.instance.deathPnl.gameObject.activeSelf)
            {
                if (isExit)
                    GameManager.instance.ExitGame();
                else
                {
                    isExit = true;
                    StartCoroutine(ExitCancel(1));
                }
            }
            else
            {
                if (pauseBtn.gameObject.activeSelf)
                    pauseBtn.onClick.Invoke();
                else if (settingBtn)
                    settingBtn.onClick.Invoke();
            }
        }
    }

    IEnumerator ExitCancel(float t)
    {
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        isExit = false;
    }
#endif
}
