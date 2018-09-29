using System;
using System.Collections.Generic;
using UnityEngine;

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
    public Stat playerBase;
    public UnitData[] enemies;
    public UnitData[] bosses;
    public LayerMask playerMask;
    public LayerMask unitMask;
    public LayerMask friendlyMask;
    public LayerMask enemyMask;
    public LayerMask wallMask;
    public BulletData[] bullets;
    public Weapon[] weapons;
    public int[] stageCosts;
    public UnitCost[] unitCosts;
    public SkillData[] skills;
    public ItemData[] items;
    public ItemSpriteData[] itemSprites;

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

    [ContextMenu("SyncWeapon")]
    public void SyncWeapon()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].weapon = weapons[enemies[i].weaponId];
            enemies[i].weapon.level = enemies[i].weaponLevel;
        }
    }
}

[Serializable]
public struct UnitCost
{
    public int cost;
    public int[] unitId;
}

[Serializable]
public struct Stat
{
    public float dmg;
    public float spd;
    public float luck;
    public float hp;
    public float mp;
    public float hpRegen;
    public float mpRegen;

    public Stat(Stat s)
    {
        dmg = s.dmg; spd = s.spd; luck = s.luck; hp = s.hp; mp = s.mp; hpRegen = s.hpRegen; mpRegen = s.mpRegen;
    }
    public Stat(float value)
    {
        dmg = value; spd = value; luck = value; hp = value; mp = value; hpRegen = value; mpRegen = value;
    }
    public Stat(float d, float s, float l, float h, float m, float hr, float mr)
    {
        dmg = d; spd = s; luck = l; hp = h; mp = m; hpRegen = hr; mpRegen = mr;
    }
    public float sum
    {
        get { return dmg + spd + luck + hp + mp + hpRegen + mpRegen; }
    }
}

[Serializable]
public struct UnitData
{
    public string name;
    public int id;
    public int cost;
    public int stage;
    public Stat stat;
    public RuntimeAnimatorController controller;
    public Color color;
    public int weaponId;
    public int weaponLevel;
    public Weapon weapon;
    public bool isFriendly;
    public MOVE_TYPE move;
    public float moveTime;
    public float moveDistance;
    public float minDistance;
    public float maxDistance;
}

[Serializable]
public struct Weapon
{
    public string name;
    public int level;
    public int[] startBulletId;
    public Vector2 spawnPoint;
    public ATTACK_TYPE type;
    public float localSpeed;
    public float worldSpeed;

    public Weapon(Weapon w)
    {
        name = w.name; level = w.level; startBulletId = (int[])w.startBulletId.Clone(); spawnPoint = w.spawnPoint;
        type = w.type; localSpeed = w.localSpeed; worldSpeed = w.worldSpeed;
    }
}

[Serializable]
public struct BulletData
{
    public string name;
    public int id;
    public BULLET_TYPE type;
    public float dmg;
    public float dealSpeed;
    public float size;
    public float angle; //TRIANGLE, SECTOR 에서 타격 범위
    public RuntimeAnimatorController controller;
    public Color color;
    public Ability[] abilities;
    public BulletChild[] children;
    public BulletChild[] onDestroy;
    public EffectData[] effects;
}

[Serializable]
public enum BULLET_TYPE
{
    CIRCLECAST, CIRCLEOVERLAP, LINECAST, LINECASTS, TRIANGLE, NONE, SECTOR
}

/// <summary>
/// Bullet Ability
/// </summary>
[Serializable]
public enum ABILITY
{
    GRAVITY, SPIN, PIERCE, TIME, GUIDE
}

[Serializable]
public struct Ability
{
    public ABILITY ability;
    public float value;
}

[Serializable]
public struct BulletChild
{
    public string name;
    public int bulletId;
    public int sortingOrder;
    public float startTime;
    public float waitTime;
    public float angle;
    public float localSpeed;
    public float worldSpeed;
    //부모를 기준으로 공전, RevolveHolder 사용x, 각자 회전
    public float revolveDistance;
    public float revolveSpeed;
    //
    public bool isRevolveTarget;//부모가 공전중인 타겟을 기준으로 공전, RevolveHolder 사용
    public bool isRepeat;
    public bool isFirst;//반복의 첫 번째는 startTime 무시
    public bool isStick;
    public bool isEndWith;
    
    public Vector2 spawnPoint;
}

[Serializable]
public struct ArmorData
{
    public string name;
    public int level;
    public float def;
    public Effect effect;
}

[Serializable]
public struct SkillData
{
    public string name;
    public Sprite spr;
    public int id;
    public int level;
    public float coolTime;
    public float coolTimeLeft;
    public float manaCost;
    public float distance;
    public bool isCircleToPlayer;
    public bool isDeath;
    public EffectData[] effects;
    public ValueData[] values;
    public SkillUpData levelUp;

    public void Reset()
    {
        name = string.Empty; spr = null; id = -1; level = 1; coolTime = 0; coolTimeLeft = 0; manaCost = 0; effects = new EffectData[0]; values = new ValueData[0];
    }
}

[Serializable]
public struct SkillUpData
{
    public float coolTime;
    public float manaCost;
    public float distance;
    public EffectData[] effects;
    public ValueData[] values;
}

[Serializable]
public struct ValueData
{
    public string name;
    public float value;
}

[Serializable]
public class EffectData
{
    public EFFECT type;
    public float value;
    public float time;
    public bool isInfinity;

    public EffectData()
    {
    }

    public EffectData(EFFECT e, float v, float t, bool i = false)
    {
        type = e; value = v; time = t; isInfinity = i;
    }

    public EffectData(EffectData ef) : this(ef.type, ef.value, ef.time, ef.isInfinity)
    {
    }
}

[Serializable]
public enum EFFECT
{
    STUN, SLOW, FIRE, ICE, KNOCKBACK
}

[Serializable]
public struct ItemData
{
    public string name;
    public int id;
    public float value;
    public float size;
    public int amount;
}

[Serializable]
public struct ItemSpriteData
{
    public int id;
    public Sprite spr;
}

[Serializable]
public enum STAT
{
    DMG, SPD, LUCK, HP, MP, HPREGEN, MPREGEN, STATPOINT
}

[Serializable]
public enum ATTACK_TYPE
{
    CLOSE, TARGET, NONTARGET, REVOLVE
}

[Serializable]
public enum MOVE_TYPE
{
    RUSH,//근접 공격
    STATUE,//원거리 고정
    DISTANCE,//원거리 도망 및 공격
    RUN,//도망 공격 X
}