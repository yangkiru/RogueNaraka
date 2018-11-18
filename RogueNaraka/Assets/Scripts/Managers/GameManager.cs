//#define DELAY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class GameManager : MonoBehaviour {
    [SerializeField][ReadOnly]
    public static GameManager instance = null;
#if DELAY
    public WaitForSeconds delayPointOne;
    public WaitForSecondsRealtime delayPointOneReal;
    public WaitForSeconds delayOne;
    public WaitForSecondsRealtime delayOneReal;
#endif
    public BoardManager boardManager;
    public MoneyManager moneyManager;
    public LevelUpManager levelUpManager;
    public SoulShopManager soulShopManager;
    public SkillManager skillManager;
    public DeathEffectPool deathEffectPool; 
    public Player player;
    
    public Button cancelBtn;//stat Upgrade

    public TMPro.TextMeshProUGUI[] statTxt;


    //Debug params
    public int stage;

    public bool isDebug;
    public bool isReset;
    public bool isStage;
    public bool spawnEffect;
    public bool autoSave = true;
    public bool removeFirst;

    public float autoSaveTime = 1;

    public bool isPause
    { get { return _isPause; } }
    [SerializeField][ReadOnly]
    private bool _isPause;

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
        if (isReset)
            PlayerPrefs.SetInt("isFirst", 0);
#if DELAY
        delayPointOne = new WaitForSeconds(0.1f);
        delayPointOneReal = new WaitForSecondsRealtime(0.1f);
        delayOne = new WaitForSeconds(1f);
        delayOneReal = new WaitForSecondsRealtime(1f);
