using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using RogueNaraka.ScrollScripts;
using RogueNaraka.UnitScripts;

public class SoulShopManager : MonoBehaviour
{
    public GameObject shopPnl;
    public GameObject statPnl;
    public GameObject skillPnl;
    public GameObject soulPnl;
    public GameObject preparingPnl;

    public Button[] menuBtns;

    public Button statBtn;

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

    string lastMusic = string.Empty;

    /// <summary>
    /// Soul 상점 패널을 열거나 닫는 함수
    /// 외부에서 접근 가능함
    /// </summary>
    /// <param name="value"></param>
    public void SetSoulShop(bool value)
    {
        if (value)
        {
            shopPnl.SetActive(true);
            
            GameManager.instance.moneyManager.Load();

            lastMusic = AudioManager.instance.music.clip.name;
            AudioManager.instance.PlayMusic("peace4");
            if (DeathManager.instance.deathPnl.gameObject.activeSelf)
            {
                menuBtns[0].onClick.Invoke();
                TutorialManager.instance.StartTutorial(3);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    menuBtns[i].interactable = false;
                }
                SoulPnlOpen();
            }
            
        }
        else
        {
            shopPnl.SetActive(false);

            AudioManager.instance.PlayMusic(lastMusic);
            if(DeathManager.instance.deathPnl.gameObject.activeSelf)
                DeathManager.instance.pauseBtn.gameObject.SetActive(false);
            else
                DeathManager.instance.pauseBtn.gameObject.SetActive(true);
            if (LevelUpManager.instance.statPnl.activeSelf)
                LevelUpManager.instance.SyncStatUpgradeTxt();
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
        soulPnl.SetActive(false);

        statPnl.SetActive(true);
        SyncStatUpgradeTxt();
        preparingPnl.SetActive(false);
    }

    public void SkillPnlOpen()
    {
        statPnl.SetActive(false);
        weaponPnl.SetActive(false);
        soulPnl.SetActive(false);

        InitSkillPnl();
        skillPnl.SetActive(true);
        preparingPnl.SetActive(true);
    }

    public void WeaponPnlOpen()
    {
        statPnl.SetActive(false);
        skillPnl.SetActive(false);
        soulPnl.SetActive(false);

        weaponPnl.SetActive(true);

        WeaponPnlUpdate(PlayerPrefs.GetInt("exp"));
        preparingPnl.SetActive(false);
    }

    public void SoulPnlOpen()
    {
        statPnl.SetActive(false);
        skillPnl.SetActive(false);
        weaponPnl.SetActive(false);

        soulPnl.SetActive(true);

        preparingPnl.SetActive(false);
        RefiningRateTxtNameUpdate();
        RefiningRateTxtUpdate();

        TutorialManager.instance.StartTutorial(4);
    }

    #region stat

    public TextMeshProUGUI[] statUpgradeTxt;
    public TextMeshProUGUI[] statUpgradeBtnTxt;
    public Button[] statUpgradeBtn;

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

    public SkillScrollView skillScrollView;

    void InitSkillPnl()
    {
        var cellData = GetUnBoughtSkills();
        if (cellData.Count > 0)
        {
            skillScrollView.gameObject.SetActive(true);
            skillScrollView.UpdateData(cellData);
        }
        else
            skillScrollView.gameObject.SetActive(false);
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
            Debug.Log("bought " + skill.GetName() + " " + skill.cost + " soul");
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

    public GameObject weaponPnl;
    public TextMeshProUGUI weaponSoulTxt;
    public TextMeshProUGUI weaponLevelTxt;
    public Image weaponBar;
    public Button weaponExpBtn;
    int weapon = -1;
    int _weapon = -1;

    public void StartAddWeaponExp()
    {
        StartCoroutine("AddWeaponExpCorou");
    }

    void WeaponPnlUpdate(int exp)
    {
        int remain;
        PlayerWeaponData data = GameManager.instance.GetPlayerWeapon(exp, out remain);
        if(data.level == GameDatabase.instance.playerWeapons.Length - 1 && remain == -1)
        {
            weaponSoulTxt.text = string.Format("{0}/{1} Soul", data.cost, data.cost); ;
            weaponBar.fillAmount = 1;
            weaponExpBtn.interactable = false;
            StopCoroutine("AddWeaponExpCorou");
        }
        else
        {
            weaponSoulTxt.text = string.Format("{0}/{1} Soul", remain, data.cost);
            weaponBar.fillAmount = (float)remain / data.cost;
            weaponExpBtn.interactable = true;
        }
        
        weaponLevelTxt.text = string.Format("{0} Level", data.level);

        weapon = data.id;
        if (weapon != _weapon)
        {
            _weapon = weapon;
            Unit player = BoardManager.instance.player;
            if (player)
            {
                player.data.weapon = data.id;
                player.attackable.Init((WeaponData)GameDatabase.instance.weapons[data.id].Clone());
            }
        }
    }

    IEnumerator AddWeaponExpCorou()
    {
        float delay = 0.5f;
        int exp = PlayerPrefs.GetInt("exp");
        while (MoneyManager.instance.UseSoul(1))
        {
            float t = delay;
            exp = exp + 1;
            PlayerPrefs.SetInt("exp", exp);
            AudioManager.instance.PlaySFX("weaponUpgrade");

            WeaponPnlUpdate(exp);

            while (t > 0)
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            }
            if(delay > 0.05f)
                delay *= 0.9f;
        }
    }
    #endregion

    #region soul

    public TextMeshProUGUI soulRefRateNameTxt;
    public TextMeshProUGUI soulRefRateValueTxt;
    public TextMeshProUGUI soulRefRateBtnTxt;
    public Button soulRefRateBtn;
    public void RefiningRateUpgrade()
    {
        float rate = MoneyManager.instance.refiningRate;
        if (soulRefRateBtnTxt.text.CompareTo("Up") == 0)
        {
            soulRefRateBtnTxt.text = string.Format("{0}Soul", (int)(rate * 100));
        }
        else if (MoneyManager.instance.UseSoul((int)(rate * 100)))
        {
            MoneyManager.instance.refiningRate = rate + 0.01f;
            RefiningRateTxtUpdate();
            soulRefRateBtnTxt.text = "Done";
            StartCoroutine(LockSoulRefRateBtn());
        }
        else
        {
            soulRefRateBtnTxt.text = "Fail";
            StartCoroutine(LockSoulRefRateBtn());
        }
    }

    IEnumerator LockSoulRefRateBtn()
    {
        soulRefRateBtn.interactable = false;
        float t = 1;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        soulRefRateBtn.interactable = true;
        soulRefRateBtnTxt.text = "Up";
    }

    public void RefiningRateTxtUpdate()
    {
        soulRefRateValueTxt.text = string.Format("{0}%~100%", MoneyManager.instance.refiningRate * 100);
    }
    public void RefiningRateTxtNameUpdate()
    {
        switch (GameManager.language)
        {
            case Language.English:
                soulRefRateNameTxt.text = "Soul refining rate";
                break;
            case Language.Korean:
                soulRefRateNameTxt.text = "정제율";
                break;
        }
    }

    #endregion
}
