﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

#if DEBUG
    public bool reset;
    public bool setStage;
    public int stage;
    public bool killPlayer;
    public bool killEnemies;
    public bool setShopStage;
    public int shopStage;
    public bool levelUp;
    public int addSoul;
    public bool resetSkillBought;
    private void OnValidate()
    {
        if (reset)
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Reset");
            reset = false;
        }
        if (setStage)
        {
            setStage = false;
            stage = 0;
            PlayerPrefs.SetInt("stage", stage);
        }
        if (killPlayer && BoardManager.instance && BoardManager.instance.player)
        {
            BoardManager.instance.player.Kill();
            Debug.Log("Kill Player");
            killPlayer = false;
        }
        if (killEnemies && BoardManager.instance && BoardManager.instance.enemies.Count > 0)
        {
            for (int i = 0; i < BoardManager.instance.enemies.Count; i++)
            {
                BoardManager.instance.enemies[i].Kill();
            }
            Debug.Log("Kill Enemies");
            killEnemies = false;
        }
        if(setShopStage)
        {
            PlayerPrefs.SetInt("shopStage", shopStage);
            setShopStage = false;
            shopStage = 0;
        }
        if(levelUp)
        {
            PlayerPrefs.SetInt("isLevelUp", 1);
            levelUp = false;
        }
        if(addSoul != 0)
        {
            if (MoneyManager.instance) MoneyManager.instance.AddSoul(addSoul);
            else
                PlayerPrefs.SetInt("soul", PlayerPrefs.GetInt("soul") + addSoul);
            addSoul = 0;
        }
        if(resetSkillBought)
        {
            PlayerPrefs.SetString("boughtSkills", string.Empty);
            resetSkillBought = false;
        }
    }
#endif
}