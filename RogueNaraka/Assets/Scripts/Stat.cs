using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float sum
    {
        get { return dmg + spd + tec + hp + mp + hpRegen + mpRegen; }
    }

    public bool Add(STAT type, float amount)
    {
        switch(type)
        {
            case STAT.DMG:
                if (dmg + amount > dmgMax)
                    return false;
                dmg += amount;
                return true;
            case STAT.SPD:
                if (spd + amount > spdMax)
                    return false;
                spd += amount;
                return true;
        }
    }

    public static Stat JsonToStat(string stat)
    {
        return JsonUtility.FromJson<Stat>(stat);
    }

    public static string StatToJson(Stat stat)
    {
        return JsonUtility.ToJson(stat);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    [Serializable]
    public enum STAT
    {
        DMG, SPD, TEC, HP, MP, HPREGEN, MPREGEN, STATPOINT
    }
}
