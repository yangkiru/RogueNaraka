//#define DELAY
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using RogueNaraka.UnitScripts;

public class GameManager : MonoBehaviour {

    const string version = "1.0.0";
    const bool isReset = true;

    [SerializeField][ReadOnly]
    public static GameManager instance = null;
    public BoardManager boardManager;
    public MoneyManager moneyManager;
    public LevelUpManager levelUpManager;
    public SoulShopManager soulShopManager;
    public SkillManager skillManager;
    public DeathEffectPool deathEffectPool; 
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

        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Load();
    }

    private void RandomStat()
    {
        Stat stat = Stat.JsonToStat(PlayerPrefs.GetString("stat"));
        int statPoint = stat.statPoints;
        Debug.Log("statPoint"+statPoint);

        Stat statBase = (Stat)GameDatabase.instance.playerBase.stat.Clone();
        stat.SetOrigin(statBase);
        while(statPoint > 0)
        {
            int type = Random.Range(0, (int)STAT.MPREGEN + 1);
            if(stat.AddOrigin((STAT)type, 1))
            {
                statPoint--;
            }
            else
            {
                int current = (int)stat.sumOrigin;
                int max = (int)stat.sumMax;
                if (current >= max)
                {
                    return;
                }
                //Maxed
            }
        }

        stat.currentHp = stat.hp;
        stat.currentMp = stat.mp;
        PlayerPrefs.SetString("stat", Stat.StatToJson(stat));
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
        
        PlayerPrefs.SetString("effect", string.Empty);
        PlayerPrefs.SetInt("isRun", 0);
        PlayerPrefs.SetInt("isLevelUp", 0);
        PlayerPrefs.SetInt("weapon", playerBase.weapon);

        PlayerPrefs.SetInt("stage", 1);

        PlayerPrefs.SetInt("isReset", 1);
    }

    private void RunGame()
    {
        Debug.Log("RunGame");
        moneyManager.Load();

        Stat stat = Stat.JsonToStat(PlayerPrefs.GetString("stat"));
        UnitData playerData = (UnitData)GameDatabase.instance.playerBase.Clone();
        playerData.weapon = PlayerPrefs.GetInt("weapon");
        playerData.stat = stat;
        string effect = PlayerPrefs.GetString("effect");
        if (effect != string.Empty)
            playerData.effects = JsonHelper.FromJson<EffectData>(effect);
        boardManager.SpawnPlayer(playerData);

        StatTextUpdate();

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
        UnitData playerBase = (UnitData)GameDatabase.instance.playerBase.Clone();
        Stat.StatToData(playerBase.stat);
        PlayerPrefs.SetInt("stage", 1);
        PlayerPrefs.SetInt("weapon", playerBase.weapon);
  
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
            RunGame();
        }
        else//Init Run
        {
            Debug.Log("RunReset");
            LoadInit();
            PlayerPrefs.SetInt("isRun", 1);
            RandomStat();
            RunGame();
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
        for(int i = 0; i < 3; i++)
        {
            SkillManager.instance.skills[i].ResetSkill();
        }
        Load();
    }

    public static Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
