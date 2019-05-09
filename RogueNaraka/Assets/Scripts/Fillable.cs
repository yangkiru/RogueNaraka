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
    public static Fillable playerHp;
    public static Fillable playerMp;

    private float t = 1;

    public enum TYPE { HEALTH, MANA }

    private void Awake()
    {
        switch(unitType)
        {
            case FILLABLE.BOSS:
                bossHp = this;
                gameObject.SetActive(false);
                break;
            case FILLABLE.PLAYER:
                if (type == TYPE.HEALTH)
                    playerHp = this;
                else
                    playerMp = this;
                break;
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
                    switch (type)
                    {
                        case TYPE.HEALTH: goal = unit.hpable.maxHp == 0 ? 0 : unit.hpable.currentHp / unit.hpable.maxHp; break;
                        case TYPE.MANA: goal = unit.mpable.maxMp == 0 ? 0 : unit.mpable.currentMp / unit.mpable.maxMp; break;
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
                    unit = BoardManager.instance ? BoardManager.instance.player : null;
                    break;
                case FILLABLE.BOSS:
                    unit = BoardManager.instance ? BoardManager.instance.boss : null;
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
