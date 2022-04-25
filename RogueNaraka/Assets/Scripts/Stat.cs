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
    public float hp;
    public float mp;

    public float dmgMax;
    public float spdMax;
    public float hpMax;
    public float mpMax;

    public float dmgTemp;
    public float spdTemp;
    public float hpTemp;
    public float mpTemp;
    public float hpRegenTemp;
    public float mpRegenTemp;

    public float currentHp;
    public float currentMp;

    public int statPoints;

    public float sumOrigin
    {
        get { return dmg + spd + hp + mp; }
    }

    public float sumMax
    {
        get { return dmgMax + spdMax + hpMax + mpMax; }
    }

    public float sumTemp
    {
        get { return dmgTemp + spdTemp + hpTemp + mpTemp; }
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
            case STAT.HP:
                hpMax += amount;
                break;
            case STAT.MP:
                mpMax += amount;
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
            case STAT.HP:
                hpTemp += amount;
                break;
            case STAT.MP:
                mpTemp += amount;
                break;
        }
    }

    public void SetOrigin(Stat s)
    {
        dmg = s.dmg;
        spd = s.spd;
        hp = s.hp;
        mp = s.mp;
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
            case STAT.HP:
                hp = value;
                break;
            case STAT.MP:
                mp = value;
                break;
        }
    }

    public void SetMax(Stat s)
    {
        dmgMax = s.dmgMax;
        spdMax = s.spdMax;
        hpMax = s.hpMax;
        mpMax = s.mpMax;
    }

    public float GetOrigin(STAT type)
    {
        switch (type)
        {
            case STAT.DMG:
                return dmg;
            case STAT.SPD:
                return spd;
            case STAT.HP:
                return hp;
            case STAT.MP:
                return mp;
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
                return hp;
            case 3:
                return mp;
            case 4:
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
            case STAT.HP:
                return hp + hpTemp;
            case STAT.MP:
                return mp + mpTemp;
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
                return hp + hpTemp;
            case 3:
                return mp + mpTemp;
            case 4:
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
            case STAT.HP:
                return hpMax;
            case STAT.MP:
                return mpMax;
            case STAT.SP:
                return statPoints;
        }
        return -1;
    }
    public float GetMax(int type)
    {
        switch ((STAT)type)
        {
            case STAT.DMG:
                return dmgMax;
            case STAT.SPD:
                return spdMax;
            case STAT.HP:
                return hpMax;
            case STAT.MP:
                return mpMax;
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
    DMG, SPD, HP, MP, SP
}
