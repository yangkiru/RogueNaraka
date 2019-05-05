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
    public void SetSoulShop(bool value, int menu = 0)
    {
        if (value)
        {
            shopPnl.SetActive(true);
            
            GameManager.instance.moneyManager.Load();

            //AudioManager.
            AudioManager.instance.PlayMusic("cave");
            
            menuBtns[menu].onClick.Invoke();

            GameManager.instance.settingBtn.SetActive(false);
            
        }
        else
        {
            shopPnl.SetActive(false);

            if(DeathManager.instance.deathPnl.gameObject.activeSelf || StatOrbManager.instance.pnl.activeSelf)
                GameManager.instance.SetSettingBtn(true);
            else
                GameManager.instance.SetPauseBtn(true);
            if (StatManager.instance.statPnl.activeSelf)
                StatManager.instance.SyncStatUpgradeTxt();
            if(DeathManager.instance.deathPnl.gameObject.activeSelf)
                AudioManager.instance.PlayMusic(AudioManager.instance.currentDeathMusic);
            else
                AudioManager.instance.PlayMusic(AudioManager.instance.currentMainMusic);
        }
    }

    public void SetSoulShop(bool value)
    {
        SetSoulShop(value, 0);
    }

    public void OpenSoulShop(int menu)
    {
        SetSoulShop(true, menu);
    }

    /// <summary>
    /// 스탯 패널만 여는 함수
    /// </summary>
    public void StatPnlOpen()
    {
        shopPnl.SetActive(true);
        SyncStatUpgradeTxt();
        skillPnl.SetActive(false);
        weaponPnl.SetActive(false);
        soulPnl.SetActive(false);

        statPnl.SetActive(true);
        SyncStatUpgradeTxt();
        preparingPnl.SetActive(false);
        TutorialManager.instance.StartTutorial(3);
    }

    public void SkillPnlOpen()
    {
        shopPnl.SetActive(true);
        statPnl.SetActive(false);
        weaponPnl.SetActive(false);
        soulPnl.SetActive(false);

        InitSkillPnl();
        skillPnl.SetActive(true);
        preparingPnl.SetActive(true);
    }

    public void WeaponPnlOpen()
    {
        shopPnl.SetActive(true);
        statPnl.SetActive(false);
        skillPnl.SetActive(false);
        soulPnl.SetActive(false);

        weaponPnl.SetActive(true);

        WeaponPnlUpdate(PlayerPrefs.GetInt("exp"));
        preparingPnl.SetActive(false);

        weaponId = -1;
        _weaponId = -1;
    }

    public void SoulPnlOpen()
    {
        shopPnl.SetActive(true);
        statPnl.SetActive(false);
        skillPnl.SetActive(false);
        weaponPnl.SetActive(false);

        soulPnl.SetActive(true);

        preparingPnl.SetActive(false);
        RefiningRateTxtNameUpdate();
        RefiningRateTxtUpdate();
        ADLanguageUpdate();

        //정제율 제한
        if (MoneyManager.instance.refiningRate >= 0.7f)
        {
            soulRefRateBtnTxt.text = "Max";
            soulRefRateBtn.interactable = false;
        }
        else
        {
            soulRefRateBtnTxt.text = "Up";
            soulRefRateBtn.interactable = true;
        }
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
            case STAT.HR:
                target = stat.hpRegenMax;
                break;
            case STAT.MR:
                target = stat.mpRegenMax;
                break;
            case STAT.SP:
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
    public TextMeshProUGUI weaponDPSTxt;
    public TextMeshProUGUI weaponRangeTxt;
    public TextMeshProUGUI weaponDescriptionTxt;
    public Image weaponBar;
    public Button weaponExpBtn;
    public GameObject weaponLevelUpBanner;
    int weaponId = -1;
    int _weaponId = -1;

    public void StartAddWeaponExp()
    {
        StartCoroutine("AddWeaponExpCorou");
    }

    int WeaponPnlUpdate(int exp)
    {
        int remain;
        PlayerWeaponData data = GameManager.instance.GetPlayerWeapon(exp, out remain);
        if(data.level == GameDatabase.instance.playerWeapons.Length-1)
        {
            weaponSoulTxt.text = string.Format("{0}/{1} Soul", data.cost, data.cost); ;
            weaponBar.fillAmount = 1;
            weaponExpBtn.interactable = false;
            Debug.Log("Max Weapon");
            StopCoroutine("AddWeaponExpCorou");
        }
        else
        {
            weaponSoulTxt.text = string.Format("{0}/{1} Soul", remain, data.cost);
            weaponBar.fillAmount = (float)remain / data.cost;
            weaponExpBtn.interactable = true;
        }
        
        weaponLevelTxt.text = string.Format("{0} Level", data.level);

        weaponId = data.id;
        if (weaponId != _weaponId)
        {
            _weaponId = weaponId;
            Unit player = BoardManager.instance.player;
            WeaponData weapon = GameDatabase.instance.weapons[weaponId];
            if (player)
            {
                player.data.weapon = data.id;
                player.attackable.Init((WeaponData)GameDatabase.instance.weapons[data.id].Clone());
                string translatedDPS = string.Empty;
                string translatedRange = string.Empty;
                switch(GameManager.language)
                {
                    default:
                        translatedDPS = "DPS";
                        translatedRange = "Range";
                        break;
                    case Language.Korean:
                        translatedDPS = "초당피해량";
                        translatedRange = "사거리";
                        break;
                }
                weaponDPSTxt.text = string.Format("{0}-{1}", translatedDPS, GetWeaponDPS(weapon).ToString("##0.##"));
                weaponRangeTxt.text = string.Format("{0}-{1}", translatedRange, weapon.attackDistance.ToString("##0.##"));
                
                weaponDescriptionTxt.text = GetWeaponDescription(data);
            }
        }
        return data.level;
    }

    string GetWeaponDescription(PlayerWeaponData data)
    {
        string description;
        if (data.description.Length > (int)GameManager.language)
            description = data.description[(int)GameManager.language];
        else if (data.description.Length > 1)
            description = data.description[0];
        else
            description = string.Empty;
        return description;
    }

    float GetWeaponDPS(WeaponData data)
    {
        float DPS = GetBulletDPS(GameDatabase.instance.bullets[data.startBulletId]);
        for (int i = 0; i < data.children.Length; i++)
            DPS += GetBulletDPS(GameDatabase.instance.bullets[data.children[i].bulletId]);
        return DPS;
    }

    float GetBulletDPS(BulletData data)
    {
        float DPS = 0;
        if (data.pierce == 1)
            DPS = data.dmg;
        else
            DPS = 1 / Mathf.Max(data.delay, 0.01f) * data.dmg;
        for (int i = 0; i < data.children.Length; i++)
            DPS += GetBulletDPS(GameDatabase.instance.bullets[data.children[i].bulletId]);
        return DPS;
    }

    int GetWeaponLevel(int exp, out int remain)
    {
        remain = 0;
        for (int i = 0; i < GameDatabase.instance.playerWeapons.Length; i++)
        {
            exp -= GameDatabase.instance.playerWeapons[i].cost;
            if (exp < 0)
            {
                remain = GameDatabase.instance.playerWeapons[i].cost + exp;
                return GameDatabase.instance.playerWeapons[i].level;
            }
        }
        return -1;
    }

    IEnumerator AddWeaponExpCorou()
    {
        int exp = PlayerPrefs.GetInt("exp");
        int speedUp = 0;
        int remain;
        int nextRemain;
        int currentLevel = GetWeaponLevel(exp, out remain);
        int lastLevel = currentLevel;
        int amount = 1 + currentLevel;
        float delay = 0.1f;

        bool isSuccess;
        do
        {
            float t = delay;
            int nextExp = exp + amount;
            int nextLevel = GetWeaponLevel(nextExp, out nextRemain);
            if(nextLevel != currentLevel)//무기 레벨 업 전
            {
                Debug.Log("amount:" + amount + " remain:" + remain);
                amount = GameDatabase.instance.playerWeapons[currentLevel].cost - remain;
                Debug.Log("amount:" + amount);
            }
            remain = nextRemain;

            isSuccess = MoneyManager.instance.UseSoul(amount);

            if (!isSuccess)
                yield break;

            exp += amount;
            PlayerPrefs.SetInt("exp", exp);
            AudioManager.instance.PlaySFX("weaponUpgrade");

            currentLevel = WeaponPnlUpdate(exp);
            if(currentLevel != lastLevel)//무기 레벨 업
            {
                weaponLevelUpBanner.SetActive(true);
                yield break;
            }
            lastLevel = currentLevel;
            while (t > 0)
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            }
            if (delay > 0.05f)
                delay *= 0.9f;
            else
            {
                if (++speedUp > 10)
                {
                    speedUp = 0;
                    amount += currentLevel + 1;
                }
            }
        } while (isSuccess);
    }
    #endregion

    #region soul

    public TextMeshProUGUI soulRefRateNameTxt;
    public TextMeshProUGUI soulRefRateValueTxt;
    public TextMeshProUGUI soulRefRateBtnTxt;

    public TextMeshProUGUI adNameTxt;
    public TextMeshProUGUI adValueTxt;
    public Button soulRefRateBtn;
    public void RefiningRateUpgrade()
    {
        float rate = MoneyManager.instance.refiningRate;
        rate = Mathf.Round(rate * 100) * 0.01f;
        int amount = GetRefiningRateCost(rate);
        Debug.Log("rate:" + rate + " amount:" + amount);
        if (soulRefRateBtnTxt.text.CompareTo("Up") == 0)
        {
            soulRefRateBtnTxt.text = string.Format("{0}", amount);
        }
        else if (MoneyManager.instance.UseSoul(amount))
        {
            float r = rate + 0.01f;
            r = Mathf.Round(r * 100) * 0.01f;
            MoneyManager.instance.refiningRate = r;
            RefiningRateTxtUpdate();
            soulRefRateBtnTxt.text = "Done";
            StartCoroutine(LockSoulRefRateBtn());
        }
        else
        {
            soulRefRateBtnTxt.text = "Fail";
            StartCoroutine(LockSoulRefRateBtn());
        }

        if (MoneyManager.instance.refiningRate >= 0.7f)
        {
            soulRefRateBtnTxt.text = "Max";
            soulRefRateBtn.interactable = false;
        }
    }

    /// <summary>
    /// 소울 정제율 비용을 구하는 함수
    /// </summary>
    /// <param name="rate"></param>
    /// <returns></returns>
    public int GetRefiningRateCost(float rate)
    {
        if (rate <= 0.3f)
            return 100;
        int delta = 0;
        if (rate <= 0.35f)
            delta = 10;
        else if (rate <= 0.4f)
            delta = 50;
        else if (rate <= 0.45f)
            delta = 100;
        else if (rate <= 0.5f)
            delta = 500;
        else if (rate <= 0.55f)
            delta = 1000;
        else if (rate <= 0.6f)
            delta = 5000;
        else if (rate <= 0.65f)
            delta = 10000;
        else if (rate <= 0.7f)
            delta = 20000;

        rate = Mathf.Round((rate - 0.01f) * 100) * 0.01f;
        return GetRefiningRateCost(rate) + delta;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        //for (float rate = 0.3f; rate + 0.01f <= 0.7f; rate += 0.01f)
        //{
        //    int result = GetRefiningRateCost(rate);
        //    Debug.Log(rate + " " + result);
        //}
        PlayerPrefs.SetFloat("exp", 0);
    }

    /// <summary>
    /// 소울 정제율 초기화
    /// </summary>
    public void ResetRefiningRateUpgrade()
    {
        if (PlayerPrefs.GetInt("isRefiningRateReset") == 1)
            return;

        float rate = MoneyManager.instance.refiningRate;
        int amount = (int)(Mathf.Round(rate * 1000));
        int result = 0;
        for(int i = 300; i < amount; i += 10)
        {
            result += i;
        }

        Debug.Log("rate:" + rate + " result:" + result);

        MoneyManager.instance.AddSoul(result);
        MoneyManager.instance.refiningRate = 0.3f;
        PlayerPrefs.SetInt("isRefiningRateReset", 1);
        
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

        if (MoneyManager.instance.refiningRate < 0.7f)
        {
            soulRefRateBtn.interactable = true;
            soulRefRateBtnTxt.text = "Up";
        }
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
                soulRefRateNameTxt.text = "<size=18>Refining rate</size>";
                break;
            case Language.Korean:
                soulRefRateNameTxt.text = "정제율";
                break;
        }
    }

    public void ADLanguageUpdate()
    {
        switch (GameManager.language)
        {
            case Language.English:
                adNameTxt.text = "Refine";
                adValueTxt.text = "Watch Video";
                break;
            case Language.Korean:
                adNameTxt.text = "즉시 정제";
                adValueTxt.text = "광고 시청";
                break;
        }
    }

    #endregion
}
