using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathManager : MonoBehaviour
{
    public static DeathManager instance;

    public Image pnl;

    void Awake()
    {
        instance = this;
    }

    public void SetDeathPnl(bool value)
    {
        pnl.gameObject.SetActive(value);
    }

    public void OnDeath()
    {
        GameManager.instance.Save();
        StartCoroutine(WaitForDeathAnimation());
    }

    public void ReGame()
    {
        SetDeathPnl(false);
        PlayerPrefs.SetInt("isRun", 0);
        BoardManager.instance.player.deathable.Revive();
        SkillManager.instance.ResetSave();

        BoardManager.instance.ClearStage();
        GameManager.instance.Load();
    }

    public void OpenSoulShop()
    {
        SoulShopManager.instance.SetSoulShop(true);
    }

    public IEnumerator WaitForDeathAnimation()
    {
        yield return null;
        MoneyManager.instance.RefineSoul();
        
        Debug.Log("Player Died");
        
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        SetDeathPnl(true);

        //player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        //t = 0;
        //while (t < 1)
        //{
        //    t += Time.unscaledDeltaTime;
        //    yield return null;
        //}
        //player.animator.updateMode = AnimatorUpdateMode.Normal;
    }
}
