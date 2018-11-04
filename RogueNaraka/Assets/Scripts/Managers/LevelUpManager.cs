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
        set { if(value == 0) _leftStat = value; PlayerPrefs.SetInt("leftStat", value); }
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
        if (GameManager.instance.soulShopManager.shopStage == 1)
            GameManager.instance.soulShopManager.SetSoulShop(true);
        if (isLeftStatChanged)
            SetStatPnl(true);
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
        }
        else
        {
            if (isLeftStatChanged)
            {
                if (lastChance)
                {
                    lastChance = false;
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
        SetStatPnl(value, 0);
    }

    public void StatUp(int type)
    {
        lastChance = true;
        if (player.AddStat((STAT)type, 1))
        {
            Debug.Log("Stat Upgraded");
            if (--leftStat <= 0)
                rollManager.SetRollPnl(false);
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
        yield return new WaitForSecondsRealtime(0.1f);
        if(SoulShopManager.instance.shopStage == 1)
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.RANDOM);
        else
            SoulShopManager.instance.ShopStage(SoulShopManager.SHOPSTAGE.DECREASE);
        PlayerPrefs.SetInt("isLevelUp", 0);
        cancelBtn.interactable = true;
        BoardManager.instance.StageUp();
        GameManager.instance.Save();
        GameManager.instance.SetPause(false);
    }

    public void SyncStatUpgradeTxt()
    {
        upgradeTxt[0].text = string.Format("{0}/{1}", player.data.stat.dmg.ToString(), player.maxStat.dmg);
        upgradeTxt[1].text = string.Format("{0}/{1}", player.data.stat.spd.ToString(), player.maxStat.spd);
        upgradeTxt[2].text = string.Format("{0}/{1}", player.data.stat.tec.ToString(), player.maxStat.tec);
        upgradeTxt[3].text = string.Format("{0}/{1}", player.data.stat.hp.ToString(), player.maxStat.hp);
        upgradeTxt[4].text = string.Format("{0}/{1}", player.data.stat.mp.ToString(), player.maxStat.mp);
        upgradeTxt[5].text = string.Format("{0}/{1}", player.data.stat.hpRegen.ToString(), player.maxStat.hpRegen);
        upgradeTxt[6].text = string.Format("{0}/{1}", player.data.stat.mpRegen.ToString(), player.maxStat.mpRegen);
    }
}
