using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    public GameObject debugPnl;
    public GameObject debugBtn;

#if UNITY_EDITOR
    private void Awake()
    {
        debugBtn.SetActive(true);
    }
#endif

    public void KillPlayer()
    {
        if(BoardManager.instance.player)
            BoardManager.instance.player.Kill();
        Debug.Log("Kill Player");
    }

    public void KillEnemies()
    {
        for (int i = 0; i < BoardManager.instance.enemies.Count; i++)
        {
            BoardManager.instance.enemies[i].Kill();
        }
        Debug.Log("Kill Enemies");
    }

    public void SetStage(int stage)
    {
        PlayerPrefs.SetInt("stage", stage);
        BoardManager.instance.SetStage(stage);
        BoardManager.instance.InitBoard();
        //FadeManager.instance.FadeOut(0.5f, true);
        Debug.Log("SetStage:" + stage);
    }

    public void ReadStageText(TMP_InputField input)
    {
        int result;
        if(int.TryParse(input.text, out result))
        {
            SetStage(result);
        }
    }

    public void SetSkill(int slot, int id)
    {
        SkillManager.instance.SetSkill(id, slot);
        Debug.Log("SetSkill:slot=" + slot + " id=" + id);
    }

    public TMP_InputField[] skillInputs;

    public void ReadSkill()
    {
        int slot;
        int.TryParse(skillInputs[0].text, out slot);

        if (slot >= 0 && slot <= 2)
        {
            int id;
            if (int.TryParse(skillInputs[1].text, out id))
                SetSkill(slot, id);
        }
        else
            Debug.Log("SetSkill ERROR:ID isn't 0~2");
    }

    public void SkillLevelUp(int slot, int amount)
    {
        SkillManager.instance.skills[slot].LevelUp(amount);
        Debug.Log("SkillLevelUp:slot=" + slot + " amount=" + amount);
    }

    public void ReadSkillLevelUp()
    {
        int slot;
        int.TryParse(skillInputs[2].text, out slot);

        if (slot >= 0 && slot <= 2)
        {
            int amount;
            if (int.TryParse(skillInputs[3].text, out amount))
                SkillLevelUp(slot, amount);
        }
        else
            Debug.Log("SetSkill ERROR:ID isn't 0~2");
    }

    public void AddSoul(int amount)
    {
        MoneyManager.instance.AddSoul(amount);
    }

    public void ReadSoul(TMP_InputField input)
    {
        int amount;
        if(int.TryParse(input.text, out amount))
        {
            AddSoul(amount);
        }
    }

    public void GodMode()
    {
        BoardManager.instance.player.hpable.isInvincible = !BoardManager.instance.player.hpable.isInvincible;
        Debug.Log("GodMode");
    }

    public void SetStat(STAT type, float value)
    {
        BoardManager.instance.player.stat.SetOrigin(type, value);
        Debug.Log("SetStat:type=" + type + "value=" + value);
    }

    public TMP_InputField[] statInput;
    public void ReadSetStat()
    {
        int typeNum;
        float value;
        if (int.TryParse(statInput[0].text, out typeNum) && float.TryParse(statInput[1].text, out value))
        {
            if (typeNum >= 0 && typeNum < 7)
            {
                STAT type = (STAT)typeNum;
                SetStat(type, value);
                GameManager.instance.StatTextUpdate();
            }
        }
    }

    public void SpawnBoss(int id)
    {
        BoardManager.instance.SpawnBoss(id);
    }

    public void ReadSpawnBoss(TMP_InputField bossInput)
    {
        int id;
        int.TryParse(bossInput.text, out id);
        SpawnBoss(id);
    }

    public void ReadSetItem(TMP_InputField input)
    {
        int id;
        if (int.TryParse(input.text, out id))
        {
            Item.instance.EquipItem(id);
        }
    }

    public void SetDebugPnl(bool value)
    {
        debugPnl.SetActive(true);
    }

    int requestReset;

    public GameObject resetPnl;

    public void RequestReset()
    {
        if(++requestReset > 10)
        {
            resetPnl.SetActive(true);
        }
    }

    public void DataReset()
    {
        PlayerPrefs.DeleteAll();
        resetPnl.SetActive(false);
    }
}