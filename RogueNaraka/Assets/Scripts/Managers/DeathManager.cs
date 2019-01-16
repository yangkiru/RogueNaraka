using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    public Image pnl;
    public Image youDied;
    public Image pauseBtn;

    void Awake()
    {
        instance = this;
    }

    public void SetDeathPnl(bool value)
    {
        pnl.gameObject.SetActive(value);
        //CameraShake.instance.Shake(0.2f, 0.2f, 0.01f);
        StartCoroutine(IconEffectCorou());
    }

    public void OnDeath()
    {
        RankManager.instance.SendPlayerRank();
        GameManager.instance.Save();
        SetDeathPnl(true);
        
        MoneyManager.instance.RandomRefineSoul();
        pauseBtn.gameObject.SetActive(false);
    }

    public void ReGame()
    {
        SetDeathPnl(false);
        PlayerPrefs.SetInt("isRun", 0);
        SkillManager.instance.ResetSave();

        BoardManager.instance.ClearStage();
        GameManager.instance.Load();

        pauseBtn.gameObject.SetActive(true);
    }

    public void OpenSoulShop()
    {
        SoulShopManager.instance.SetSoulShop(true);
    }

    IEnumerator IconEffectCorou()
    {
        float size = 3f;
        float t = 0.5f;
        RectTransform imgRect = youDied.rectTransform;
        imgRect.localScale = new Vector3(size, size, 0);
        Vector3 two = new Vector3(2, 2, 1);
        while (t > 0)
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
            imgRect.localScale = Vector3.Lerp(imgRect.localScale, two, 1 - t * 2);
        }
        imgRect.localScale = two;
    }
}
