using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat : ICloneable
{

    public float dmg;
    public float spd;
    public float tec;
    public float hp;
    public float mp;
    public float hpRegen;
    public float mpRegen;

    public float dmgMax;
    public float spdMax;
    public float tecMax;
    public float hpMax;
    public float mpMax;
    public float hpRegenMax;
    public float mpRegenMax;

    public float tmpDmg;
    public float tmpSpd;
    public float tmpTec;
    public float tmpHp;
    public float tmpMp;
    public float tmpHpRegen;
    public float tmpMpRegen;

    public float currentHp;
    public float currentMp;

    public int statPoints;

    public void Init()
    {
        tmpDmg = 0;
        tmpSpd = 0;
        tmpTec = 0;
        tmpHp = 0;
        tmpMp = 0;
        tmpHpRegen = 0;
        tmpMpRegen = 0;
    }

    public float sumOrigin
    {
        get { return dmg + spd + tec + hp + mp + hpRegen + mpRegen; }
    }

    public float sumMax
    {
        get { return dmgMax + spdMax + tecMax + hpMax + mpMax + hpRegenMax + mpRegenMax; }
    }

    public bool AddOrigin(STAT type, float amount, bool isCheck = false)
    {
        switch(type)
        {
            case STAT.DMG:
                if (dmg + amount > dmgMax)
                    return false;
                if(!isCheck)
                    dmg += amount;
                return true;
            case STAT.SPD:
                if (spd + amount > spdMax)
                    return false;
                if (!isCheck)
                    spd += amount;
                return true;
            case STAT.TEC:
                if (tec + amount > tecMax)
                    return false;
                if (!isCheck)
                    tec += amount;
                return true;
            case STAT.HP:
                if (hp + amount > hpMax)
                    return false;
                if (!isCheck)
                    hp += amount;
                return true;
            case STAT.MP:
                if (mp + amount > mpMax)
                    return false;
                if (!isCheck)
                    mp += amount;
                return true;
            case STAT.HPREGEN:
                if (hpRegen + amount > hpRegenMax)
                    return false;
                if (!isCheck)
                    hpRegen += amount;
                return true;
            case STAT.MPREGEN:
                if (mpRegen + amount > mpRegenMax)
                    return false;
                if (!isCheck)
                    mpRegen += amount;
                return true;
        }
        return false;
    }

    public void AddMax(STAT type, float amount)
    {
        switch (type)
        {
            case STAT.DMG:
                dmgMax += amount;
                break;
            case STAT.SPD:
                spdMax += amount;
                break;
            case STAT.TEC:
                tecMax += amount;
                break;
            case STAT.HP:
                hpMax += amount;
                break;
            case STAT.MP:
                mpMax += amount;
                break;
            case STAT.HPREGEN:
                hpRegenMax += amount;
                break;
            case STAT.MPREGEN:
                mpRegenMax += amount;
                break;
            case STAT.STATPOINT:
                statPoints += (int)amount;
                break;
        }
    }

    public void SetOrigin(Stat s)
    {
        dmg = s.dmg;
        spd = s.spd;
        tec = s.tec;
        hp = s.hp;
        mp = s.mp;
        hpRegen = s.hpRegen;
        mpRegen = s.mpRegen;
    }

    public void SetMax(Stat s)
    {
        dmgMax = s.dmgMax;
        spdMax = s.spdMax;
        tecMax = s.tecMax;
        hpMax = s.hpMax;
        mpMax = s.mpMax;
        hpRegenMax = s.hpRegenMax;
        mpRegenMax = s.mpRegenMax;
    }

    public float GetOrigin(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return dmg;
            case STAT.SPD:
                return spd;
            case STAT.TEC:
                return tec;
            case STAT.HP:
                return hp;
            case STAT.MP:
                return mp;
            case STAT.HPREGEN:
                return hpRegen;
            case STAT.MPREGEN:
                return mpRegen;
        }
        return -1;
    }

    public float GetCurrent(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return dmg + tmpDmg;
            case STAT.SPD:
                return spd + tmpSpd;
            case STAT.TEC:
                return tec + tmpTec;
            case STAT.HP:
                return hp + tmpHp;
            case STAT.MP:
                return mp + tmpMp;
            case STAT.HPREGEN:
                return hpRegen + tmpHpRegen;
            case STAT.MPREGEN:
                return mpRegen + tmpMpRegen;
        }
        return -1;
    }

    public float GetMax(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return dmgMax;
            case STAT.SPD:
                return spdMax;
            case STAT.TEC:
                return tecMax;
            case STAT.HP:
                return hpMax;
            case STAT.MP:
                return mpMax;
            case STAT.HPREGEN:
                return hpRegenMax;
            case STAT.MPREGEN:
                return mpRegenMax;
            case STAT.STATPOINT:
                return statPoints;
        }
        return -1;
    }

    public static Stat JsonToStat(string stat)
    {
        return JsonUtility.FromJson<Stat>(stat);
    }

    public static string StatToJson(Stat stat)
    {
        return JsonUtility.ToJson(stat);
    }

    public static void StatToData(Stat stat, string str = "stat")
    {
        PlayerPrefs.SetString(str, Stat.StatToJson(stat));
    }

    public static Stat DataToStat(string str = "stat")
    {
        return JsonToStat(PlayerPrefs.GetString(str));
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public enum STAT
{
    DMG, SPD, TEC, HP, HPREGEN, MP, MPREGEN, STATPOINT
}
