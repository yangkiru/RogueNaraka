using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class StatManager : MonoBehaviour {

    public TextMeshProUGUI[] upgradeTxt;
    public TextMeshProUGUI leftStatTxt;

    public GameObject statPnl;

    public Unit player { get { return BoardManager.instance.player; } }

    public RollManager rollManager;

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
    public static StatManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
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
                    lastChance = true;
                    return;
                }
                else
                {
                    Debug.Log("Close");
                    this.leftStat = 0;
                    lastChance = false;
                    statPnl.SetActive(false);
                    rollManager.SetRollPnl(false);
                }
            }
            else if(leftStat != 0)
            {
                lastChance = false;
                rollManager.SetRollPnl(true);
                statPnl.SetActive(false);
            }
            else
            {
                lastChance = false;
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
            {
                rollManager.SetRollPnl(false);
                //leftStat = 0;
            }
            leftStatTxt.text = leftStat + " Points";
            SyncStatUpgradeTxt();
            Stat.StatToData(player.data.stat);
            GameManager.instance.StatTextUpdate();
        }
        else
        {
            GameManager.instance.soulShopManager.SetSoulShop(true);
            Debug.Log("Stat Maxed");
        }
    }



    public void SyncStatUpgradeTxt()
    {
        for (int i = 0; i < 7; i++)
        {
            upgradeTxt[i].text = string.Format("{0}/{1}", player.data.stat.GetOrigin((STAT)i), player.data.stat.GetMax((STAT)i));
        }
    }
}
