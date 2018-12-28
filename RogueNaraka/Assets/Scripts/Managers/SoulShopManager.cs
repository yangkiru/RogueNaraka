using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using RogueNaraka.ScrollScripts;

public class SoulShopManager : MonoBehaviour
{
    public GameObject shopPnl;
    public GameObject statPnl;
    public GameObject skillPnl;
    public GameObject weaponPnl;
    public SkillScrollView skillScrollView;

    public Button[] statUpgradeBtn;

    public TextMeshProUGUI[] statUpgradeTxt;
    public TextMeshProUGUI[] statUpgradeBtnTxt;
    public int shopStage
    { get { return _shopStage; } }
    [SerializeField]
    private int _shopStage;

    private STAT statUpgrading;

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
    public void StatPnlOpen()
    {
        SyncStatUpgradeTxt();
        skillPnl.SetActive(false);
        weaponPnl.SetActive(false);

        statPnl.SetActive(true);
        SyncStatUpgradeTxt();
    }

    public void SkillPnlOpen()
    {
        statPnl.SetActive(false);
        weaponPnl.SetActive(false);

        InitSkillPnl();
        skillPnl.SetActive(true);
    }

    public void WeaponPnlOpen()
    {
        statPnl.SetActive(false);
        skillPnl.SetActive(false);

        weaponPnl.SetActive(true);
    }

    #region stat

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
            statUpgradeBtnTxt[type].text = GetRequiredSoul((STAT)type).ToString();
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
    /// 스탯 업그레이드 값 구하는 함수
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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
            case STAT.TEC:
                target = stat.tecMax;
                break;
            case STAT.HP:
                target = stat.hpMax;
                break;
            case STAT.MP:
                target = stat.mpMax;
                break;
            case STAT.HPREGEN:
                target = stat.hpRegenMax;
                break;
            case STAT.MPREGEN:
                target = stat.mpRegenMax;
                break;
            case STAT.STATPOINT:
                target = stat.statPoints;
                break;
        }

        return (int)((target - 4) * 10);
    }

    public IEnumerator StatUp()
    {
        int required = GetRequiredSoul(statUpgrading);
        if (MoneyManager.instance.soul >= required)
        {
            Debug.Log("Max Stat Upgraded");

            Stat stat = BoardManager.instance.player.stat;
            stat.AddMax(statUpgrading, 1);
            PlayerPrefs.SetString("stat", Stat.StatToJson(stat));
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
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        //SetCheckPnl(false);
        //cancelBtn.interactable = true;
        //okBtn.interactable = true;
        for (int i = 0; i < statUpgradeBtn.Length; i++)
        {
            statUpgradeBtn[i].interactable = true;
        }
        statUpgradeBtnTxt[(int)statUpgrading].text = "Up";
        SyncStatUpgradeTxt();
        //버튼 잠금 해제
    }


    /// <summary>
    /// 맥스 스탯 값 업데이트
    /// </summary>
    private void SyncStatUpgradeTxt()
    {
        Stat stat = (Stat)Stat.JsonToStat(PlayerPrefs.GetString("stat")).Clone();

        for (int i = 0; i < statUpgradeTxt.Length; i++)
            statUpgradeTxt[i].text = stat.GetMax((STAT)i).ToString();
    }

    #endregion
    #region skill

    void InitSkillPnl()
    {
        var cellData = GetUnBoughtSkills();
        skillScrollView.UpdateData(cellData);
    }

    public List<SkillData> GetUnBoughtSkills()
    {
        List<SkillData> list = new List<SkillData>();

        bool[] boughts = SkillData.GetBoughtSkills();
        for (int i = 0; i < GameDatabase.instance.skills.Length; i++)
        {
            if (!boughts[i])
                list.Add((SkillData)GameDatabase.instance.skills[i].Clone());
        }
        return list;
    }

    public void BuySkill(Button btn, TextMeshProUGUI txt, int id)
    {
        SkillData skill = GameDatabase.instance.skills[id];
        if (txt.text.CompareTo("Buy") == 0)
        {
            Debug.Log("Buying skill " + skill.GetName() + skill.cost + " soul");
            txt.text = skill.cost.ToString();
        }
        else
        {
            Debug.Log(skill.GetName() + "bought " + skill.cost + " soul");
            if (MoneyManager.instance.UseSoul(skill.cost))
            {
                SkillData.Buy(skill.id);
                txt.text = "Done";
            }
            else
                txt.text = "Fail";
            StartCoroutine(BtnCorou(btn, txt));
        }
    }

    IEnumerator BtnCorou(Button btn, TextMeshProUGUI txt)
    {
        btn.interactable = false;
        float t = 0;
        while(t < 1)
        {
            yield return null;
            t += Time.unscaledDeltaTime;
        }

        if (txt.text.CompareTo("Done") == 0)
            InitSkillPnl();

        btn.interactable = true;
        txt.text = "Buy";
    }

    #endregion

    #region weapon

    #endregion
}
