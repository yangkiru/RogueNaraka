using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RogueNaraka.PopUpScripts;
using RogueNaraka.NotificationScripts;

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
    private LocalPushManager.AndroidLocalPush refineLocalPush;
    
    private Coroutine checkRemainTimeCoroutine;

    public void SetActiveSoulRefiningPnl(bool _active) {
        if(_active) {
            this.SoulRefiningPnl.gameObject.SetActive(true);
        } else {
            this.SoulRefiningPnl.gameObject.SetActive(false);
            SetActiveBtnLayout(true);
        }
    }

    public void SetSoulRefineData(float _refineRate) {
        this.oriRefineRate = _refineRate;
        double minutesForRefine = GetMinutesForRefineSoul(
            (int)(MoneyManager.instance.unrefinedSoul * (1.0f - this.oriRefineRate)));
        if(minutesForRefine == 0) {
            SetActiveSoulRefiningPnl(false);
            GainRefinedSoul();
        } else {
            this.isRefining = true;
            this.endRefineDateTime = DateTime.Now.AddMinutes(minutesForRefine);
            this.RefiningPercentText.text = string.Format("{0} %", (int)(this.oriRefineRate * 100.0f));
            #if !UNITY_EDITOR
                int msgIdx = UnityEngine.Random.Range(0, 3);
                switch(msgIdx) {
                    case 0:
                        this.refineLocalPush = LocalPushManager.Instance.SetLocalPush("소울 정제 완료!", "주인공을 강화해주세요.", this.endRefineDateTime);
                    break;
                    case 1:
                        this.refineLocalPush = LocalPushManager.Instance.SetLocalPush("소울 정제가 완료 되었습니다.", "나라카로 돌아오세요!", this.endRefineDateTime);
                    break;
                    case 2:
                        this.refineLocalPush = LocalPushManager.Instance.SetLocalPush("소울 정제가 완료 되었습니다.", "지금 바로 업그레이드하세요!", this.endRefineDateTime);
                    break;
                }
            #endif
            //Save
            PlayerPrefs.SetInt("IsRefining", 1);
            PlayerPrefs.SetString("EndRefineDateTime", this.endRefineDateTime.ToString());
            PlayerPrefs.SetFloat("OriRefineRate", this.oriRefineRate);
            this.checkRemainTimeCoroutine = StartCoroutine(CheckRemainTime());
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
        GainRefinedSoul();
    }

    public void LoadRefiningData() {
        //Load Refining Data
        if(PlayerPrefs.GetInt("IsRefining") == 1) {
            this.isRefining = true;
        }
        if(PlayerPrefs.GetString("EndRefineDateTime") != "") {
            this.endRefineDateTime = DateTime.Parse(PlayerPrefs.GetString("EndRefineDateTime"));
            this.checkRemainTimeCoroutine = StartCoroutine(CheckRemainTime());
        }
        this.oriRefineRate = PlayerPrefs.GetFloat("OriRefineRate");
        this.RefiningPercentText.text = string.Format("{0} %", (int)(this.oriRefineRate * 100.0f));
    }

    private double GetMinutesForRefineSoul(int _unrefinedSoulAmount) {
        for(int i = 0; i < GameDatabase.instance.requiredMinutesForSoulRefineArray.Length; ++i) {
            if(_unrefinedSoulAmount <= GameDatabase.instance.requiredMinutesForSoulRefineArray[i].endAmount) {
                return GameDatabase.instance.requiredMinutesForSoulRefineArray[i].requiredMinutes;
            }
        }
        throw new ArgumentException(string.Format("Not Correct Refine Soul Amount : {0}", _unrefinedSoulAmount));
    }

    private void GainRefinedSoul(float _refineRate = 1.0f) {
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
            string.Format(popupContext, (int)(MoneyManager.instance.unrefinedSoul * _refineRate)),
            (OneButtonPopUpController _popup) => {
                _popup.DeactivatePopUp();
                PopUpManager.Instance.DeactivateBackPanel();
                GameManager.instance.SetPause(false);
            });
        this.isRefining = false;

        MoneyManager.instance.RefineSoul(_refineRate);
        
        PlayerPrefs.SetInt("IsRefining", 0);
        PlayerPrefs.SetString("EndRefineDateTime", "");
        PlayerPrefs.SetFloat("OriRefineRate", 0.0f);
    }

    ///<summary>소울 즉시 정제 버튼 클릭 함수</summary>
    public void OnClickNowRefineButton() {
        #if !UNITY_EDITOR
            LocalPushManager.Instance.CancelLocalPush(this.refineLocalPush);
        #endif
        StopCoroutine(this.checkRemainTimeCoroutine);
        this.isRefining = false;
        PlayerPrefs.SetInt("IsRefining", 0);
        GainRefinedSoul(this.oriRefineRate);
        this.refineLocalPush = null;
        SetActiveSoulRefiningPnl(false);
    }
}