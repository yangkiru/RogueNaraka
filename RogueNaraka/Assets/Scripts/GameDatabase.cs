using System;
using System.Collections.Generic;
using UnityEngine;
using RogueNaraka.SkillScripts;

public class GameDatabase : ScriptableObject
{

    private static GameDatabase _instance;
    public static GameDatabase instance
    {
        get
        {
            if (_instance == null) _instance = Resources.Load<GameDatabase>("GameDatabase");
            return _instance;
        }
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Game/Create Default Database")]
    public static void Save()
    {
        if (_instance == null) _instance = new GameDatabase();
        if (!System.IO.Directory.Exists("Assets/Resources")) System.IO.Directory.CreateDirectory("Assets/Resources");
        UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/Resources/GameDatabase.asset");
    }
#endif

    public static int friendlyLayer = 8;
    public static int enemyLayer = 9;
    public static int wallLayer = 10;
    public UnitData playerBase;
    public UnitData[] enemies;
    public UnitData[] bosses;
    public UnitData[] spawnables;
    public LayerMask unitMask;
    public LayerMask friendlyMask;
    public LayerMask enemyMask;
    public LayerMask wallMask;
    public BulletData[] bullets;
    public WeaponData[] weapons;
    public PlayerWeaponData[] playerWeapons;
    public int[] stageCosts;
    public UnitCost[] unitCosts;
    public SkillData[] skills;
    public ItemData[] items;
    public ItemSpriteData[] itemSprites;
    public EffectSpriteData[] effects;
    public Sprite statSprite;

    public StatLang[] statLang;
    public string[] langFonts;

    void OnEnable()
    {
        BulletIdToData();
        UnitCostSync();
    }

    [ContextMenu("BulletIdToData")]
    public void BulletIdToData()
    {
        for(int i = 0; i < bullets.Length; i++)
        {
            bullets[i].id = i;
        }
    }

    [ContextMenu("UnitCostSync")]
    public void UnitCostSync()
    {
        List<int> costList = new List<int>();
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].id = i;
            if (!costList.Contains(enemies[i].cost))
                costList.Add(enemies[i].cost);
        }

        unitCosts = new UnitCost[costList.Count];

        for (int i = 0; i < costList.Count; i++)
        {
            List<int> list = new List<int>();
            for(int j = 0; j < enemies.Length; j++)
            {
                if (costList[i] == enemies[j].cost)
                    list.Add(enemies[j].id);
            }
            unitCosts[i].cost = costList[i];
            unitCosts[i].unitId = list.ToArray();
        }
    }

    [ContextMenu("StageFunc")]
    public void StageFunc()
    {
        for(int i = 0; i < 50; i++)
        {
            stageCosts[i] = 3 + i * 2;
        }
    }

    void BulletSwap(int a, int b)
    {
        BulletData temp = bullets[a];
        bullets[a] = bullets[b];
        bullets[b] = temp;
    }

    void BulletClone(int from, int to)
    {
        bullets[to] = (BulletData)bullets[from].Clone();
    }

    void EffectSpriteName()
    {
        for(int i = 0; i < effects.Length; i++)
        {
            effects[i].name = effects[i].type.ToString();
        }
    }

    [ContextMenu("Temp")]
    void Temp()
    {
        EffectSpriteName();
    }
}

[Serializable]
public struct UnitCost
{
    public int cost;
    public int[] unitId;
}

[Serializable]
public class UnitData : ICloneable
{
    public string name;
    public int id;
    public int cost;
    public int stage;
    public int maxStage;
    public int weapon;
    public Stat stat;
    public RuntimeAnimatorController controller;
    public RuntimeAnimatorController deathEffectController;
    public Color color = Color.white;
    public Color bulletColor = Color.white;
    public bool isFriendly;
    public bool isStanding;
    public bool isDeathEffect;
    public bool isCorpse;
    public MOVE_TYPE move;
    public float tackleSize;
    public float tackleDamage;
    public float tackleDelay;
    public float moveDelay;
    public float moveDistance;
    public float moveSpeed;
    public float accelerationRate;
    public float decelerationRate;
    public float limitTime;
    public Vector2 shadowPos;
    public Order order;
    public EffectData[] effects;
    public EffectResistance[] resistances;
    public string deathSFX;
    public string attackSFX;

    public object Clone()
    {
        UnitData clone = (UnitData)this.MemberwiseClone();

        clone.stat = (Stat)stat.Clone();
        return clone;
    }
}

[Serializable]
public struct ShakeData
{
    public float time;
    public float power;
    public float gap;
    public bool isOnHit;
}

