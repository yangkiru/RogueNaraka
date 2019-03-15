using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class Fillable : MonoBehaviour
{
    public float current = 100;
    public float goal = 100;
    public Image img;
    public Unit unit;
    public TYPE type;
    public FILLABLE unitType;

    public static Fillable bossHp;

    private float t = 1;

    public enum TYPE { HEALTH, MANA }

    private void Awake()
    {
        if(unitType == FILLABLE.BOSS)
        {
            bossHp = this;
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        unit = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (unit || current != goal)
        {
            if (unit)
            {
                if (unit.deathable.isDeath)
                {
                    goal = 0;
                    unit = null;
                }
                else
                {
                    try
                    {
                        switch (type)
                        {
                            case TYPE.HEALTH: goal = unit.hpable.currentHp / unit.hpable.maxHp; break;
                            case TYPE.MANA: goal = unit.mpable.currentMp / unit.mpable.maxMp; break;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (float.IsNaN(current))
            {
                switch (type)
                {
                    case TYPE.HEALTH: current = unit.hpable.currentHp; break;
                    case TYPE.MANA: current = unit.mpable.currentMp; break;
                }
            }
            
            t += Time.deltaTime;
            if (t > 1)
                t = 1;
            if (current == goal)
                t = 0;
            current = Mathf.Lerp(current, goal, t);
            img.fillAmount = current;
        }
        else
        {
            switch (unitType)
            {
                case FILLABLE.PLAYER:
                    unit = BoardManager.instance.player;
                    break;
                case FILLABLE.BOSS:
                    unit = BoardManager.instance.boss;
                    break;
            }
            if (unit && unit.deathable.isDeath)
                unit = null;
        }
    }
}

    [System.Serializable]
    public enum FILLABLE
    { PLAYER, BOSS }
