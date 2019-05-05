﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SingletonPattern;

public class StageSkipManager : MonoSingleton<StageSkipManager>
{
    public Button upBtn;
    public Button downBtn;
    public TextMeshProUGUI stageTxt;
    public GameObject stageSkipPnl;

    public GameObject resultPnl;
    public TextMeshProUGUI statAmountTxt;
    public TextMeshProUGUI[] statTxts;

    Stat randomStat;

    public bool IsSkipStage { get { return PlayerPrefs.GetInt("isSkipStage") == 1; } set { PlayerPrefs.SetInt("isSkipStage", value ? 1 : 0); } }

    public int SelectedStage { get { return PlayerPrefs.GetInt("selectedStage"); } set { PlayerPrefs.SetInt("selectedStage", value); } }
    public int selectedStage = 1;

    public void SetStageSkipPnl(bool value)
    {
        if (value)
        {
            Init();
            stageSkipPnl.SetActive(true);
        }
        else
            stageSkipPnl.SetActive(false);
    }

    public void Init()
    {
        selectedStage = 1;
        downBtn.interactable = false;
        upBtn.interactable = GetSkipableStage() > 1;
        stageTxt.text = "1 Stage";
    }

    public void UpDownSkipStage(bool isUp)
    {
        int max = GetSkipableStage();
        if (isUp && max > selectedStage)
        {
            selectedStage += 30;
            downBtn.interactable = true;
            if (max <= selectedStage)
                upBtn.interactable = false;
        }
        else if (!isUp && selectedStage > 1)
        {
            selectedStage -= 30;
            upBtn.interactable = true;
            if (selectedStage == 1)
                downBtn.interactable = false;
        }
        stageTxt.text = string.Format("{0} Stage", selectedStage);
    }

    public void SkipStage()
    {
        DeathManager.instance.EndGame();
        IsSkipStage = true;
        SelectedStage = selectedStage;
        GameManager.instance.LoadInit(selectedStage);
        GameManager.instance.Load();
        RageManager.instance.Rage(selectedStage / 30);
        RageManager.instance.isRage = false;
        SetStageSkipPnl(false);

        
        //AddRandomStat(BoardManager.instance.player.stat, GetRandomStatAmount());
        //SetResultPnl(true);
    }

    public int GetSkipableStage()
    {
        Debug.Log((RankManager.instance.highScore - 1) / 30 * 30 + 1);
        return (RankManager.instance.highScore - 1) / 30 * 30 + 1;
    }

    public void SetResultPnl(bool value)
    {
        if (value)
        {
            for (int i = 0; i < statTxts.Length; i++)
                statTxts[i].gameObject.SetActive(false);
            statAmountTxt.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("SetResultPnl:false");
            Stat stat = Stat.DataToStat();
            if (stat != null)
                GameManager.instance.RunGame(stat);
            else
                Debug.Log("Empty Stat");
        }
        resultPnl.SetActive(value);
    }

    public int GetRandomBookAmount()
    {
        int value = selectedStage / 30 + 1;
        return Random.Range(value * 3, value * 10 + 1);
    }

    public int GetRandomStatAmount()
    {
        int value = selectedStage / 30;
        return Random.Range(value * 15, value * 45 + 1);
    }

    public void AddRandomStat(Stat stat, int amount)
    {
        randomStat = new Stat();
        statAmountTxt.text = amount.ToString();
        while(stat.sumMax != stat.sumOrigin)
        {
            if (amount <= 0)
                break;
            STAT type = (STAT)Random.Range(0, (int)STAT.MR + 1);
            if (stat.AddOrigin(type, 1))
            {
                randomStat.AddOrigin(type, 1, false, true);
                amount--;
            }
        }
        for (int i = 0; i < (int)STAT.MR + 1; i++)
        {
            statTxts[i].text = string.Format("{0} {1}", ((STAT)i).ToString(), randomStat.GetOrigin(i));
        }
    }
}
