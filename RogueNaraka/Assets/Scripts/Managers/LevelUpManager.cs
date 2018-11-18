using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour {

    public RollManager rollManager;

    public Text[] upgradeTxt;
    public TextMeshProUGUI leftStatTxt;

    public GameObject selectPnl;
    public GameObject statPnl;

    public Button cancelBtn;

    public Player player;

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

    /// <summary>
    /// Player가 Enemy를 모두 처치하고 화면 상단에 도착하면 호출
    /// </summary>
    /// 
    public void LevelUp()
    {
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
        if (GameManager.AddStat((STAT)type, 1))
        {
            Debug.Log("Stat Upgraded");
            if (--leftStat <= 0)
                rollManager.SetRollPnl(false);
            leftStatTxt.text = leftStat + " Points";
            SyncStatUpgradeTxt();
            GameManager.instance.SyncPlayerStat();
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
#if !DELAY
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.1f);
#endif
        //SetSelectPnl(false);
#if DELAY
        yield return GameManager.instance.delayPointOneReal;
#else
        yield return delay;
#endif
        if (SoulShopManager.instance.shopStage <= 1)
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.SET);
        else
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.DECREASE);
        PlayerPrefs.SetInt("isLevelUp", 0);
        cancelBtn.interactable = true;
        BoardManager.instance.StageUp();
        GameManager.instance.Save();
        GameManager.instance.SetPause(false);
        leftStat = 0;
        lastChance = false;
    }

    public void SyncStatUpgradeTxt()
    {
        upgradeTxt[0].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.DMG, false), GameManager.GetStat(STAT.DMG, true));
        upgradeTxt[1].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.SPD, false), GameManager.GetStat(STAT.SPD, true));
        upgradeTxt[2].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.TEC, false), GameManager.GetStat(STAT.TEC, true));
        upgradeTxt[3].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.HP, false), GameManager.GetStat(STAT.HP, true));
        upgradeTxt[4].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.MP, false), GameManager.GetStat(STAT.MP, true));
        upgradeTxt[5].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.HPREGEN, false), GameManager.GetStat(STAT.HPREGEN, true));
        upgradeTxt[6].text = string.Format("{0}/{1}", GameManager.GetStat(STAT.MPREGEN, false), GameManager.GetStat(STAT.MPREGEN, true));
    }
}