[Serializable]
public class PlayerWeaponData
{
    public int level;
    public int cost;
    public int id;
}

[Serializable]
public class WeaponData : ICloneable
{
    public string name;
    public int id;
    public ATTACK_TYPE type;
    public int startBulletId;
    public BulletChildData[] children;
    public BulletChildData[] onDestroy;
    public Vector3 offset;
    public float beforeAttackDelay;
    public float afterAttackDelay;
    public float attackDistance;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public class BulletData : ICloneable
{
    public string name;
    public int id;
    public float dmg;
    public STAT related;
    public RuntimeAnimatorController controller;
    public BULLET_TYPE type;
    public float localSpeed;
    public float worldSpeed;
    public float localAccel;
    public float worldAccel;
    public float guideSpeed;
    public float spinSpeed;
    public float size;
    public float delay;
    public float limitTime;
    public float disapearStartTime;
    public float disapearDuration;
    public int pierce;
    public bool isSplash;
    public bool isHitableWall;
    public bool isBounceable;
    public ShakeData shake;
    public Order order;
    public EffectData[] effects;

    public BulletChildData[] children;
    public BulletChildData[] onDestroy;

    public string[] hitSFX;
    public string spawnSFX;
    
    public object Clone()
    {
        BulletData data = (BulletData)this.MemberwiseClone();

        data.effects = new EffectData[effects.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            data.effects[i] = (EffectData)effects[i].Clone();
        }

        data.children = new BulletChildData[children.Length];
        for (int i = 0; i < children.Length;i++)
        {
            data.children[i] = (BulletChildData)children[i].Clone();
        }

        data.onDestroy = new BulletChildData[onDestroy.Length];
        for (int i = 0; i < onDestroy.Length; i++)
        {
            data.onDestroy[i] = (BulletChildData)onDestroy[i].Clone();
        }
        return data;
    }

    public EffectData GetEffect(EFFECT type)
    {
        for(int i = 0; i < effects.Length;i++)
        {
            if (effects[i].type == type)
                return effects[i];
        }
        return null;
    }
}

[Serializable]
public class BulletChildData : ICloneable
{
    public string name;
    public int bulletId;
    public int sortingOrder;
    public float startTime;
    public float waitTime;
    public float angle;

    public bool isRepeat;
    public bool isStick;
    public bool isDestroyWith;

    public Vector3 offset;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public class SkillData:ICloneable
{
    public string name;
    public string[] nameLang;
    public int[] bulletIds;
    public int[] unitIds;
    public Sprite spr;
    public int id;
    public int level;
    public int cost;
    public float coolTime;
    public float coolTimeLeft;
    public float manaCost;
    public float size;
    public bool isCircleToPlayer;
    public bool isDeath;
    public bool isBasic;
    public EffectData[] effects;
    public ValueData[] values;
    public SkillUpData levelUp;
    [TextArea]
    public string[] description;

    public string useSFX;

    public object Clone()
    {
        SkillData clone = (SkillData)this.MemberwiseClone();
        clone.bulletIds = new int[bulletIds.Length];
        for (int i = 0; i < bulletIds.Length; i++)
            clone.bulletIds[i] = bulletIds[i];

        clone.unitIds = new int[unitIds.Length];
        for (int i = 0; i < unitIds.Length; i++)
            clone.unitIds[i] = unitIds[i];
        clone.effects = new EffectData[effects.Length];
        for (int i = 0; i < effects.Length; i++)
            clone.effects[i] = (EffectData)effects[i].Clone();
        clone.values = new ValueData[values.Length];
        for (int i = 0; i < values.Length; i++)
            clone.values[i] = (ValueData)values[i].Clone();
        return clone;
    }

    public void Reset()
    {
        name = string.Empty; spr = null; id = -1; level = 1; coolTime = 0; coolTimeLeft = 0; manaCost = 0; effects = new EffectData[0]; values = new ValueData[0];
    }

    public string GetName()
    {
        string result = nameLang.Length > (int)GameManager.language ?
            nameLang[(int)GameManager.language] : (nameLang.Length > 0 ? nameLang[0] : name);
        return result;
    }

    public string GetDescription()
    {
        string result = description.Length > (int)GameManager.language ?
            description[(int)GameManager.language] : (description.Length > 0 ? description[0] : string.Empty);

        return result;
    }

    public static bool[] GetBoughtSkills()
    {
        string str = PlayerPrefs.GetString("boughtSkills");
        bool[] save;
        bool[] result;
        bool isSave = false;
        if ((str.CompareTo(string.Empty) != 0))
            save = JsonHelper.FromJson<bool>(str);
        else
        {
            save = new bool[GameDatabase.instance.skills.Length];
            isSave = true;
        }

        if (save.Length < GameDatabase.instance.skills.Length)
            result = new bool[GameDatabase.instance.skills.Length];
        else
            result = save;
        for (int i = 0; i < result.Length; i++)
        {
            if (GameDatabase.instance.skills[i].isBasic)
                result[i] = false;
        }
        if (isSave)
            SaveBoughtSkills(result);
        return result;
    }

    public static bool IsBought(int id)
    {
        bool[] boughts = GetBoughtSkills();

        return boughts[id];
    }

    public static void Buy(int id)
    {
        bool[] bought = GetBoughtSkills();
        bought[id] = true;
        SaveBoughtSkills(bought);
    }

    public static void SaveBoughtSkills(bool[] boughts)
    {
        PlayerPrefs.SetString("boughtSkills", JsonHelper.ToJson<bool>(boughts));
    }
}

[Serializable]
public struct SkillSaveData
{
    public int id;
    public int level;
    public float coolTimeLeft;

