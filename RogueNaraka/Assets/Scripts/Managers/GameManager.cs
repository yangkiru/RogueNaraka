﻿//#define DELAY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using RogueNaraka.UnitScripts;
using TMPro;

public class GameManager : MonoBehaviour {

    const string version = "1.0.0";
    const bool isReset = true;
    public static Language language;

    [SerializeField][ReadOnly]
    public static GameManager instance = null;
    public BoardManager boardManager;
    public MoneyManager moneyManager;
    public LevelUpManager levelUpManager;
    public SoulShopManager soulShopManager;
    public SkillManager skillManager;
    public DeathEffectPool deathEffectPool;
    public GameObject settingPnl;
    public Unit player
    {
        get { return boardManager.player; }
    }
    
    public Button cancelBtn;//stat Upgrade

    public TMPro.TextMeshProUGUI[] statTxt;

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

        //업데이트된 버전이 초기화가 필요하면
        if (isReset && PlayerPrefs.GetInt(version) == 0)
        {
            PlayerPrefs.SetInt("isReset", 0);
        }

        string lang = PlayerPrefs.GetString("language");
        try
        {
            if (lang == string.Empty)
                language = (Language)System.Enum.Parse(typeof(Language), Application.systemLanguage.ToString());
            else
                language = (Language)System.Enum.Parse(typeof(Language), lang);
        } catch
        {
            language = Language.English;
        }
        Application.targetFrameRate = 60;
    }

    private void RandomStat(Stat stat)
    {
        int statPoint = stat.statPoints;
        Debug.Log("RandomStat " + stat.statPoints);
        while(statPoint > 0)
        {
            int type = Random.Range(0, (int)STAT.MPREGEN + 1);
            if (stat.AddOrigin((STAT)type, 1))
            {
                Debug.Log((STAT)type);
                statPoint--;
            }
            else
            {
                int origin = (int)stat.sumOrigin;
                int max = (int)stat.sumMax;
                Debug.Log("origin:" + origin + " max:" + max);
                if (origin >= max)
                    break;
            }
        }
    }

    private IEnumerator RandomStatOrb(Stat stat, Stat random)
    { 
        float sum = random.sumOrigin;
        Debug.Log("RandomStatOrb " + random.ToString());
        float delay = 1 / sum;
        StartCoroutine(StatOrbManager.instance.ActivePot(true));

        float t = 3;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        do
        {
            System.Action<STAT, Stat> onEnd = null;
            int type = Random.Range(0, (int)STAT.MPREGEN + 1);
            if (random.GetOrigin((STAT)type) <= 0)
                continue;

            random.AddOrigin((STAT)type, -1);
            sum = random.sumOrigin;

            if (sum >= 1)
                onEnd = AddStat;
            else
                onEnd = OnRandomStatEnd;
            
            StatOrbManager.instance.AddStat((STAT)type, stat, onEnd);
            
            t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);
        } while(sum > 0) ;
    }

    void AddStat(STAT type, Stat stat)
    {
        stat.AddOrigin(type, 1);
        StatTextUpdate(stat);
    }

    void OnRandomStatEnd(STAT type, Stat stat)
    {
        stat.AddOrigin(type, 1);
        StatTextUpdate(stat);
        PlayerPrefs.SetString("randomStat", string.Empty);
        
        stat.currentHp = stat.GetCurrent(STAT.HP);
        stat.currentMp = stat.GetCurrent(STAT.MP);

        Stat.StatToData(stat);

        StartCoroutine(StatOrbManager.instance.ActivePot(false));

        StartCoroutine(RunGameCorou(stat, 0.5f));
    }

    IEnumerator RunGameCorou(Stat stat, float time)
    {
        while(time > 0)
        {
            yield return null;
            time -= Time.unscaledDeltaTime;
        }
        RunGame(stat);
    }

    public IEnumerator AutoSave(float time)
    {
        while (true)
        {
            Save();
            float t = 0;
            while (t < time)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    public void Save()
    {
        moneyManager.Save();

        EffectData[] effectDatas = new EffectData[player.effectable.effects.Count];
        for (int i = 0; i < effectDatas.Length; i++)
        {
            effectDatas[i] = (EffectData)(player.effectable.effects[i].data.Clone());
        }
        PlayerPrefs.SetString("effect", JsonHelper.ToJson<EffectData>(effectDatas));

        PlayerPrefs.SetString("stat", Stat.StatToJson(player.data.stat));
        PlayerPrefs.SetInt("stage", boardManager.stage);
        PlayerPrefs.SetInt("weapon", player.attackable.weapon.id);
        SkillManager.instance.Save();
        Item.instance.Save();

        //Debug.Log("Saved");
    }

    private void ResetData()
    {
        UnityEngine.PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt(version, 1);

        moneyManager.Reset();
        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        soulShopManager.ShopStage(SoulShopManager.SHOPSTAGE.SET);

        UnitData playerBase = GameDatabase.instance.playerBase;

        Stat dbStat = (Stat)playerBase.stat.Clone();
        Stat.StatToData(dbStat);
       
        PlayerPrefs.SetString("stat", Stat.StatToJson(dbStat));
        PlayerPrefs.SetString("randomStat", string.Empty);
        
        PlayerPrefs.SetString("effect", string.Empty);
        PlayerPrefs.SetInt("isRun", 0);
        PlayerPrefs.SetInt("isLevelUp", 0);

        PlayerPrefs.SetInt("exp", 0);

        PlayerPrefs.SetInt("stage", 1);

        PlayerPrefs.SetInt("isReset", 1);
        PlayerPrefs.SetInt("language", 0);
    }

    private void RunGame(Stat stat = null)
    {
        Debug.Log("RunGame");
        moneyManager.Load();
        
        if(stat == null)
            stat = Stat.JsonToStat(PlayerPrefs.GetString("stat"));
        UnitData playerData = (UnitData)GameDatabase.instance.playerBase.Clone();
        playerData.weapon = GetPlayerWeapon(PlayerPrefs.GetInt("exp")).id;
        playerData.stat = stat;
        string effect = PlayerPrefs.GetString("effect");
        if (effect != string.Empty)
            playerData.effects = JsonHelper.FromJson<EffectData>(effect);

        boardManager.SpawnPlayer(playerData);

        boardManager.SetStage(PlayerPrefs.GetInt("stage"));

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

    private void LoadInit()
    {
        UnitData playerBase = (UnitData)GameDatabase.instance.playerBase.Clone();
        Stat stat = Stat.JsonToStat(PlayerPrefs.GetString("stat"));
        stat.SetOrigin(playerBase.stat);
        Stat randomStat = new Stat();
        randomStat.SetMax(stat);
        for (int i = 0; i < (int)STAT.MPREGEN + 1; i++)
            randomStat.AddMax((STAT)i, -playerBase.stat.GetOrigin((STAT)i));
        randomStat.statPoints = stat.statPoints;
        RandomStat(randomStat);

        Stat.StatToData(randomStat, "randomStat");
        Stat.StatToData(stat);

        PlayerPrefs.SetInt("stage", 1);
  
        skillManager.ResetSave();
        Item.instance.ResetSave();
        soulShopManager.ShopStage(SoulShopManager.SHOPSTAGE.SET);
    }

    public void Load()
    {
        if (PlayerPrefs.GetInt("isReset") == 0)//reset
        {
            Debug.Log("Load First");
            ResetData();
        }
        if (PlayerPrefs.GetInt("isRun") == 1)
        {
            Debug.Log("Open Run");

            Stat stat = Stat.JsonToStat(PlayerPrefs.GetString("stat"));
            Stat randomStat = Stat.DataToStat("randomStat");
            StatTextUpdate(stat);

            if (randomStat != null)
            {
                StartCoroutine(RandomStatOrb(stat, randomStat));
            }
            else
                RunGame(stat);
        }
        else//Init Run
        {
            Debug.Log("RunReset");
            PlayerPrefs.SetInt("isRun", 1);
            LoadInit();
            Load();
        }
    }

    public PlayerWeaponData GetPlayerWeapon(int exp)
    {
        for (int i = 0; i < GameDatabase.instance.playerWeapons.Length; i++)
        {
            exp -= GameDatabase.instance.playerWeapons[i].cost;
            if (exp < 0)
            {
                return GameDatabase.instance.playerWeapons[i];
            }
        }
        return null;
    }

    public PlayerWeaponData GetPlayerWeapon(int exp, out int remain)
    {
        remain = -1;
        int i;
        for(i = 0; i < GameDatabase.instance.playerWeapons.Length; i++)
        {
            exp -= GameDatabase.instance.playerWeapons[i].cost;
            if (exp < 0)
            {
                remain = GameDatabase.instance.playerWeapons[i].cost + exp;
                return GameDatabase.instance.playerWeapons[i];
            }
        }
        return GameDatabase.instance.playerWeapons[i - 1];
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

    public void StatTextUpdate(Stat stat)
    {
        statTxt[0].text = stat.dmg.ToString();
        statTxt[1].text = stat.spd.ToString();
        statTxt[2].text = stat.tec.ToString();
        statTxt[3].text = stat.hp.ToString();
        statTxt[4].text = stat.hpRegen.ToString();
        statTxt[5].text = stat.mp.ToString();
        statTxt[6].text = stat.mpRegen.ToString();
    }

    public void SetPause(bool value)
    {
        _isPause = value;
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void SetSettingPnl(bool value)
    {
        if(value)
        {
            SetPause(true);
        }
        else
        {
            if (!RollManager.instance.rollPnl.activeSelf && !SoulShopManager.instance.shopPnl.activeSelf)
                SetPause(false);
        }
        settingPnl.SetActive(value);
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
        float t = 0;
        while (t < 3)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        Debug.Log("Waited");
        player.deathable.Revive();
        
        player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        player.animator.updateMode = AnimatorUpdateMode.Normal;

        SkillManager.instance.ResetSave();

        boardManager.ClearStage();
        Load();
    }

    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void SetLanguage(int num)
    {
        language = (Language)num;
        PlayerPrefs.SetString("language", language.ToString());
    }

    public void InitDropdown(TMP_Dropdown dropdown)
    {
        dropdown.value = (int)language;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
