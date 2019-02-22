using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class LevelUpManager : MonoBehaviour {

    public RollManager rollManager;

    public TextMeshProUGUI[] upgradeTxt;
    public TextMeshProUGUI leftStatTxt;
    public Fade fade;

    public GameObject statPnl;

    public Button cancelBtn;

    public Unit player { get { return BoardManager.instance.player; } }

    public int leftStat
    {   get { return PlayerPrefs.GetInt("leftStat"); }
        set { if(leftStat == 0) _leftStat = value; PlayerPrefs.SetInt("leftStat", value); }
    }

    public int _leftStat
    {
        get { return PlayerPrefs.GetInt("_leftStat"); }
        set { PlayerPrefs.SetInt("_leftStat", value); }
    }

    public bool isLeftStatChanged
    {
        get { return leftStat != 0 && leftStat != _leftStat; }
    }
    public static LevelUpManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        isLevelUp = PlayerPrefs.GetInt("isLevelUp") == 1;
    }

    float time;
    bool isLevelUp;

    void Update()
    {
        if (!FadeManager.instance.pnl.gameObject.activeSelf && !isLevelUp && player && player.gameObject.activeSelf && BoardManager.instance.enemies.Count <= 0)
        {
            player.autoMoveable.enabled = false;
            player.moveable.agent.Stop();

            fade.FadeOut();
            isLevelUp = true;
            //if (endStageCorou == null)
            //{
            //    endStageCorou = EndStageCorou();
            //    StartCoroutine(endStageCorou);
            //}
        }
    }

    //IEnumerator endStageCorou;
    //IEnumerator EndStageCorou(float time = 3.5f)
    //{
    //    PlayerPrefs.SetInt("isLevelUp", 1);
    //    fade.FadeIn();
    //    do
    //    {
    //        yield return null;
    //        time -= Time.deltaTime;
    //    } while (time > 0);

    //    LevelUp();
    //    endStageCorou = null;
    //}

    /// <summary>
    /// Player가 Enemy를 모두 처치하고 화면 상단에 도착하면 호출
    /// </summary>
    /// 
    public void LevelUp()
    {
        Debug.Log("LevelUp");
        GameManager.instance.SetPause(true);
        PlayerPrefs.SetInt("isLevelUp", 1);
        SyncStatUpgradeTxt();
        SkillManager.instance.SetIsDragable(false);
        if (isLeftStatChanged)
        {
            SetStatPnl(true);
        }
        else
        {
            rollManager.SetRollPnl(true);
        }
        GameManager.instance.Save();
        time = 0;
        player.moveable.agent.Stop();
        player.autoMoveable.enabled = true;
    }

    bool lastChance;

    public void SetStatPnl(bool value, int leftStat)
    {
        if(value)
        {
            if (leftStat > 0)
                this.leftStat = leftStat;
            leftStatTxt.text = leftStat + " Points";
            statPnl.SetActive(value);
            SyncStatUpgradeTxt();
        }
        else
        {
            if (isLeftStatChanged)
            {
                
                if (!lastChance)
                {
                    if (GameManager.instance.soulShopManager.shopStage <= 1)
                    {
                        GameManager.instance.soulShopManager.SetSoulShop(true);
                    }
                    lastChance = true;
                    return;
                }
                else
                {
                    statPnl.SetActive(false);
                    rollManager.SetRollPnl(false);
                }
            }
            else
            {
                rollManager.SetRollPnl(true);
                statPnl.SetActive(false);
            }
        }
        
    }

    public void SetStatPnl(bool value)
    {
        SetStatPnl(value, leftStat);
    }

    public void StatUp(int type)
    {
        if (player.data.stat.AddOrigin((STAT)type, 1))
        {
            Debug.Log("Stat Upgraded");
            if (--leftStat <= 0)
                rollManager.SetRollPnl(false);
            leftStatTxt.text = leftStat + " Points";
            SyncStatUpgradeTxt();
            GameManager.instance.StatTextUpdate();
        }
        else
        {
            Debug.Log("Stat Maxed");
        }
    }

    public IEnumerator EndLevelUp()
    {
        Debug.Log("EndLevelUp");
        cancelBtn.interactable = false;
        statPnl.SetActive(false);

        float t = 0;
        while (t < 0.1f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        
        PlayerPrefs.SetInt("isLevelUp", 0);
        cancelBtn.interactable = true;
        BoardManager.instance.StageUp();
        //BoardManager.instance.InitBoard();
        GameManager.instance.Save();
        GameManager.instance.SetPause(false);
        leftStat = 0;
        lastChance = false;
        isLevelUp = false;
    }

    public void SyncStatUpgradeTxt()
    {
        for (int i = 0; i < 7; i++)
        {
            upgradeTxt[i].text = string.Format("{0}/{1}", player.data.stat.GetOrigin((STAT)i), player.data.stat.GetMax((STAT)i));
            //upgradeTxt[0].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.DMG), player.data.stat.GetMax(STAT.DMG));
            //upgradeTxt[1].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.SPD), player.data.stat.GetMax(STAT.SPD));
            //upgradeTxt[2].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.TEC), player.data.stat.GetMax(STAT.TEC));
            //upgradeTxt[3].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.HP), player.data.stat.GetMax(STAT.HP));
            //upgradeTxt[4].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.MP), player.data.stat.GetMax(STAT.MP));
            //upgradeTxt[5].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.HPREGEN), player.data.stat.GetMax(STAT.HPREGEN));
            //upgradeTxt[6].text = string.Format("{0}/{1}", player.data.stat.GetOrigin(STAT.MPREGEN), player.data.stat.GetMax(STAT.MPREGEN));
        }
    }
}
