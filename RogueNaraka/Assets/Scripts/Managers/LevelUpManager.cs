﻿using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance = null;

    public Fade fade;

    float time;

    public RollManager rollManager;
    public StatManager statManager;

    IEnumerator endStageCorou;

    static public bool IsLevelUp
    {
        get { return PlayerPrefs.GetInt("isLevelUp") == 1; }
        set { PlayerPrefs.SetInt("isLevelUp", value ? 1 : 0); }
    }

    public Unit player { get { return BoardManager.instance.player; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void RequestEndStageCorou()
    {
        if (endStageCorou != null)
            return;

        //Debug.Log("StartEndStageCoroutine!!!");
        endStageCorou = EndStageCorou();
        StartCoroutine(EndStageCorou());
    }

    IEnumerator EndStageCorou()
    {
        do
        {
            yield return null;
            if (player.deathable.isDeath)
            {
                endStageCorou = null;
                yield break;
            }
        } while (BoardManager.instance.enemies.Count != 0 || SoulParticle.soulCount != 0);
        Debug.Log("EndStage!!");
        OnEndStage();
        float t = 2;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        //AdMobManager.instance.RequestBanner();
        endStageCorou = null;
    }

    public void OnEndStage()
    {
        player.autoMoveable.enabled = false;
        player.moveable.Stop();

        fade.FadeOut();
    }

    public void OnFadeOut()
    {
        LevelUp();
    }

    public void LevelUp()
    {
        Debug.Log("LevelUp");
        BoardManager.instance.ClearStage();
        GameManager.instance.SetPause(true);
        IsLevelUp = true;
        statManager.SyncStatUpgradeTxt();
        //SkillManager.instance.SetIsDragable(false);
        if (statManager.isLeftStatChanged)
        {
            rollManager.SetRollPnl(true);
            rollManager.SetShowCase(RollManager.ROLL_TYPE.ALL);
            rollManager.SetOnFadeOut(rollManager.StageStart);
            statManager.SetStatPnl(true);
            //fade.FadeIn();
        }
        else
        {
            rollManager.SetRollPnl(true);
            rollManager.SetShowCase(RollManager.ROLL_TYPE.ALL);
            rollManager.Roll();
            rollManager.SetOnFadeOut(rollManager.StageStart);
        }
        GameManager.instance.Save();
        time = 0;
        player.moveable.Stop();
        player.autoMoveable.enabled = true;
    }
}
