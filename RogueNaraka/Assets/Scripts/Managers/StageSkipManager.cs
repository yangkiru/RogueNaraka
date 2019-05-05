using System.Collections;
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
    public GameObject pnl;

    public bool IsSkipStage { get { return PlayerPrefs.GetInt("isSkipStage") == 1; } set { PlayerPrefs.SetInt("isSkipStage", value ? 1 : 0); } }
    public int selectedStage = 1;

    public void SetStageSkipPnl(bool value)
    {
        if (value)
        {
            Init();
            pnl.SetActive(true);
        }
        else
            pnl.SetActive(false);
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
        int max = GetSkipableStage() + 1;
        if (isUp && max > selectedStage)
        {
            selectedStage += 30;
            downBtn.interactable = true;
            if (max == selectedStage)
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
        GameManager.instance.LoadInit(selectedStage);
        GameManager.instance.Load();
        RageManager.instance.Rage(selectedStage / 30);
        RageManager.instance.isRage = false;
        SetStageSkipPnl(false);
    }

    public int GetSkipableStage()
    {
        return RankManager.instance.highScore / 30 * 30;
    }

    public int GetRandomExpItemAmount()
    {
        int value = selectedStage / 30 + 1;
        return Random.Range(value * 3, value * 10 + 1);
    }

    public int GetRandomStatAmount()
    {
        int value = selectedStage / 30 + 1;
        return Random.Range(value * 15, value * 45 + 1);
    }
}
