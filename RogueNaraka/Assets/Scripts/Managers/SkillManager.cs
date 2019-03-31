using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SkillScripts;
using TMPro;

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
    /// 스킬 장착
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

    //#region SkillChangeParams

    //public GameObject changePnl;
    //public TextMeshProUGUI changeCostTxt;
    //public TextMeshProUGUI lastChangeCostTxt;
    //public TextMeshProUGUI levelTxt;

    //SkillData data;

    //bool isChange;
    //bool isCancel;
    //#endregion

    ///// <summary>
    ///// 스킬을 교체하는 패널을 여는 함수
    ///// </summary>
    ///// <param name="data">교체하는 스킬</param>
    ///// <param name="position">교체될 슬롯</param>
    //public void OpenChangePnl(SkillData data, int position)
    //{
    //    StartCoroutine(ChangeCorou(data, position));
    //}

    ///// <summary>
    ///// 피보나치 수열
    ///// </summary>
    ///// <param name="level">스킬 레벨</param>
    ///// <returns></returns>
    //public int GetChangeCost(int level)
    //{
    //    switch(level)
    //    {
    //        case 0: case 1:
    //            return 0;
    //        case 2:
    //            return 10;
    //        case 3:
    //            return 20;
    //        default:
    //            return GetChangeCost(level - 2) + GetChangeCost(level - 1);
    //    }
    //}

    //IEnumerator ChangeCorou(SkillData data, int position)
    //{
    //    Debug.Log("ChangeCorou");
    //    int level = skills[position].skill.data.level;
    //    int cost = GetChangeCost(level);
    //    int lastCost = cost;
    //    while (!MoneyManager.instance.IsUseable(cost) && level > 1)
    //    {
    //        lastCost = cost;
    //        cost = GetChangeCost(--level);
    //    }

    //    changeCostTxt.text = string.Format("{0}Soul", cost);
    //    lastChangeCostTxt.text = string.Format("{0}Soul", cost);

    //    do//Wait for btn action
    //    {
    //        yield return null;
    //    } while (!isChange && !isCancel);

    //    if(isChange)
    //    {
    //        data = (SkillData)data.Clone();
    //        data.level = level;
    //        skills[position].Init(data);
    //    }

    //    RollManager.instance.SetRollPnl(false, RollManager.instance.isStageUp);

    //    changePnl.SetActive(false);

    //    //Reset
    //    isChange = false;
    //    isCancel = false;
    //}

    //public void Change()
    //{
    //    isChange = true;
    //}

    //public void Cancel()
    //{
    //    isCancel = true;
    //}

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