    public static SkillSaveData SkillToSave(SkillData data)
    {
        SkillSaveData saveData = new SkillSaveData();
        saveData.id = data.id;
        saveData.level = data.level;
        saveData.coolTimeLeft = data.coolTimeLeft;
        return saveData;
    }
}

[Serializable]
public class SkillUpData : ICloneable
{
    public float manaCost;
    public float size;
    public float coolTime;
    public EffectData[] effects;
    public ValueData[] values;

    public object Clone()
    {
        SkillUpData clone = (SkillUpData)this.MemberwiseClone();
        clone.effects = new EffectData[effects.Length];
        for(int i = 0; i < effects.Length; i++)
        {
            clone.effects[i] = (EffectData)effects[i].Clone();
        }
        clone.values = new ValueData[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            clone.values[i] = (ValueData)values[i].Clone();
        }
        return clone;
    }
}

[Serializable]
public class ValueData : ICloneable
{
    public Value name;
    public float value;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public struct EffectSpriteData
{
    public string name;
    public EFFECT type;
    public Sprite spr;
    public Sprite particle;
}

[Serializable]
public class EffectResistance
{
    public EFFECT type;
    public float value;
}

[Serializable]
public class EffectData:ICloneable
{
    public EFFECT type;
    public float value;
    public float time;

    public EffectData() { }

    public EffectData(EFFECT e, float v, float t)
    {
        type = e; value = v; time = t;
    }

    public EffectData(EffectData ef) : this(ef.type, ef.value, ef.time) { }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public struct ItemData
{
    public string name;
    public string[] nameLang;
    public int id;
    public float value;
    public float size;
    [TextArea]
    public string[] description;

    public string GetName()
    {
        string result = nameLang.Length > (int)GameManager.language ?
            nameLang[(int)GameManager.language] : (nameLang.Length > 0 ? nameLang[0] : name);
        return result;
    }

    public string GetDescription()
    {
        string result = description.Length > (int)GameManager.language ?
            description[(int)GameManager.language] : (description.Length > 0 ? description[0] : string.Empty);

        return result;
    }
}

[Serializable]
public struct ItemSpriteData
{
    public int id;
    public string name;
    public Sprite spr;
    [TextArea]
    public string description;
}

[Serializable]
public class StatLang
{
    public string lang;
    public string[] items;
}

[Serializable]
public enum BULLET_TYPE
{
    CIRCLE_CAST, DYNAMIC_CIRCLE, NONE, DYNAMIC_POLY, RAY_CAST
}

[Serializable]
public enum EFFECT
{
    Stun, Slow, Fire, Ice, Knockback, Poison, Heal, LifeSteal, DmgBuff, SpdBuff, TecBuff,
    HpBuff, HpRegenBuff, MpBuff, MpRegenBuff, Accel
}

[Serializable]
public enum ATTACK_TYPE
{
    STOP_BEFORE, STOP_AFTER, DONT_STOP
}

[Serializable]
public enum Value
{
    Amount, Time, Damage, LifeSteal, Hp, Accel, Delay, Fire
}

[Serializable]
public enum MOVE_TYPE
{
    RANDOM,
    RUSH,//근접 공격
    STATUE,//고정
    DISTANCE,//거리 유지
    RUN,//도망
    REST_RUSH,
    FOLLOW,
    BOSS0
}

[Serializable]
public enum Language
{
    English, Korean
}