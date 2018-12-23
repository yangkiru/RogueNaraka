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
    public int[] stageCosts;
    public UnitCost[] unitCosts;
    public SkillData[] skills;
    public ItemData[] items;
    public ItemSpriteData[] itemSprites;
    public EffectSpriteData[] effects;
    public Sprite statSprite;

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

    [ContextMenu("Temp")]
    void Temp()
    {
        //bullets[34] = (BulletData)bullets[15].Clone();
    }
}

[Serializable]
public struct UnitCost
{
    public int cost;
    public int[] unitId;
}

//[Serializable]
//public struct KnowledgeData
//{
//    public float fire;
//    public float ice;
//    public float poison;
//    public float knockBack;
//    public float stun;
//    public float slow;
//    public float gravity;

//    public KnowledgeData(float value)
//    {
//        fire = value; ice = value; poison = value; knockBack = value; stun = value; slow = value; gravity = value;
//    }
//    public KnowledgeData(KnowledgeData k, float value = 0) : this(value)
//    {
//        fire += k.fire; ice += k.ice; poison += k.poison; knockBack += k.knockBack; stun += k.stun; slow += k.slow; gravity += k.gravity;
//    }

//    public static float GetHalf(float know)
//    {
//        return (1 + know) * 0.5f;
//    }

//    public static float GetNegative(float know)
//    {
//        return 2 - know;
//    }

//    public static float GetAdditional(float know)
//    {
//        return know - 1;
//    }
//}

[Serializable]
public class UnitData : ICloneable
{
    public string name;
    public int id;
    public int cost;
    public int stage;
    public int weapon;
    public Stat stat;
    public RuntimeAnimatorController controller;
    public Color color;
    public bool isFriendly;
    public bool isStanding;
    public MOVE_TYPE move;
    public float size;
    public float moveDelay;
    public float moveDistance;
    public float moveSpeed;
    public Vector2 shadowPos;
    public EffectData[] effects;

    public object Clone()
    {
        UnitData clone = (UnitData)this.MemberwiseClone();

        clone.stat = (Stat)stat.Clone();
        return clone;
    }
}

//[Serializable]
//public struct Weapon
//{
//    public string name;
//    public int id;
//    public int level;
//    public int[] startBulletId;
//    public Vector2 spawnPoint;
//    public ATTACK_TYPE type;
//    public float beforeAttackDelay;
//    public float afterAttackDelay;
//    public float localSpeed;
//    public float worldSpeed;

//    public Weapon(Weapon w)
//    {
//        this = w;
//        startBulletId = (int[])w.startBulletId.Clone();
//    }

//    public Weapon(int id, int level):this(GameDatabase.instance.weapons[id])
//    {
//        this.level = level;
//    }
//}

[Serializable]
public struct ShakeData
{
    public float time;
    public float power;
    public float gap;
    public bool isOnHit;
}

public enum SORTING_LAYER
{
    TOP, MID, BOT
}

[Serializable]
public class WeaponData : ICloneable
{
    public string name;
    public int id;
    public ATTACK_TYPE type;
    public int startBulletId;
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
    public RuntimeAnimatorController controller;
    public BULLET_TYPE type;
    public float localSpeed;
    public float worldSpeed;
    public float localAccel;
    public float worldAccel;
    public float size;
    public float delay;
    public float limitTime;
    public int pierce;
    public ShakeData shake;
    public EffectData[] effects;

    public BulletChildData[] children;
    public BulletChildData[] onDestroy;
    
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
public struct BulletChildData : ICloneable
{
    public string name;
    public int bulletId;
    public int sortingOrder;
    public float startTime;
    public float waitTime;
    public float angle;

    public bool isRepeat;
    public bool isFirst;//반복의 첫 번째는 startTime 무시
    public bool isStick;
    public bool isDestroyWith;

    public Vector3 offset;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public enum BULLET_TYPE
{
    CIRCLECAST, RAYCAST, TRIANGLE, SECTOR, NONE, DYNAMIC
}

/// <summary>
/// Bullet Ability
/// 중력, 자전, 관통, 시간파괴, 유도, 주인 죽으면 파괴
/// </summary>
[Serializable]
public enum ABILITY
{
    GRAVITY, SPIN, PIERCE, TIME, GUIDE, OWNER
}

[Serializable]
public struct Ability : ICloneable
{
    public ABILITY ability;
    public float value;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

//[Serializable]
//public struct ArmorData
//{
//    public string name;
//    public int level;
//    public float def;
//    public Effect effect;
//}

[Serializable]
enum SKILL_ID
{ ThunderStrike, IceBreak, Genesis, BloodSwamp, ScarecrowSoldier }

[Serializable]
public class SkillData:ICloneable
{
    public string name;
    public int[] bulletIds;
    public int[] unitIds;
    public Sprite spr;
    public int id;
    public int level;
    public float coolTime;
    public float coolTimeLeft;
    public float manaCost;
    public float size;
    public bool isCircleToPlayer;
    public bool isDeath;
    public EffectData[] effects;
    public ValueData[] values;
    public SkillUpData levelUp;
    [TextArea]
    public string description;

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

    public string desc
    {
        get
        {
            {
                SKILL_ID _id = (SKILL_ID)id;
                if (id >= GameDatabase.instance.skills.Length)
                    return string.Empty;
                SkillData data = GameDatabase.instance.skills[id];
                string des = data.description;
                
                float tec = BoardManager.instance.player.data.stat.GetCurrent(STAT.TEC);
                switch (_id)
                {
                    case SKILL_ID.ThunderStrike:
                        return string.Format(des, data.values[0].value, string.Format("{0:##0.##}({1:##0.##}TEC*0.1)", tec * 0.1f, tec));
                }
                return des;
            }
        }
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
public enum SKILL_VALUE
{
    AMOUNT, TIME
}

[Serializable]
public class SkillUpData : ICloneable
{
    public float manaCost;
    public float size;
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
    public SKILL_VALUE name;
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
public enum EFFECT
{
    STUN, SLOW, FIRE, ICE, KNOCKBACK, POISON, HEAL, LIFESTEAL
}

[Serializable]
public struct ItemData
{
    public string name;
    public int id;
    public float value;
    public float size;
    [TextArea]
    public string description;

    public string GetDescription()
    {
        float temp = 0;
        switch (id)
        {
            case 0://HealPotion
                //temp = Player.instance.data.stat.tec;
                return string.Format(description, temp, temp);
            case 1://ManaPotion
                //temp = Player.instance.data.stat.tec;
                return string.Format(description, temp, temp);
            default:
                return description;
        }
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
public enum ATTACK_TYPE
{
    STOP_BEFORE, STOP_AFTER, DONT_STOP
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
}