using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    public GameObject[] btnLayout;
    public Image deathPnl;
    public Image soulPnl;

    public Image youDied;
    public Image pauseBtn;

    public TextMeshProUGUI soulRefiningRateTxt;
    public TextMeshProUGUI unSoulTxt;
    public TextMeshProUGUI soulTxt;

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
            btnLayout[0].SetActive(false);
            btnLayout[1].SetActive(false);
            StartCoroutine(PumpCorou(youDied.rectTransform, 3, 0.5f));
        }
    }

    public void OnDeath()
    {
        //RankManager.instance.SendPlayerRank();
        if(BoardManager.instance.player)
            GameManager.instance.Save();

        RankManager.instance.SendPlayerRank();

        SetDeathPnl(true);

        StartCoroutine(SoulPnlCorou(1));

        pauseBtn.gameObject.SetActive(false);

        AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomDeathMusic());
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
        else
        {
            btnLayout[0].SetActive(true);
            btnLayout[1].SetActive(true);
        }
    }

    public void SetSoulPnl(bool value)
    {
        if(value)
        {
            float lastRefiningRate = PlayerPrefs.GetFloat("lastRefiningRate");
            float refiningRate = lastRefiningRate == 0 ? MoneyManager.instance.GetRandomRefiningRate() : lastRefiningRate;
            soulTxt.text = "0";
            unSoulTxt.text = "0";
            PlayerPrefs.SetFloat("lastRefiningRate", refiningRate);
            soulPnl.gameObject.SetActive(true);
            rate = refiningRate;
            StartCoroutine(SoulRefiningRateTxtCorou(refiningRate));
            StartCoroutine(PumpCorou(soulPnl.rectTransform, 0f, 0.25f));
            soulCorou = SoulCorou();
            StartCoroutine(soulCorou);
            AdMobManager.instance.RequestRewardBasedVideo();
        }
        else
        {
            soulPnl.gameObject.SetActive(false);
            btnLayout[0].SetActive(true);
            btnLayout[1].SetActive(true);
            PlayerPrefs.SetFloat("lastRefiningRate", -1);
        }
    }

    IEnumerator SoulRefiningRateTxtCorou(float rate)
    {
        int intRate = (int)(rate * 100);
        float delay = 1f / intRate;
        for (int i = 0; i <= intRate; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);

            if (Input.anyKey)
                i = intRate;

            soulRefiningRateTxt.text = string.Format("{0}%", i);
        }

        int unSoul = MoneyManager.instance.unrefinedSoul;
        delay = 0.5f / intRate;
        for (int i = 0; i <= unSoul; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);

            if (Input.anyKey)
                i = unSoul;

            unSoulTxt.text = string.Format("{0}<size=12><sprite=0></size>", i);
        }

        int soul = (int)(unSoul * rate);
        delay = 0.5f / intRate;
        for (int i = 0; i <= soul; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);
            if (Input.anyKey)
                i = soul;

            soulTxt.text = string.Format("{0}<size=12><sprite=0></size>", i);
        }
    }

    bool isADReward;
    //-    public bool isClose { get; set; }
    //-
    //-    IEnumerator SoulAutoCloseCorou(float rate)
    //-    {
    //-        float t = 6;
    //-        do
    //-        {
    //-            yield return null;
    //-            t -= Time.unscaledDeltaTime* (isADActive? 0 : 1);
    //-            if(isADReward)
    //-            {
    //-                MoneyManager.instance.RefineSoul(rate* 2);
    //-                SetSoulPnl(false);
    //-                yield break;
    //-            }
    //-            if(isClose)
    //-            {
    //-                MoneyManager.instance.RefineSoul(rate);
    //-                SetSoulPnl(false);
    //-                isClose = false;
    //-                yield break;
    //-            }
    //-        } while (t > 0);
    //-
    //-        if (soulPnl.gameObject.activeSelf)
    //-        {
    //-            MoneyManager.instance.RefineSoul(rate);
    //-            SetSoulPnl(false);
    //-        }
    //-    }

    IEnumerator soulCorou;
    IEnumerator SoulCorou()
    {
        while(true)
        {
            yield return null;
            if(isADReward)
            {
                yield return null;
                isADReward = false;
                MoneyManager.instance.RefineSoul(rate * 2);
                SetSoulPnl(false);
                break;
            }
        }
        soulCorou = null;
    }

    float rate;

    public void OnSoulRefiningRatePnlClose()
    {
        StopCoroutine(soulCorou);
        MoneyManager.instance.RefineSoul(rate);
        SetSoulPnl(false);
    }

    public void OnADReward()
    {
        isADReward = true;
    }

    public void ReGame()
    {
        SetDeathPnl(false);

        //AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomMainMusic());
        PlayerPrefs.SetInt("isRun", 0);
        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        Item.instance.InitItem();

        BoardManager.instance.ClearStage();
        GameManager.instance.Load();
        PlayerPrefs.SetFloat("lastRefiningRate", 0);

        BoardManager.instance.player.hpable.SetFullHp();
        BoardManager.instance.player.mpable.SetFullMp();

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
