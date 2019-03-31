using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SkillScripts;

public class SkillManager : MonoBehaviour {

    public GameManager gameManager
    { get { return GameManager.instance; } }
    public RectTransform[] points;
    public CircleRenderer circle;

    public SkillGUI[] skills;

    public static SkillManager instance = null;

    private void Awake()
    {
        if (instance != this)
            instance = this;
    }

    public SkillData GetSkillData(int id)
    {
        try
        {
            return GameDatabase.instance.skills[id];
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// 스킬 설정
    /// </summary>
    /// <param name="data"></param>
    /// <param name="position"></param>
    public void SetSkill(SkillData data, int position)
    {
        if (skills[position].skill == null || data.id != skills[position].skill.data.id)
        {
            AudioManager.instance.PlaySFX("skillEquip");
            skills[position].Init(data);
        }
        else
        {
            AudioManager.instance.PlaySFX("skillLevelUp");
            skills[position].LevelUp(1);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sc">showCaseId</param>
    /// <param name="position"></param>
    public void SetSkill(int id, int position)
    {
        SetSkill(GetSkillData(id), position);
    }

    public void InitSkills()
    {
        for (int i = 0; i < 3; i++)
        {
            skills[i].ResetSkill();
        }
    }

    /// <summary>
    /// Skill Save
    /// </summary>
    public void Save()
    {
        SkillSaveData[] datas = new SkillSaveData[3];
        for(int i = 0; i < 3; i++)
        {
            if (skills[i].skill)
            {
                datas[i] = SkillSaveData.SkillToSave(skills[i].skill.data);
                Debug.Log("Skill Saved" + datas[i].id + "id " + datas[i].level + "level");
            }
            else
            {
                datas[i] = new SkillSaveData();
                datas[i].id = -1;
            }
        }
        string str = JsonHelper.ToJson<SkillSaveData>(datas);
        PlayerPrefs.SetString("skill", str);
        //Debug.Log("Save Skills : " + str);
    }

    /// <summary>
    /// Skill Load
    /// </summary>
    public void Load()
    {
        string str = PlayerPrefs.GetString("skill");
        SkillSaveData[] datas = JsonHelper.FromJson<SkillSaveData>(str);
        Debug.Log("SkillSaveData : " + str);
  
        if (datas == null)
        {
            for(int i = 0; i < 3; i++)
            {
                skills[i].ResetSkill();
            }
            return;
        }
            

        for (int i = 0; i < 3; i++)
        {
            //Debug.Log(datas[i].id);
            if (datas[i].id == -1)
                skills[i].ResetSkill();
            else
            {
                if (GameDatabase.instance.skills.Length > datas[i].id)
                {
                    Debug.Log("Skill Loaded" + datas[i].id + "id " + datas[i].level + "level");
                    SkillData skill = (SkillData)GameDatabase.instance.skills[datas[i].id].Clone();
                    skill.coolTimeLeft = datas[i].coolTimeLeft;
                    skills[i].Init(skill);
                    skills[i].LevelUp(datas[i].level - 1);
                }
                else
                    Debug.Log(string.Format("SkillMissing:id too big{0}", datas[i].id));
            }
        }
        //Debug.Log("Load Skills : " + str);
    }

    /// <summary>
    /// Reset Skill
    /// </summary>
    public void ResetSave()
    {
        for(int i = 0; i < 3; i++)
        {
            skills[i].ResetSkill();
        }
        //ResetShowCase();
        Save();
    }
}
