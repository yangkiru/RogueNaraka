using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeManager : MonoBehaviour
{
    public Button pauseBtn;
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
                pauseBtn.onClick.Invoke();
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