#endif
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetInt("isFirst", 0);
            SceneManager.LoadScene(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
            player.Suicide();
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
    }

    private void RandomStat()
    {
        int statPoint = PlayerPrefs.GetInt("statPoint");
        Debug.Log("statPoint"+statPoint);
        while(statPoint > 0)
        {
            int type = Random.Range(0, (int)STAT.MPREGEN + 1);
            if(AddStat((STAT)type, 1))
            {
                statPoint--;
            }
            else
            {
                int current = (int)GetStatSum(false);
                int max = (int)GetStatSum(true);
                if (current >= max)
                {
                    Debug.Log("Max");
                    return;
                }
                //Maxed
            }
        }
        SyncPlayerStat();
        player.SetHealth(GetStat(STAT.HP));
        player.SetMana(GetStat(STAT.MP));
    }

    public IEnumerator AutoSave(float time)
    {
        while (true)
        {
            Save();
            yield return new WaitForSecondsRealtime(time);
        }
    }

    public void Save(bool isRun = true)
    {
        moneyManager.Save();

        EffectData[] effectDatas = new EffectData[player.effects.Count];
        for (int i = 0; i < effectDatas.Length; i++)
        {
            effectDatas[i] = (EffectData)(player.effects[i].data.Clone());
        }
        PlayerPrefs.SetString("effect", JsonHelper.ToJson<EffectData>(effectDatas));

        if (isRun)
        {
            PlayerPrefs.SetFloat("dmg", player.data.stat.dmg);
            PlayerPrefs.SetFloat("spd", player.data.stat.spd);
            PlayerPrefs.SetFloat("tec", player.data.stat.tec);
            PlayerPrefs.SetFloat("hp", player.data.stat.hp);
            PlayerPrefs.SetFloat("mp", player.data.stat.mp);
            PlayerPrefs.SetFloat("hpRegen", player.data.stat.hpRegen);
            PlayerPrefs.SetFloat("mpRegen", player.data.stat.mpRegen);
            PlayerPrefs.SetFloat("health", player.health);
            PlayerPrefs.SetFloat("mana", player.mana);
            PlayerPrefs.SetInt("stage", boardManager.stage);
            SkillManager.instance.Save();
            Item.instance.Save();
            PlayerPrefs.SetInt("weaponId", player.weapon.id);
            PlayerPrefs.SetInt("weaponoLevel", player.weapon.level);
        }
        //Debug.Log("Saved");
    }

    public void RunGame()
    {
        Debug.Log("RunGame");
        PlayerPrefs.SetInt("isRun", 1);
        LoadRun();
        RandomStat();
        PlayerPrefs.SetFloat("health", player.data.stat.hp);
        PlayerPrefs.SetFloat("mana", player.data.stat.mp);
        Save();
        StatTextUpdate();
    }

    private void LoadFirst()
    {
        moneyManager.Reset();
        UnitData playerBase = (UnitData)GameDatabase.instance.playerBase.Clone();
        Stat dbStat = playerBase.stat;
        SyncStatToData(dbStat);

        Stat maxStat = new Stat(5);
        UnityEngine.PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("dmgMax", maxStat.dmg);
        PlayerPrefs.SetFloat("spdMax", maxStat.spd);
        PlayerPrefs.SetFloat("tecMax", maxStat.tec);
        PlayerPrefs.SetFloat("hpMax", maxStat.hp);
        PlayerPrefs.SetFloat("mpMax", maxStat.mp);
        PlayerPrefs.SetFloat("hpRegenMax", maxStat.hpRegen);
        PlayerPrefs.SetFloat("mpRegenMax", maxStat.mpRegen);

        PlayerPrefs.SetInt("isFirst", 1);
        PlayerPrefs.SetString("effect", string.Empty);
        PlayerPrefs.SetInt("statPoint", 5);
        PlayerPrefs.SetInt("isRun", 0);
        PlayerPrefs.SetInt("isLevelUp", 0);
        PlayerPrefs.SetInt("weaponId", playerBase.weaponId);
        PlayerPrefs.SetInt("weaponLevel", playerBase.weaponLevel);

        PlayerPrefs.SetInt("stage", 1);

        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        Item.instance.Load();
        soulShopManager.ShopStage(SoulShopManager.SHOPSTAGE.SET);
    }

    private void LoadRun()
    {
        moneyManager.Load();
        Stat stat = new Stat(PlayerPrefs.GetFloat("dmg"), PlayerPrefs.GetFloat("spd"),
        PlayerPrefs.GetFloat("tec"), PlayerPrefs.GetFloat("hp"), PlayerPrefs.GetFloat("mp"),
        PlayerPrefs.GetFloat("hpRegen"), PlayerPrefs.GetFloat("mpRegen"));
        UnitData playerData = (UnitData)GameDatabase.instance.playerBase.Clone();
        playerData.stat = stat;
        player.SyncData(playerData, false);
        player.SetHealth(PlayerPrefs.GetFloat("health"));
        player.SetMana(PlayerPrefs.GetFloat("mana"));

        StatTextUpdate();

        player.EquipWeapon(PlayerPrefs.GetInt("weaponId"), PlayerPrefs.GetInt("weaponLevel"));

        //StartCoroutine(WaitForEffectPoolInit());
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

        soulShopManager.ShopStage(SoulShopManager.SHOPSTAGE.SYNC);

        if (PlayerPrefs.GetInt("isLevelUp") == 1)
        {
            levelUpManager.LevelUp();
        }
        else
        {
            boardManager.InitBoard();
        }
    }

    public void PlayerEffect()
    {
        if (PlayerPrefs.GetString("effect") != string.Empty)
        {
            EffectData[] temp = JsonHelper.FromJson<EffectData>(PlayerPrefs.GetString("effect"));
            for (int i = 0; i < temp.Length; i++)
            {
                player.AddEffect((EffectData)temp[i].Clone());
            }
        }
    }

    //IEnumerator WaitForEffectPoolInit()
    //{
    //    while (boardManager == null || boardManager.effectPool.GetCount() <= 100)
    //        yield return null;
    //    if (PlayerPrefs.GetString("effect") != string.Empty)
    //    {
    //        EffectData[] temp = JsonHelper.FromJson<EffectData>(PlayerPrefs.GetString("effect"));
    //        for (int i = 0; i < temp.Length; i++)
    //        {
    //            player.AddEffect((EffectData)temp[i].Clone());
    //        }
    //    }
    //}

    private void LoadInit()
    {
        moneyManager.Load();
        UnitData playerBase = (UnitData)GameDatabase.instance.playerBase.Clone();
        SyncStatToData(playerBase.stat);
        PlayerPrefs.SetInt("stage", 1);
        PlayerPrefs.SetFloat("health", 1);
        PlayerPrefs.SetFloat("mana", 1);
        PlayerPrefs.SetInt("weaponId", playerBase.weaponId);
        PlayerPrefs.SetInt("weaponLevel", playerBase.weaponLevel);
        skillManager.ResetSave();
        Item.instance.ResetSave();
        Item.instance.Load();
        soulShopManager.ShopStage(SoulShopManager.SHOPSTAGE.SET);
        RunGame();
    }

    public void Load()
    {
        if (PlayerPrefs.GetInt("isFirst") == 0)//reset
        {
            Debug.Log("Load First");
            LoadFirst();
        }
        if (PlayerPrefs.GetInt("isRun") == 1)
        {
            Debug.Log("Open Run");
            LoadRun();
        }
        else//Init Run
        {
            Debug.Log("Init Run");
            LoadInit();
        }
    }

    //public void SyncDataToStat()
    //{
    //    PlayerPrefs.SetFloat("dmg", player.data.stat.dmg);
    //    PlayerPrefs.SetFloat("spd", player.data.stat.spd);
    //    PlayerPrefs.SetFloat("tec", player.data.stat.tec);
    //    PlayerPrefs.SetFloat("hp", player.data.stat.hp);
    //    PlayerPrefs.SetFloat("mp", player.data.stat.mp);
    //    PlayerPrefs.SetFloat("hpRegen", player.data.stat.hpRegen);
    //    PlayerPrefs.SetFloat("mpRegen", player.data.stat.mpRegen);
    //    PlayerPrefs.SetFloat("health", player.health);
    //    PlayerPrefs.SetFloat("mana", player.mana);
    //    PlayerPrefs.SetInt("stage", boardManager.stage);
    //}

    public void SyncStatToData(Stat stat)
    {
        PlayerPrefs.SetFloat("dmg", stat.dmg);
        PlayerPrefs.SetFloat("spd", stat.spd);
        PlayerPrefs.SetFloat("tec", stat.tec);
        PlayerPrefs.SetFloat("hp", stat.hp);
        PlayerPrefs.SetFloat("mp", stat.mp);
        PlayerPrefs.SetFloat("hpRegen", stat.hpRegen);
        PlayerPrefs.SetFloat("mpRegen", stat.mpRegen);
    }

    public void SyncPlayerStat(bool isMax = false)
    {
         for (int i = 0; i < (int)STAT.MPREGEN + 1; i++)
        {
            player.SetStat((STAT)i, GetStat((STAT)i, isMax));
        }
    }

    public void StatTextUpdate()
    {
        statTxt[0].text = player.data.stat.dmg.ToString();
        statTxt[1].text = player.data.stat.spd.ToString();
        statTxt[2].text = player.data.stat.tec.ToString();
        statTxt[3].text = player.data.stat.hp.ToString();
        statTxt[4].text = player.data.stat.hpRegen.ToString();
        statTxt[5].text = player.data.stat.mp.ToString();
        statTxt[6].text = player.data.stat.mpRegen.ToString();
    }

    public void SetPause(bool value)
    {
        _isPause = value;
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void Setting()
    {
    }

    /// <summary>
    /// 플레이어가 죽었을 시 호출
    /// Player의 OnDeath에서 호출 됨
    /// </summary>
    public IEnumerator OnEnd()
    {
        yield return null;
        moneyManager.RefineSoul();
        PlayerPrefs.SetInt("isRun", 0);
        Debug.Log("End");
        yield return new WaitForSecondsRealtime(3);
        Debug.Log("Waited");
        boardManager.ClearStage();
        player.SetDeath(false);
        Load();
        player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        yield return new WaitForSecondsRealtime(1);
        player.animator.updateMode = AnimatorUpdateMode.Normal;
    }

    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static void AddStat(STAT type, float amount, bool isMax = false)
    {
        SetStat(type, GameManager.GetStat(type, isMax) + amount, isMax);
    }

    public static void SetStat(STAT type, float value, bool isMax = false)
    {
        string max = string.Empty;
        if (isMax)
            max = "Max";
        switch (type)
        {
            case STAT.DMG:
                PlayerPrefs.SetFloat("dmg" + max, value);
                break;
            case STAT.SPD:
                PlayerPrefs.SetFloat("spd" + max, value);
                break;
            case STAT.TEC:
                PlayerPrefs.SetFloat("tec" + max, value);
                break;
            case STAT.HP:
                PlayerPrefs.SetFloat("hp" + max, value);
                break;
            case STAT.MP:
                PlayerPrefs.SetFloat("mp" + max, value);
                break;
            case STAT.HPREGEN:
                PlayerPrefs.SetFloat("hpRegen" + max, value);
                break;
            case STAT.MPREGEN:
                PlayerPrefs.SetFloat("mpRegen" + max, value);
                break;
            case STAT.STATPOINT:
                PlayerPrefs.SetInt("statPoint", (int)value);
                break;
        }
    }

    public static float GetStat(STAT type, bool isMax = false)
    {
        string max = string.Empty;
        if (isMax)
            max = "Max";
        switch (type)
        {
            case STAT.DMG:
                return PlayerPrefs.GetFloat("dmg" + max);
            case STAT.SPD:
                return PlayerPrefs.GetFloat("spd" + max);
            case STAT.TEC:
                return PlayerPrefs.GetFloat("tec" + max);
            case STAT.HP:
                return PlayerPrefs.GetFloat("hp" + max);
            case STAT.MP:
                return PlayerPrefs.GetFloat("mp" + max);
            case STAT.HPREGEN:
                return PlayerPrefs.GetFloat("hpRegen" + max);
            case STAT.MPREGEN:
                return PlayerPrefs.GetFloat("mpRegen" + max);
            case STAT.STATPOINT:
                return PlayerPrefs.GetInt("statPoint");
        }
        return -1;
    }

    public static bool AddStat(STAT type, float amount)
    {
        float value;
        value = GameManager.GetStat(type, false) + amount;
        if (value <= GameManager.GetStat(type, true))
        {
            SetStat(type, value, false);
            return true;
        }
        else
            return false;
    }

    public static float GetStatSum(bool isMax = false)
    {
        string max = string.Empty;
        if (isMax)
            max = "Max";
        Stat temp = new Stat();
        temp.dmg = PlayerPrefs.GetFloat("dmg" + max);
        temp.spd = PlayerPrefs.GetFloat("spd" + max);
        temp.tec = PlayerPrefs.GetFloat("tec" + max);
        temp.hp = PlayerPrefs.GetFloat("hp" + max);
        temp.mp = PlayerPrefs.GetFloat("mp" + max);
        temp.hpRegen = PlayerPrefs.GetFloat("hpRegen" + max);
        temp.mpRegen = PlayerPrefs.GetFloat("mpRegen" + max);

        return temp.sum;
    }
}
