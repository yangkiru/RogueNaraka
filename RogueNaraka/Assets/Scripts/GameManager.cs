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
    
    public Button cancelBtn;//stat Upgrade

    public TMPro.TextMeshProUGUI[] statTxt;


    //Debug params
    public int weaponId;
    public int weaponLevel;
    private int _weaponId = 0;
    private int _weaponLevel = 0;
    public int stage;

    public bool isDebug;
    public bool isReset;
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
            PlayerPrefs.SetFloat("tec", player.stat.tec);
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
        //Debug.Log("Saved");
    }

    public void RunGame()
    {
        isRun = true;
        PlayerPrefs.SetInt("isRun", 1);
        Load();
        RandomStat();
        PlayerPrefs.SetFloat("health", player.stat.hp);
        PlayerPrefs.SetFloat("mana", player.stat.mp);
        Save();
        StatTextUpdate();
    }

    private void LoadFirst()
    {
        moneyManager.Save();
        Stat dbStat = GameDatabase.instance.playerBase;
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
        Weapon weapon = new Weapon(GameDatabase.instance.weapons[weaponId]);//weapon save data로 변경 필요
        weapon.level = weaponLevel;
        PlayerPrefs.SetString("weapon", JsonUtility.ToJson(weapon));

        PlayerPrefs.SetInt("stage", 1);

        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        Item.instance.Load();
    }

    private void LoadRun()
    {
        isRun = true;
        moneyManager.Load();
        Stat stat = new Stat(PlayerPrefs.GetFloat("dmg"), PlayerPrefs.GetFloat("spd"),
        PlayerPrefs.GetFloat("tec"), PlayerPrefs.GetFloat("hp"), PlayerPrefs.GetFloat("mp"),
        PlayerPrefs.GetFloat("hpRegen"), PlayerPrefs.GetFloat("mpRegen"));

        player.SyncData(stat, true, false);
        player.SetHealth(PlayerPrefs.GetFloat("health"));
        player.SetMana(PlayerPrefs.GetFloat("mana"));

        StatTextUpdate();

        player.SetMaxStat(new Stat(GetStat((STAT)0, true), GetStat((STAT)1, true), GetStat((STAT)2, true),
            GetStat((STAT)3, true), GetStat((STAT)4, true), GetStat((STAT)5, true), GetStat((STAT)6, true)));

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
            LevelUpManager.instance.LevelUp();
        }
        else
        {
            boardManager.InitBoard();
        }
    }

    private void LoadInit()
    {
        moneyManager.Load();
        Stat dbStat = GameDatabase.instance.playerBase;
        SyncStatToData(dbStat);
        PlayerPrefs.SetInt("stage", 1);
        PlayerPrefs.SetFloat("health", 1);
        PlayerPrefs.SetFloat("mana", 1);
        Weapon weapon = new Weapon(GameDatabase.instance.weapons[weaponId]);//weapon save data로 변경 필요
        weapon.level = weaponLevel;
        PlayerPrefs.SetString("weapon", JsonUtility.ToJson(weapon));
        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        Item.instance.Load();
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

    public void SyncStatToData(Stat stat)
    {
        stat = GameDatabase.instance.playerBase;
        PlayerPrefs.SetFloat("dmg", stat.dmg);
        PlayerPrefs.SetFloat("spd", stat.spd);
        PlayerPrefs.SetFloat("tec", stat.tec);
        PlayerPrefs.SetFloat("hp", stat.hp);
        PlayerPrefs.SetFloat("mp", stat.mp);
        PlayerPrefs.SetFloat("hpRegen", stat.hpRegen);
        PlayerPrefs.SetFloat("mpRegen", stat.mpRegen);
    }

    public void StatTextUpdate()
    {
        statTxt[0].text = player.stat.dmg.ToString();
        statTxt[1].text = player.stat.spd.ToString();
        statTxt[2].text = player.stat.tec.ToString();
        statTxt[3].text = player.stat.hp.ToString();
        statTxt[4].text = player.stat.hpRegen.ToString();
        statTxt[5].text = player.stat.mp.ToString();
        statTxt[6].text = player.stat.mpRegen.ToString();
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
        moneyManager.CollectedSoulToSoul();
        PlayerPrefs.SetInt("isRun", 0);
        Save();
        Debug.Log("End");
        yield return new WaitForSecondsRealtime(3);
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
        SetStat(type, GameManager.GetStat(type, isMax) + amount);
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
}
