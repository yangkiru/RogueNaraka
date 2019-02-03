using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    public GameObject btnLayout;
    public Image deathPnl;
    public Image soulPnl;

    public Image youDied;
    public Image pauseBtn;

    public TextMeshProUGUI soulRefiningRateTxt;

    void Awake()
    {
        instance = this;
    }

    public void SetDeathPnl(bool value)
    {
        deathPnl.gameObject.SetActive(value);
        //CameraShake.instance.Shake(0.2f, 0.2f, 0.01f);
        if (value)
        {
            btnLayout.SetActive(false);
            StartCoroutine(PumpCorou(youDied.rectTransform, 3, 0.5f));
        }
    }

    public void OnDeath()
    {
        //RankManager.instance.SendPlayerRank();
        GameManager.instance.Save();

        SetDeathPnl(true);

        StartCoroutine(SoulPnlCorou(1));

        pauseBtn.gameObject.SetActive(false);
    }

    IEnumerator SoulPnlCorou(float t)
    {
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        if (PlayerPrefs.GetFloat("lastRefiningRate") != -1)
        {
            SetSoulPnl(true);
        }
    }

    public void SetSoulPnl(bool value)
    {
        if(value)
        {
            float lastRefiningRate = PlayerPrefs.GetFloat("lastRefiningRate");
            float refiningRate = lastRefiningRate == 0 ? MoneyManager.instance.GetRandomRefiningRate() : lastRefiningRate;
            PlayerPrefs.SetFloat("lastRefiningRate", refiningRate);
            soulPnl.gameObject.SetActive(true);
            StartCoroutine(SoulRefiningRateTxtCorou((int)(refiningRate * 100)));
            StartCoroutine(PumpCorou(soulPnl.rectTransform, 0f, 0.25f));
        }
        else
        {
            soulPnl.gameObject.SetActive(false);
            btnLayout.SetActive(true);
            PlayerPrefs.SetFloat("lastRefiningRate", -1);
        }
    }

    IEnumerator SoulRefiningRateTxtCorou(int rate)
    {
        float delay = 1f / rate;
        for (int i = 0; i <= rate; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);
            soulRefiningRateTxt.text = string.Format("{0}%", i);
        }

        StartCoroutine(SoulAutoCloseCorou(rate));
    }

    IEnumerator SoulAutoCloseCorou(int rate)
    {
        float t = 5;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        if (soulPnl.gameObject.activeSelf)
        {
            SetSoulPnl(false);
        }
    }

    public void ReGame()
    {
        SetDeathPnl(false);
        PlayerPrefs.SetInt("isRun", 0);
        SkillManager.instance.ResetSave();

        BoardManager.instance.ClearStage();
        GameManager.instance.Load();
        PlayerPrefs.SetFloat("lastRefiningRate", 0);

        pauseBtn.gameObject.SetActive(true);
    }

    public void OpenSoulShop()
    {
        SoulShopManager.instance.SetSoulShop(true);
    }

    IEnumerator PumpCorou(RectTransform rect, float size, float t)
    {
        RectTransform imgRect = rect;
        Vector3 origin = imgRect.localScale;
        imgRect.localScale = new Vector3(size, size, 0);
        float tt = 0;
        while (tt < t)
        {
            yield return null;
            tt += Time.unscaledDeltaTime;
            imgRect.localScale = Vector3.Lerp(imgRect.localScale, origin, tt / t);
        }
        imgRect.localScale = origin;
    }
}
