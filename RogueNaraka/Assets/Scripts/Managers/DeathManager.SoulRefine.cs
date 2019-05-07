using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//About Soul Refine Fuction
public partial class DeathManager : MonoBehaviour {
    [Header("Soul Refine Panel Setting")]
    public GameObject SoulRefiningPnl;
    public TextMeshProUGUI RefiningPercentText;
    public TextMeshProUGUI RemainTimeText;

    private bool isRefining = true;
    private float oriRefineRate;
    private DateTime endRefineDateTime;
    private TimeSpan OriRemainTime;

    public void SetActiveSoulRefiningPnl(bool _active) {
        if(_active) {
            this.SoulRefiningPnl.gameObject.SetActive(true);
        } else {
            this.SoulRefiningPnl.gameObject.SetActive(false);
            //SetActiveBtnLayout(true);
        }
    }

    public void SetSoulRefineData(float _refineRate, double _remainMinutes) {
        this.oriRefineRate = _refineRate;
        this.endRefineDateTime = DateTime.Now.AddMinutes(_remainMinutes);
        this.RefiningPercentText.text = string.Format("{0} %", (int)(_refineRate * 100.0f));
        StartCoroutine(CheckRemainTime());
    }

    public IEnumerator CheckRemainTime() {
        WaitForFixedUpdate checkYield = new WaitForFixedUpdate();
        OriRemainTime = TimeSpan.MaxValue;
        while(true) {
            TimeSpan remainTime = endRefineDateTime - DateTime.Now;
            if((OriRemainTime - remainTime).TotalSeconds >= 1) {
                OriRemainTime = remainTime;
                this.RemainTimeText.text = string.Format("남은 시간 : {0}", remainTime.ToString("mm'분 'ss'초'"));
            }
            yield return checkYield;
        }
    }

    ///<summary>소울 즉시 정제 버튼 클릭 함수</summary>
    public void OnClickNowRefineButton() {
        this.isRefining = false;
        MoneyManager.instance.RefineSoul();
        SetActiveSoulRefiningPnl(false);
    }
}