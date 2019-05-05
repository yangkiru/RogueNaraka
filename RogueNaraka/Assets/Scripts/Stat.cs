using RogueNaraka.UnitScripts;
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

    public float dmgTemp;
    public float spdTemp;
    public float tecTemp;
    public float hpTemp;
    public float mpTemp;
    public float hpRegenTemp;
    public float mpRegenTemp;

    public float currentHp;
    public float currentMp;

    public int statPoints;

    public float sumOrigin
    {
        get { return dmg + spd + tec + hp + mp + hpRegen + mpRegen; }
    }

    public float sumMax
    {
        get { return dmgMax + spdMax + tecMax + hpMax + mpMax + hpRegenMax + mpRegenMax; }
    }

    public float sumTemp
    {
        get { return dmgTemp + spdTemp + tecTemp + hpTemp + mpTemp + hpRegenTemp + mpRegenTemp; }
    }

    public bool AddOrigin(STAT type, float amount, bool isCheck = false, bool isIgnoreMax = false)
    {
        switch(type)
        {
            case STAT.DMG:
                if (!isIgnoreMax && dmg + amount > dmgMax)
                    return false;
                if(!isCheck)
                    dmg += amount;
                return true;
            case STAT.SPD:
                if (!isIgnoreMax && spd + amount > spdMax)
                    return false;
                if (!isCheck)
                    spd += amount;
                return true;
            case STAT.TEC:
                if (!isIgnoreMax && tec + amount > tecMax)
                    return false;
                if (!isCheck)
                    tec += amount;
                return true;
            case STAT.HP:
                if (!isIgnoreMax && hp + amount > hpMax)
                    return false;
                if (!isCheck)
                    hp += amount;
                return true;
            case STAT.MP:
                if (!isIgnoreMax && mp + amount > mpMax)
                    return false;
                if (!isCheck)
                    mp += amount;
                return true;
            case STAT.HR:
                if (!isIgnoreMax && hpRegen + amount > hpRegenMax)
                    return false;
                if (!isCheck)
                    hpRegen += amount;
                return true;
            case STAT.MR:
                if (!isIgnoreMax && mpRegen + amount > mpRegenMax)
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
            case STAT.HR:
                hpRegenMax += amount;
                break;
            case STAT.MR:
                mpRegenMax += amount;
                break;
            case STAT.SP:
                statPoints += (int)amount;
                break;
        }
    }

    public void AddTemp(STAT type, float amount)
    {
        switch (type)
        {
            case STAT.DMG:
                dmgTemp += amount;
                break;
            case STAT.SPD:
                spdTemp += amount;
                break;
            case STAT.TEC:
                tecTemp += amount;
                break;
            case STAT.HP:
                hpTemp += amount;
                break;
            case STAT.MP:
                mpTemp += amount;
                break;
            case STAT.HR:
                hpRegenTemp += amount;
                break;
            case STAT.MR:
                mpRegenTemp += amount;
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

    public void SetOrigin(STAT type, float value)
    {
        switch (type)
        {
            case STAT.DMG:
                dmg = value;
                break;
            case STAT.SPD:
                spd = value;
                break;
            case STAT.TEC:
                tec = value;
                break;
            case STAT.HP:
                hp = value;
                break;
            case STAT.MP:
                mp = value;
                break;
            case STAT.HR:
                hpRegen = value;
                break;
            case STAT.MR:
                mpRegen = value;
                break;
        }
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
            case STAT.HR:
                return hpRegen;
            case STAT.MR:
                return mpRegen;
            case STAT.SP:
                return statPoints;
        }
        return -1;
    }

    public float GetOrigin(int type)
    {
        switch (type)
        {
            case 0:
                return dmg;
            case 1:
                return spd;
            case 2:
                return tec;
            case 3:
                return hp;
            case 4:
                return mp;
            case 5:
                return hpRegen;
            case 6:
                return mpRegen;
            case 7:
                return statPoints;
        }
        return -1;
    }

    public float GetCurrent(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return dmg + dmgTemp;
            case STAT.SPD:
                return spd + spdTemp;
            case STAT.TEC:
                return tec + tecTemp;
            case STAT.HP:
                return hp + hpTemp;
            case STAT.MP:
                return mp + mpTemp;
            case STAT.HR:
                return hpRegen + hpRegenTemp;
            case STAT.MR:
                return mpRegen + mpRegenTemp;
            case STAT.SP:
                return statPoints;
        }
        return -1;
    }
    
    public float GetCurrent(int type)
    {
        switch(type)
        {
            case 0:
                return dmg + dmgTemp;
            case 1:
                return spd + spdTemp;
            case 2:
                return tec + tecTemp;
            case 3:
                return hp + hpTemp;
            case 4:
                return mp + mpTemp;
            case 5:
                return hpRegen + hpRegenTemp;
            case 6:
                return mpRegen + mpRegenTemp;
            case 7:
                return statPoints;
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
            case STAT.HR:
                return hpRegenMax;
            case STAT.MR:
                return mpRegenMax;
            case STAT.SP:
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
        if(stat == null)
            PlayerPrefs.SetString(str, string.Empty);
        else
            PlayerPrefs.SetString(str, Stat.StatToJson(stat));
    }

    public static Stat DataToStat(string str = "stat")
    {
        string json = PlayerPrefs.GetString(str);
        if (json == string.Empty)
            return null;
        else
            return JsonToStat(json);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

[Serializable]
public enum STAT
{
    DMG, SPD, TEC, HP, HR, MP, MR, SP
}
