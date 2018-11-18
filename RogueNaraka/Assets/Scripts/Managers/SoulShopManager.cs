using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoulShopManager : MonoBehaviour
{
    public GameObject shopPnl;
    public GameObject statPnl;
    public GameObject checkPnl;

    public Button okBtn;
    public Button cancelBtn;
    public Button[] statUpgradeBtn;

    public Text checkTxt;
    public TextMeshProUGUI[] statUpgradeTxt;
    public TextMeshProUGUI[] statUpgradeBtnTxt;
    public int shopStage
    { get { return _shopStage; } }
    [SerializeField]
    private int _shopStage;

    /// <summary>
    /// Ok == true or Cancel == false
    /// </summary>
    private bool selected;
    /// <summary>
    /// 선택한 Stat 업그레이드
    /// </summary>
    private STAT statUpgrading;
    /// <summary>
    /// 업그레이드 클릭시 재클릭 금지
    /// </summary>
    private bool isUpgrading;

    private IEnumerator currentCoroutine;

    public static SoulShopManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public enum SHOPSTAGE
    { SET, DECREASE, SYNC}
    /// <summary>
    /// ShopStage 관련 함수
    /// </summary>
    /// <param name="act"></param>
    public void ShopStage(SHOPSTAGE act)
    {
        switch(act)
        {
            case SHOPSTAGE.SET:
                _shopStage = 5;
                PlayerPrefs.SetInt("shopStage", shopStage);
                break;
            case SHOPSTAGE.DECREASE:
                _shopStage--;
                PlayerPrefs.SetInt("shopStage", shopStage);
                break;
            case SHOPSTAGE.SYNC:
                _shopStage = PlayerPrefs.GetInt("shopStage");
                break;
        }
    }

    bool isStatUpgrading = false;
    /// <summary>
    /// 업그레이드 버튼을 누르면 upgrading에 값이 전달됨
    /// </summary>
    /// <param name="type"></param>
    public void SelectStatUpgrade(int type)
    {
        if (isStatUpgrading && statUpgrading == (STAT)type)
            StartCoroutine(StatUp());
        else
        {
            statUpgrading = (STAT)type;
            statUpgradeBtnTxt[type].text = GetRequiredStat((STAT)type).ToString();
            isStatUpgrading = true;
            for(int i = 0; i < statUpgradeBtn.Length;i++)
            {
                if (i == type)
                    continue;
                statUpgradeBtn[i].interactable = true;
                statUpgradeBtnTxt[i].text = "Up";
            }
        }
    }
    /// <summary>
    /// Soul 상점 패널을 열거나 닫는 함수
    /// 외부에서 접근 가능함
    /// </summary>
    /// <param name="value"></param>
    public void SetSoulShop(bool value)
    {
        if (value)
        {
            GameManager.instance.SetPause(true);
            shopPnl.SetActive(true);
            StatPnlOpen();
            GameManager.instance.moneyManager.Load();
        }
        else
        {
            shopPnl.SetActive(false);
            ShopStage(SHOPSTAGE.DECREASE);
        }
    }

    /// <summary>
    /// 스탯 패널만 여는 함수
    /// </summary>
    private void StatPnlOpen()
    {
        SyncStatUpgradeTxt();
        statPnl.SetActive(true);
        SyncStatUpgradeTxt();
        //스탯 외 다른 패널은 닫아야함
    }

    /// <summary>
    /// 업그레이드 선택 패널 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetCheckPnl(bool value)
    {
        checkPnl.SetActive(value);
        if (value)
        {
            cancelBtn.interactable = true;
            okBtn.interactable = true;
        }
    }

    /// <summary>
    /// Ok와 Cancel 중에 선택하는 함수
    /// UGUI에서 호출, 해당 코루틴을 호출함
    /// </summary>
    /// <param name="value"></param>
    public void Select(bool value)
    {
        selected = value;
        StartCoroutine(currentCoroutine);
    }

    /// <summary>
    /// 스탯 업그레이드 값 구하는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private int GetRequiredStat(STAT type)
    {
        return ((int)GameManager.GetStat(type, true) - 4) * 10;
    }

    /// <summary>
    /// 소울 업그레이드의 도메인
    /// </summary>
    [SerializeField]
    public enum SOULCOROUTINE
    { STAT }

    /// <summary>
    /// 무엇을 업그레이드 할 지 선택하는 함수
    /// UGUI에서 호출
    /// </summary>
    /// <param name="soulCoroutine"></param>
    public void SelectCoroutine(int soulCoroutine)
    {
        switch((SOULCOROUTINE)soulCoroutine)
        {
            case SOULCOROUTINE.STAT:
                currentCoroutine = StatUp();
                checkTxt.text = GetRequiredStat(statUpgrading) + " Soul이 필요합니다.\n업그레이드 하시겠습니까?";
                break;
        }
    }

    /// <summary>
    /// okBtn을 누르면 selected 가 참이됨upgrading의 스탯이 업그레이드 됨
    /// 만약 MaxStatUpCheck에서 false를 받으면 알려주고 checkPnl을 닫음
    /// </summary>
    public IEnumerator StatUp()
    {
        int required = GetRequiredStat(statUpgrading);
        isUpgrading = true;
        if (MoneyManager.instance.soul >= required)
        {
            Debug.Log("Max Stat Upgraded");
            GameManager.AddStat(statUpgrading, 1, true);
            MoneyManager.instance.AddSoul(-required);
            MoneyManager.instance.Save();
            SyncStatUpgradeTxt();
            statUpgradeBtnTxt[(int)statUpgrading].text = "Done";
            isStatUpgrading = false;
        }
        else
        {
            Debug.Log("Not Enough Soul");
            statUpgradeBtnTxt[(int)statUpgrading].text = "Fail";
            isStatUpgrading = false;
        }
        //버튼 잠금
        //cancelBtn.interactable = false;
        //okBtn.interactable = false;
        for (int i = 0; i < statUpgradeBtn.Length; i++)
        {
            statUpgradeBtn[i].interactable = false;
        }
#if DELAY
            yield return GameManager.instance.delayOneReal;
#else
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
#endif
        //SetCheckPnl(false);
        //cancelBtn.interactable = true;
        //okBtn.interactable = true;
        for (int i = 0; i < statUpgradeBtn.Length; i++)
        {
            statUpgradeBtn[i].interactable = true;
        }
        statUpgradeBtnTxt[(int)statUpgrading].text = "Up";
        isUpgrading = false;
        SyncStatUpgradeTxt();
        //버튼 잠금 해제
    }


    /// <summary>
    /// 맥스 스탯 값 업데이트
    /// </summary>
    private void SyncStatUpgradeTxt()
    {
        for (int i = 0; i < statUpgradeTxt.Length; i++)
            statUpgradeTxt[i].text = GameManager.GetStat((STAT)i, true).ToString();
    }
}
