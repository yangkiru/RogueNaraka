using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RogueNaraka.PopUpScripts;

//About Soul Refine Fuction
public partial class DeathManager : MonoBehaviour {
    [Header("Soul Refine Panel Setting")]
    public GameObject SoulRefiningPnl;
    public TextMeshProUGUI RefiningPercentText;
    public TextMeshProUGUI RemainTimeText;

    private bool isRefining;
    private float oriRefineRate;
    private DateTime endRefineDateTime;
    private TimeSpan oriRemainTime;
    private int unrefinedSoulAmount;

    public void SetActiveSoulRefiningPnl(bool _active) {
        if(_active) {
            this.SoulRefiningPnl.gameObject.SetActive(true);
        } else {
            this.SoulRefiningPnl.gameObject.SetActive(false);
            SetActiveBtnLayout(true);
        }
    }

    public void SetSoulRefineData(float _refineRate, int _unrefinedSoulAmount) {
        this.oriRefineRate = _refineRate;
        int refinedSoulAmount = (int)(_unrefinedSoulAmount * _refineRate);
        MoneyManager.instance.AddSoul(refinedSoulAmount, true);
        this.unrefinedSoulAmount = _unrefinedSoulAmount - refinedSoulAmount;
        double minutesForRefine = GetMinutesForRefineSoul(this.unrefinedSoulAmount);
        if(minutesForRefine == 0) {
            SetActiveSoulRefiningPnl(false);
            GainRefinedSoul(this.unrefinedSoulAmount);
        } else {
            this.isRefining = true;
            PlayerPrefs.SetInt("IsRefining", 1);
            PlayerPrefs.SetInt("UnrefinedSoulAmount", this.unrefinedSoulAmount);
            this.endRefineDateTime = DateTime.Now.AddMinutes(minutesForRefine);
            this.RefiningPercentText.text = string.Format("{0} %", (int)(_refineRate * 100.0f));
            StartCoroutine(CheckRemainTime());
        }
    }

    public IEnumerator CheckRemainTime() {
        WaitForFixedUpdate checkYield = new WaitForFixedUpdate();
        
        this.oriRemainTime = TimeSpan.MaxValue;
        TimeSpan remainTime;
        do {
            remainTime = endRefineDateTime - DateTime.Now;
            if(((int)this.oriRemainTime.TotalSeconds - (int)remainTime.TotalSeconds) >= 1) {
                this.oriRemainTime = remainTime;
                this.RemainTimeText.text = string.Format("남은 시간 : {0:D2}분 {1:D2}초", (int)remainTime.TotalMinutes, (int)remainTime.Seconds);
            }
            yield return checkYield;
        } while(remainTime.TotalSeconds > 0);

        SetActiveSoulRefiningPnl(false);
        GainRefinedSoul(this.unrefinedSoulAmount);
    }

    private double GetMinutesForRefineSoul(int _unrefinedSoulAmount) {
        for(int i = 0; i < GameDatabase.instance.requiredMinutesForSoulRefineArray.Length; ++i) {
            if(_unrefinedSoulAmount <= GameDatabase.instance.requiredMinutesForSoulRefineArray[i].endAmount) {
                return GameDatabase.instance.requiredMinutesForSoulRefineArray[i].requiredMinutes;
            }
        }
        throw new ArgumentException(string.Format("Not Correct Refine Soul Amount : {0}", _unrefinedSoulAmount));
    }

    private void GainRefinedSoul(int _refinedSoul) {
        MoneyManager.instance.AddSoul(_refinedSoul, true);
        PlayerPrefs.SetInt("UnrefinedSoulAmount", 0);
        string popupContext = "";
        switch(GameManager.language) {
            case Language.English:
                popupContext = "You have refined {0} Souls.";
            break;
            case Language.Korean:
                popupContext = "{0} 소울을 정제하였습니다.";
            break;
        }
        PopUpManager.Instance.ActivateOneButtonPopUp(
            string.Format(popupContext, _refinedSoul),
            (OneButtonPopUpController _popup) => {
                _popup.DeactivatePopUp();
                PopUpManager.Instance.DeactivateBackPanel();
                GameManager.instance.SetPause(false);
            });
        this.isRefining = false;
        PlayerPrefs.SetInt("IsRefining", 0);
    }

    ///<summary>소울 즉시 정제 버튼 클릭 함수</summary>
    public void OnClickNowRefineButton() {
        this.isRefining = false;
        PlayerPrefs.SetInt("IsRefining", 0);
        MoneyManager.instance.RefineSoul();
        SetActiveSoulRefiningPnl(false);
    }
}