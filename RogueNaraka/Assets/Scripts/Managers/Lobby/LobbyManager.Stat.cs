using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class LobbyManager : MonoBehaviour
{
    [Header("Stat")]
    public Button StatBtn;
    public GameObject StatPnl;
    public TextMeshProUGUI StatCoinTxt;
    public TextMeshProUGUI[] StatName;
    public TextMeshProUGUI[] StatInfo;
    public GameObject[] StatEffects;

    public void OpenStatPnl(){
        for(int i = 0; i < StatName.Length; i++){
            StatName[i].text = GameDatabase.instance.statLang[(int)GameManager.language].items[i];
        }
        MoneyManager.instance.Load();
        StatCoinTxt.text = "Coin : " + MoneyManager.instance.soul;
        SyncStatUpgradeTxt();
        StatPnl.SetActive(true);
        DungeonPnl.SetActive(false);

        if (OnSelectButtonCorou != null)
            StopCoroutine(OnSelectButtonCorou);
        OnSelectButtonCorou = StartCoroutine(OnSelectButton(MenuButtons[3]));
    }

    public void SyncStatUpgradeTxt(){
        Stat stat = (Stat)Stat.JsonToStat(PlayerPrefs.GetString("stat")).Clone();

        for(int i = 0; i < StatName.Length; i++){
            int statAmount = (int)stat.GetMax((STAT)i);
            StatInfo[i].text = statAmount.ToString()+"\nPrice:"+GetRequiredSoul(statAmount);
        }
    }

    private int GetRequiredSoul(STAT type)
    {
        Stat stat = (Stat)Stat.JsonToStat(PlayerPrefs.GetString("stat")).Clone();
        float target = 0;
        switch(type)
        {
            case STAT.DMG:
                target = stat.dmgMax;
                break;
            case STAT.SPD:
                target = stat.spdMax;
                break;
            case STAT.HP:
                target = stat.hpMax;
                break;
            case STAT.MP:
                target = stat.mpMax;
                break;
            case STAT.SP:
                target = stat.statPoints;
                break;
        }

        return GetRequiredSoul((int)target);
    }

    private int GetRequiredSoul(int num){
        return (int)((num - 4) * 10);
    }

    public void StatUpgrade(int type)
    {
        STAT statType = (STAT)type;
        int required = GetRequiredSoul(statType);
        if (MoneyManager.instance.soul >= required)
        {
            Stat stat = (Stat)Stat.JsonToStat(PlayerPrefs.GetString("stat")).Clone();
            stat.AddMax(statType, 1);
            PlayerPrefs.SetString("stat", Stat.StatToJson(stat));
            MoneyManager.instance.AddSoul(-required);
            SyncStatUpgradeTxt();
            StatCoinTxt.text = "Coin : " + MoneyManager.instance.soul;
            AudioManager.instance.PlaySFX("statUp");
            StatEffects[type].SetActive(false);
            StatEffects[type].SetActive(true);
        }
        
    }
}
