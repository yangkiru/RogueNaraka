using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class LevelUpManager : MonoBehaviour {

    public RollManager rollManager;

    public Text[] upgradeTxt;
    public TextMeshProUGUI leftStatTxt;

    public GameObject selectPnl;
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
    }

    float time;
    bool isLevelUp;

    void Update()
    {
        if (!isLevelUp && player && BoardManager.instance.enemies.Count <= 0)
        {
            time -= Time.deltaTime;
            if (time > 0)
                return;
            else
                time = 0.5f;
            player.autoMoveable.enabled = false;
            player.moveable.Move(BoardManager.instance.goalPoint);
            float distance = Vector2.Distance(player.transform.position, BoardManager.instance.goalPoint);
            if (distance <= 0.5f)
            {
                LevelUp();
            }
        }
    }

    /// <summary>
    /// Player가 Enemy를 모두 처치하고 화면 상단에 도착하면 호출
    /// </summary>
    /// 
    public void LevelUp()
    {
        isLevelUp = true;
        Debug.Log("LevelUp");
        GameManager.instance.SetPause(true);
        SyncStatUpgradeTxt();
        SkillManager.instance.SetIsDragable(false);
        if (GameManager.instance.soulShopManager.shopStage <= 1)
            GameManager.instance.soulShopManager.SetSoulShop(true);
        if (isLeftStatChanged)
        {
            Debug.Log("남은 스탯이 있따:" + leftStat + " " + _leftStat);
            SetStatPnl(true);
        }
        else
        {
            rollManager.SetRollPnl(true);
        }
        PlayerPrefs.SetInt("isLevelUp", 1);
        GameManager.instance.Save();
        time = 0;
        player.autoMoveable.enabled = true;
        player.moveable.agent.Stop();
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
                Debug.Log("남은 스탯이 있따:" + this.leftStat + " " + _leftStat);
                
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
                Debug.Log("남은 스탯이 없따:" + leftStat + " " + _leftStat);
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
        if (player.data.stat.AddCurrent((STAT)type, 1))
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
        //SetSelectPnl(false);
#if DELAY
        yield return GameManager.instance.delayPointOneReal;
#else
        float t = 0;
        while (t < 0.1f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
#endif
        if (SoulShopManager.instance.shopStage <= 1)
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.SET);
        else
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.DECREASE);
        PlayerPrefs.SetInt("isLevelUp", 0);
        cancelBtn.interactable = true;
        BoardManager.instance.StageUp();
        BoardManager.instance.InitBoard();
        GameManager.instance.Save();
        GameManager.instance.SetPause(false);
        leftStat = 0;
        lastChance = false;
        isLevelUp = false;
    }

    public void SyncStatUpgradeTxt()
    {
        upgradeTxt[0].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.DMG), player.data.stat.GetMax(STAT.DMG));
        upgradeTxt[1].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.SPD), player.data.stat.GetMax(STAT.SPD));
        upgradeTxt[2].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.TEC), player.data.stat.GetMax(STAT.TEC));
        upgradeTxt[3].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.HP), player.data.stat.GetMax(STAT.HP));
        upgradeTxt[4].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.MP), player.data.stat.GetMax(STAT.MP));
        upgradeTxt[5].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.HPREGEN), player.data.stat.GetMax(STAT.HPREGEN));
        upgradeTxt[6].text = string.Format("{0}/{1}", player.data.stat.GetCurrent(STAT.MPREGEN), player.data.stat.GetMax(STAT.MPREGEN));
    }
}
