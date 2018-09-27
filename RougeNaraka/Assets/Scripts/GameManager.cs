using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {
    [SerializeField][ReadOnly]
    public static GameManager instance = null;

    public BoardManager boardManager;
    public MoneyManager moneyManager;
    public Player player;

    public GameObject selectPnl;
    public GameObject statPnl;
    public GameObject soulShopPnl;
    public GameObject soulStatPnl;
    public GameObject soulStatCheckPnl;

    
    public Button cancelBtn;//stat Upgrade
    public Button soulStatOkBtn;
    public Button soulStatCancelBtn;

    public Text soulStatCheckTxt;
    public Text soulShopSoulTxt;

    public Text[] statTxt;
    public Text[] upgradeTxt;
    public Text[] maxUpgradeTxt;

    //Debug params
    public int weaponId;
    public int weaponLevel;
    private int _weaponId = 0;
    private int _weaponLevel = 0;
    public int stage;

    public bool isDebug;
    public bool isReset;
    public bool levelUp;
    public bool isRun;
    public bool isStage;
    public bool spawnEffect;
    public bool autoSave = true;
    public bool removeFirst;

    public float autoSaveTime = 1;

    public bool isPause
    { get { return _isPause; } }
    [SerializeField][ReadOnly]
    private bool _isPause;
    private bool isUpgraded;
    private bool isLevelUp;
    private bool isBuy;

    private IEnumerator autoSaveCoroutine;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
            Destroy(gameObject);

        //if (isDebug)
        //{
        //    if (boardManager != null)
        //        boardManager.InitBoard();
        //}
        if(isReset)
            PlayerPrefs.SetInt("isFirst", 0);

        if(isRun)
            PlayerPrefs.SetInt("isRun", 1);

        Load();
    }

    private void Update()
    {
        //test
        if (levelUp)
        {
            levelUp = false;
            LevelUp();
        }
        if(autoSave)
        {
            if(autoSaveCoroutine == null)
            {
                autoSaveCoroutine = AutoSave(autoSaveTime);
                StartCoroutine(autoSaveCoroutine);
            }
        }
        else
        {
            if(autoSaveCoroutine != null)
            {
                Debug.Log("Stop AutoSave");
                StopCoroutine(autoSaveCoroutine);
                autoSaveCoroutine = null;
            }
        }

        if(weaponId != _weaponId || weaponLevel != _weaponLevel)
        {
            _weaponId = weaponId;
            _weaponLevel = weaponLevel;
            player.EquipWeapon(weaponId, weaponLevel);
        }
    }

    public void RunGame()
    {
        isRun = true;
        PlayerPrefs.SetInt("isRun", 1);
        SoulShopClose();
        Load();
        RandomStat();
        PlayerPrefs.SetFloat("health", player.stat.hp);
        PlayerPrefs.SetFloat("mana", player.stat.mp);
        Save();
        StatTextUpdate();
    }

    private void RandomStat()
    {
        int statPoint = PlayerPrefs.GetInt("statPoint");
        while(statPoint > 0)
        {
            int type = Random.Range(0, (int)STAT.MPREGEN + 1);
            if(player.AddStat((STAT)type, 1))
            {
                statPoint--;
            }
            else
            {
                int current = (int)player.stat.sum;
                int max = (int)player.maxStat.sum;
                if (current == max)
                    return;
                //Maxed
            }
        }
    }

    public IEnumerator AutoSave(float time)
    {
        while (true)
        {
            Save();
            yield return new WaitForSecondsRealtime(time);
        }
    }

    public void Save()
    {
        moneyManager.Save();

        EffectData[] effectDatas = new EffectData[player.effects.Count];
        for (int i = 0; i < effectDatas.Length; i++)
        {
            effectDatas[i] = player.effects[i].data;
        }
        PlayerPrefs.SetString("effect", JsonHelper.ToJson<EffectData>(effectDatas));

        if(isRun)
        {
            PlayerPrefs.SetFloat("dmg", player.stat.dmg);
            PlayerPrefs.SetFloat("spd", player.stat.spd);
            PlayerPrefs.SetFloat("luck", player.stat.luck);
            PlayerPrefs.SetFloat("hp", player.stat.hp);
            PlayerPrefs.SetFloat("mp", player.stat.mp);
            PlayerPrefs.SetFloat("hpRegen", player.stat.hpRegen);
            PlayerPrefs.SetFloat("mpRegen", player.stat.mpRegen);
            PlayerPrefs.SetFloat("health", player.health);
            PlayerPrefs.SetFloat("mana", player.mana);
            PlayerPrefs.SetInt("stage", boardManager.stage);
            SkillManager.instance.Save();
            Item.instance.Save();
            PlayerPrefs.SetString("weapon", JsonUtility.ToJson(player.weapon));
        }
        if(isLevelUp)
            PlayerPrefs.SetInt("isLevelUp", 1);
        else
            PlayerPrefs.SetInt("isLevelUp", 0);
        //Debug.Log("Saved");
    }

    public void Load()
    {
        if (PlayerPrefs.GetInt("isFirst") == 0)//reset
        {
            Debug.Log("First Run");
            moneyManager.SetGold(0);
            moneyManager.Save();
            PlayerPrefs.SetFloat("isLevelUp", 0);
            Stat dbStat = GameDatabase.instance.playerBase;
            SyncStatToData(dbStat);

            Stat maxStat = new Stat(5);

            PlayerPrefs.SetFloat("dmgMax", maxStat.dmg);
            PlayerPrefs.SetFloat("spdMax", maxStat.spd);
            PlayerPrefs.SetFloat("luckMax", maxStat.luck);
            PlayerPrefs.SetFloat("hpMax", maxStat.hp);
            PlayerPrefs.SetFloat("mpMax", maxStat.mp);
            PlayerPrefs.SetFloat("hpRegenMax", maxStat.hpRegen);
            PlayerPrefs.SetFloat("mpRegenMax", maxStat.mpRegen);

            PlayerPrefs.SetInt("isFirst", 1);
            PlayerPrefs.SetString("effect", string.Empty);
            PlayerPrefs.SetInt("statPoint", 5);
            PlayerPrefs.SetInt("isRun", 0);
            PlayerPrefs.SetInt("isLevelUp", 0);
            Weapon weapon = new Weapon(GameDatabase.instance.weapons[weaponId]);//weapon save data로 변경 필요
            weapon.level = weaponLevel;
            PlayerPrefs.SetString("weapon", JsonUtility.ToJson(weapon));

            PlayerPrefs.SetInt("stage", 1);

            SkillManager.instance.ResetSave();
            Item.instance.ResetSave();
        }

        //Debug.Log("Load");

        if (PlayerPrefs.GetInt("isRun") == 1)
        {
            Debug.Log("Open Run");
            isRun = true;
            moneyManager.Load();
            Stat stat = new Stat(PlayerPrefs.GetFloat("dmg"), PlayerPrefs.GetFloat("spd"),
            PlayerPrefs.GetFloat("luck"), PlayerPrefs.GetFloat("hp"), PlayerPrefs.GetFloat("mp"),
            PlayerPrefs.GetFloat("hpRegen"), PlayerPrefs.GetFloat("mpRegen"));

            player.SyncData(stat, true, false);
            player.SetHealth(PlayerPrefs.GetFloat("health"));
            player.SetMana(PlayerPrefs.GetFloat("mana"));

            StatTextUpdate();

            player.SetMaxStat(new Stat(GetMaxStat((STAT)0), GetMaxStat((STAT)1), GetMaxStat((STAT)2),
                GetMaxStat((STAT)3), GetMaxStat((STAT)4), GetMaxStat((STAT)5), GetMaxStat((STAT)6)));

            player.EquipWeapon(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString("weapon")));

            if (PlayerPrefs.GetString("effect") != string.Empty)
            {
                EffectData[] temp = JsonHelper.FromJson<EffectData>(PlayerPrefs.GetString("effect"));
                for (int i = 0; i < temp.Length; i++)
                {
                    player.AddEffect(temp[i].type, temp[i].value, temp[i].time, temp[i].isInfinity);
                }
            }
            if (autoSave)
            {
                autoSaveCoroutine = AutoSave(autoSaveTime);
                StartCoroutine(autoSaveCoroutine);
            }
            boardManager.SetStage(PlayerPrefs.GetInt("stage"));
            if (isStage)//디버깅용
                boardManager.SetStage(stage);

            SkillManager.instance.Load();
            Item.instance.Load();

            if (PlayerPrefs.GetInt("isLevelUp") == 1)
            {
                LevelUp();
                SetPause(true);
            }
            else
            {
                boardManager.InitBoard();
            }
        }
        else//Open Shop, Init Run
        {
            Debug.Log("Open Shop");
            SetPause(true);
            moneyManager.Load();
            Stat dbStat = GameDatabase.instance.playerBase;
            SyncStatToData(dbStat);
            PlayerPrefs.SetInt("stage", 1);
            PlayerPrefs.SetFloat("health", 1);
            PlayerPrefs.SetFloat("mana", 1);
            SoulShopOpen();
            SkillManager.instance.ResetSave();
            Item.instance.ResetSave();
        }
    }

    public void SyncStatToData(Stat stat)
    {
        stat = GameDatabase.instance.playerBase;
        PlayerPrefs.SetFloat("dmg", stat.dmg);
        PlayerPrefs.SetFloat("spd", stat.spd);
        PlayerPrefs.SetFloat("luck", stat.luck);
        PlayerPrefs.SetFloat("hp", stat.hp);
        PlayerPrefs.SetFloat("mp", stat.mp);
        PlayerPrefs.SetFloat("hpRegen", stat.hpRegen);
        PlayerPrefs.SetFloat("mpRegen", stat.mpRegen);
    }

    public void StatTextUpdate()
    {
        statTxt[0].text = player.stat.dmg.ToString();
        statTxt[1].text = player.stat.spd.ToString();
        statTxt[2].text = player.stat.luck.ToString();
        statTxt[3].text = player.stat.hp.ToString();
        statTxt[4].text = player.stat.mp.ToString();
        statTxt[5].text = string.Format("{0} / {1}", player.stat.hpRegen.ToString(), player.stat.mpRegen.ToString());
    }

    public void SetPause(bool value)
    {
        _isPause = value;
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void SyncSoulShopSoulTxt()
    {
        soulShopSoulTxt.text = moneyManager.soul.ToString();
    }

    public void SoulShopOpen()
    {
        soulShopPnl.SetActive(true);
        soulStatPnl.SetActive(true);
        SoulStatOpen();
        moneyManager.Load();
        SyncSoulShopSoulTxt();
    }

    public void SoulStatOpen()
    {
        SyncMaxStatUpgradeTxt();
        soulStatPnl.SetActive(true);
        //스탯 외 다른 패널은 닫아야함
    }
    public void SoulShopClose()
    {
        soulShopPnl.SetActive(false);
    }

    private STAT upgrading;

    /// <summary>
    /// 업그레이드 버튼을 누르면 upgrading에 값이 전달됨
    /// </summary>
    /// <param name="type"></param>
    public void SelectUpgrade(int type)
    {
        upgrading = (STAT)type;
    }

    public void SetSoulStatCheckPnl(bool value)
    {
        soulStatCheckPnl.SetActive(value);
        if (value)
        {
            soulStatCancelBtn.interactable = true;
            soulStatOkBtn.interactable = true;
            soulStatCheckTxt.text = GetRequiredStat(upgrading).ToString() + " Soul이 필요합니다.\n업그레이드 하시겠습니까?";
        }
    }

    /// <summary>
    /// soulOkBtn을 누르면 upgrading의 스탯이 업그레이드 됨
    /// 만약 MaxStatUpCheck에서 false를 받으면 알려주고 checkPnl을 닫음
    /// </summary>
    public void MaxStatUp()
    {
        if (!isUpgraded)
        {
            if (MaxStatUpCheck(upgrading))
            {
                
                soulStatCancelBtn.interactable = false;
                soulStatOkBtn.interactable = false;

                Debug.Log("Max Stat Upgraded");
                AddMaxStat(upgrading, 1);
                moneyManager.Save();
                soulShopSoulTxt.text = moneyManager.soul.ToString();
                isUpgraded = true;
                StartCoroutine(WaitForCloseStatSelectPnl());
            }
            else
            {
                Debug.Log("Not Enough Soul");
                soulStatCancelBtn.interactable = false;
                soulStatOkBtn.interactable = false;
                soulStatCheckTxt.text = "Soul이 부족합니다!";
                StartCoroutine(WaitForCloseStatSelectPnl());
            }
        }
    }

    public IEnumerator WaitForCloseStatSelectPnl()
    {
        yield return new WaitForSecondsRealtime(1);
        SetSoulStatCheckPnl(false);
        soulStatCancelBtn.interactable = true;
        soulStatOkBtn.interactable = true;
        isUpgraded = false;
        SyncMaxStatUpgradeTxt();
        SyncSoulShopSoulTxt();
    }

    public bool MaxStatUpCheck(STAT type)
    {
        int required = GetRequiredStat(type);
        if (moneyManager.soul >= required)
        {
            moneyManager.AddSoul(-required);
            return true;
        }
        else return false;
    }

    public int GetRequiredStat(STAT type)
    {
        return ((int)GetMaxStat(type) - 4) * 10;
    }

    public void SyncMaxStatUpgradeTxt()
    {
        maxUpgradeTxt[0].text = GetMaxStat((STAT)0).ToString();
        maxUpgradeTxt[1].text = GetMaxStat((STAT)1).ToString();
        maxUpgradeTxt[2].text = GetMaxStat((STAT)2).ToString();
        maxUpgradeTxt[3].text = GetMaxStat((STAT)3).ToString();
        maxUpgradeTxt[4].text = GetMaxStat((STAT)4).ToString();
        maxUpgradeTxt[5].text = GetMaxStat((STAT)5).ToString();
        maxUpgradeTxt[6].text = GetMaxStat((STAT)6).ToString();
        maxUpgradeTxt[7].text = GetMaxStat((STAT)7).ToString();
    }

    public float GetMaxStat(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return PlayerPrefs.GetFloat("dmgMax");
            case STAT.SPD:
                return PlayerPrefs.GetFloat("spdMax");
            case STAT.LUCK:
                return PlayerPrefs.GetFloat("luckMax");
            case STAT.HP:
                return PlayerPrefs.GetFloat("hpMax");
            case STAT.MP:
                return PlayerPrefs.GetFloat("mpMax");
            case STAT.HPREGEN:
                return PlayerPrefs.GetFloat("hpRegenMax");
            case STAT.MPREGEN:
                return PlayerPrefs.GetFloat("mpRegenMax");
            case STAT.STATPOINT:
                return PlayerPrefs.GetInt("statPoint");
        }
        return -1;
    }

    public void SetMaxStat(STAT type, float value)
    {
        switch (type)
        {
            case STAT.DMG:
                PlayerPrefs.SetFloat("dmgMax", value);
                break;
            case STAT.SPD:
                PlayerPrefs.SetFloat("spdMax", value);
                break;
            case STAT.LUCK:
                PlayerPrefs.SetFloat("luckMax", value);
                break;
            case STAT.HP:
                PlayerPrefs.SetFloat("hpMax", value);
                break;
            case STAT.MP:
                PlayerPrefs.SetFloat("mpMax", value);
                break;
            case STAT.HPREGEN:
                PlayerPrefs.SetFloat("hpRegenMax", value);
                break;
            case STAT.MPREGEN:
                PlayerPrefs.SetFloat("mpRegenMax", value);
                break;
            case STAT.STATPOINT:
                PlayerPrefs.SetInt("statPoint", (int)value);
                break;
        }
    }

    public void AddMaxStat(STAT type, float amount)
    {
        SetMaxStat(type, GetMaxStat(type) + amount);
    }

    /// <summary>
    /// Player가 Enemy를 모두 처치하고 화면 상단에 도착하면 호출
    /// </summary>
    public void LevelUp()
    {
        SetPause(true);
        //player.agent.OnDestinationInvalid -= LevelUp;
        //player.agent.OnDestinationReached -= LevelUp;
        isLevelUp = true;
        SyncStatUpgradeTxt();
        SetSelectPnl(true);
        Save();
    }

    public void SetSelectPnl(bool value)
    {
        selectPnl.SetActive(value);
        //Debug.Log("SelectPnl " + value);
    }

    public void SyncStatUpgradeTxt()
    {
        upgradeTxt[0].text = string.Format("{0}/{1}", player.stat.dmg.ToString(), player.maxStat.dmg);
        upgradeTxt[1].text = string.Format("{0}/{1}", player.stat.spd.ToString(), player.maxStat.spd);
        upgradeTxt[2].text = string.Format("{0}/{1}", player.stat.luck.ToString(), player.maxStat.luck);
        upgradeTxt[3].text = string.Format("{0}/{1}", player.stat.hp.ToString(), player.maxStat.hp);
        upgradeTxt[4].text = string.Format("{0}/{1}", player.stat.mp.ToString(), player.maxStat.mp);
        upgradeTxt[5].text = string.Format("{0}/{1}", player.stat.hpRegen.ToString(), player.maxStat.hpRegen);
        upgradeTxt[6].text = string.Format("{0}/{1}", player.stat.mpRegen.ToString(), player.maxStat.mpRegen);
    }

    public void EndLevelUp()
    {
        isLevelUp = false;
        SetSelectPnl(false);
        SetPause(false);
        StageUp();
        Save();
    }

    public void StatUp(int type)
    {
        if (!isUpgraded)
        {
            if (player.AddStat((STAT)type, 1))
            {
                Debug.Log("Stat Upgraded");
                isUpgraded = true;
                cancelBtn.interactable = false;
                statPnl.SetActive(false);
                StartCoroutine(WaitForStatUpClose());
                StatTextUpdate();
                EndLevelUp();
            }
            else
            {
                Debug.Log("Stat Maxed");
            }
        }
    }

    public void StageUp()
    {
        boardManager.ClearStage();
        boardManager.StageUp();
        boardManager.InitBoard();
    }

    private IEnumerator WaitForStatUpClose()
    {
        yield return new WaitForSecondsRealtime(1);
        isUpgraded = false;
        cancelBtn.interactable = true;
        SetPause(false);
    }

    public void Setting()
    {
    }

    /// <summary>
    /// 플레이어가 죽었을 시 호출
    /// Player의 OnDeath에서 호출 됨
    /// </summary>
    public void OnEnd()
    {
        //SetPause(true);
        moneyManager.CollectedSoulToSoul();
        moneyManager.SetGold(0);
        PlayerPrefs.SetInt("isRun", 0);
        Save();
        Debug.Log("End");
        StartCoroutine(WaitForEnd());
        player.GetComponent<Collider2D>().enabled = true;
    }

    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private IEnumerator WaitForEnd()
    {
        yield return new WaitForSecondsRealtime(3);
        boardManager.ClearStage();
        player.SetDeath(false);
        Load();
        player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        yield return new WaitForSecondsRealtime(1);
        player.animator.updateMode = AnimatorUpdateMode.Normal;
    }
}
