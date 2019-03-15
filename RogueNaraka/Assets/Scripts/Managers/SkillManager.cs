using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.SkillScripts;

public class SkillManager : MonoBehaviour {

    public GameManager gameManager
    { get { return GameManager.instance; } }
    public RectTransform[] points;
    public LineRenderer lr;
    public CircleRenderer circle;

    public SkillGUI[] skills;

    [SerializeField]
    ThunderStrike thunderStrike;

    public static SkillManager instance = null;

    private void Awake()
    {
        if (instance != this)
            instance = this;
    }

    //public void Init()
    //{
    //    SetIsDragable(false);
    //    //scroll.Init();
    //}
    ////////////////////////
    //// Rolling Save
    ////////////////////////
    //private void SaveShowCase()
    //{
    //    string showCaseStr = JsonHelper.ToJson<int>(showCaseId);
    //    PlayerPrefs.SetString("showCase", showCaseStr);
    //}

    //private void SaveSelected()
    //{
    //    PlayerPrefs.SetInt("selected", selected); 
    //}

    //private void SaveRollCount()
    //{
    //    PlayerPrefs.SetInt("rollCount", rollCount);
    //}

    //private void LoadRollCount()
    //{
    //    rollCount = PlayerPrefs.GetInt("rollCount");
    //}

    //private bool LoadShowCase()
    //{
    //    string showCaseStr = PlayerPrefs.GetString("showCase");
    //    if (showCaseStr != string.Empty)
    //    {
    //        showCaseId = JsonHelper.FromJson<int>(showCaseStr);
    //        Debug.Log("showCase Loaded:" + showCaseStr);
    //        return true;
    //    }
    //    return false;
    //}

    //private bool LoadSelected()
    //{
    //    if (PlayerPrefs.GetInt("selected") != -1)
    //    {
    //        selected = PlayerPrefs.GetInt("selected");
    //        Debug.Log("selected Loaded:" + selected);
    //        return true;
    //    }
    //    return false;
    //}
    
    //public void ResetShowCase()
    //{
    //    PlayerPrefs.SetString("showCase", string.Empty);
    //    PlayerPrefs.SetInt("rollCount", 0);
    //    ResetSelected();
    //}

    //public void ResetSelected()
    //{
    //    PlayerPrefs.SetInt("selected", -1);
    //}
    ////////////////////////
    //public SkillData GetData(int id)
    //{
    //    return GameDatabase.instance.skills[id];
    //}

    //public Sprite GetSprite(int id)
    //{
    //    if (id >= GameDatabase.instance.skills.Length)
    //    {
    //        Debug.Log(string.Format("SaveData Not Matching With DataBase:Skill {0}",id));
    //        return null;
    //    }
    //    return GameDatabase.instance.skills[id].spr;
    //}

    //public int GetRandom()
    //{
    //    return Random.Range(0, GameDatabase.instance.skills.Length);
    //}

    //public void SetSprite(int position, Sprite spr)
    //{
    //    showCase[position].sprite = spr;
    //}

    //public void SyncShowCase(int position)
    //{
    //    showCase[position].sprite = GetSprite(showCaseId[position]);
    //    //SetShowCaseEnable(position, true);
    //}

    //public void SetShowCaseEnable(int position, bool value)
    //{
    //    showCase[position].enabled = value;
    //}

    ///// <summary>
    ///// RandomSelect Skills
    ///// </summary>
    //public void RollSkill()
    //{
    //    SetIsDragable(false);
    //    LoadRollCount();
    //    if (!LoadSelected())//로드에 실패하면
    //    {
    //        selected = Random.Range(0, 10);//새로운 selected
    //        rollCount++;
    //        SaveRollCount();
    //        SaveSelected();//저장
    //    }
    //    //Save Here
    //    int spin = 1;
    //    if(_selected != -1)
    //        scroll.Spin(spin * 10 + selected - _selected);
    //    else
    //        scroll.Spin(spin * 10 + selected);
    //    StartCoroutine(CheckRollEnd());
    //}


    ///// <summary>
    ///// 쇼케이스 설정
    ///// </summary>
    //public void SetShowCase()
    //{
    //    if (!LoadShowCase())
    //    {
    //        for (int i = 0; i < showCase.Length; i++)
    //        {
    //            int rnd;
    //            for (int j = 0; j < 50; j++)
    //            {
    //                rnd = GetRandom();
    //                if (IsSetable(i, rnd))
    //                {
    //                    showCase[i].enabled = true;
    //                    showCaseId[i] = rnd;
    //                }
    //            }
    //            SetSprite(i, GetSprite(showCaseId[i]));
    //        }
    //        SaveShowCase();
    //    }
    //    else
    //    {
    //        for (int i = 0; i < showCase.Length; i++)
    //        {
    //            showCase[i].enabled = true;
    //            SetSprite(i, GetSprite(showCaseId[i]));
    //        }
    //    }
    //    //RollSkill();
    //}

    ///// <summary>
    ///// 회전 끝났는지 체크
    ///// </summary>
    ///// <returns></returns>
    //public IEnumerator CheckRollEnd()
    //{
    //    float t = 0;
    //    while (t < 0.5f)
    //    {
    //        t += Time.unscaledDeltaTime;
    //        yield return null;
    //    }
    //    while (true)
    //    {
    //        if (scroll.rolling <= 0)//회전하는 코루틴 개수
    //            break;
    //        t = 0;
    //        while (t < 0.1f)
    //        {
    //            t += Time.unscaledDeltaTime;
    //            yield return null;
    //        }
    //    }
    //    OnRollEnd();
    //}
    ///// <summary>
    ///// 스킬들의 중복검사
    ///// </summary>
    ///// <param name="position">쇼케이스의 위치</param>
    ///// <param name="id">스킬의 ID</param>
    ///// <returns></returns>
    //public bool IsSetable(int position, int id)
    //{
    //    if (position == 0)
    //        return true;
    //    else if (position == 1)
    //        return showCaseId[position - 1] != id;
    //    else if (position == showCaseId.Length - 2)
    //        return showCaseId[0] != id && showCaseId[position - 2] != id && showCaseId[position - 1] != id;
    //    else if (position == showCaseId.Length - 1)
    //        return showCaseId[0] != id && showCaseId[1] != id && showCaseId[position - 2] != id && showCaseId[position - 1] != id;
    //    else
    //        return showCaseId[position - 2] != id && showCaseId[position - 1] != id;
    //}

    //public void SetIsDragable(bool value)
    //{
    //    isDragable = value;
    //}

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

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="sc">showCaseId</param>
    ///// <param name="position"></param>
    //public void EquipSkill(int sc, int position)
    //{
    //    SetSkill(sc, position);
    //    SetSkillPnl(false);
    //    ResetShowCase();
    //}

    //public void SetSkillPnl(bool value)
    //{
    //    if (value)
    //    {
    //        //skillPnl.SetActive(value);
    //        Init();
    //        SetShowCase();
    //    }
    //    else
    //    {
    //        //LevelUpManager.instance.SetSelectPnl(false);
    //        //skillPnl.SetActive(value);
    //        StartCoroutine(LevelUpManager.instance.EndLevelUp());
    //    }
    //}

    //public void DrawLine(int position, bool isSkill = false)
    //{
    //    if (!isSkill)
    //    {
    //        points[0].position = new Vector3(showCase[position].transform.position.x, showCase[position].transform.position.y, 0);
    //    }
    //    else
    //    {
    //        points[0].position = new Vector3(skills[position].transform.position.x, skills[position].transform.position.y, 0);
    //    }
    //    Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    points[2].position = new Vector3(mp.x, mp.y, 0);
    //    float mid = (BoardManager.instance.boardRange[0].x + BoardManager.instance.boardRange[1].x) / 2;
    //    points[1].position = new Vector3((mid + mp.x) / 2, (points[0].position.y + points[2].position.y) / 2, 0);
    //}

    public void SetLine(bool value)
    {
        lr.enabled = value;
    }

    //public bool HasSkill(int id)
    //{
    //    return skills[0].skill.data.id == id || skills[1].skill.data.id == id || skills[2].skill.data.id == id;
    //}

    //public bool EqualSkill(int position)
    //{
    //    return skills[position].skill.data.id == GetId(position);
    //}

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

//    public void SetSkillSelectPnl(bool value)
//    {
//        skillSelectPnl.SetActive(value);
//    }

//    private bool isRoll;//참이면 ReRoll을 하는 중, 거짓이면 Reject를 하는 중
//    public void ReRoll()
//    {
//        if (scroll.rolling <= 0)
//        {
//            skillSelectTxt.text = "스킬을 다시 선택하는데 " + (rollCount * 10) + " Soul이 소모됩니다.";
//            isRoll = true;
//            SetSkillSelectPnl(true);
//        }
//    }

//    public void Reject()
//    {
//        skillSelectTxt.text = "스킬을 선택하지 않고 다음 스테이지로 넘어가시겠습니까?";
//        isRoll = false;
//        SetSkillSelectPnl(true);
//    }

//    public void Ok()
//    {
//        if(isRoll)
//        {
//            Debug.Log("Ok, Roll");
//                if (MoneyManager.instance.UseSoul(rollCount * 10))
//                {
//                    _selected = selected;//이전의 selected를 저장
//                    ResetSelected();//기존의 saved selected를 초기화하고 진행
//                    RollSkill();
//                    SetSkillSelectPnl(false);
//                }
//                else
//                    StartCoroutine(CanNotRoll());
//        }
//        else
//        {
//            SetSkillPnl(false);
//            SetSkillSelectPnl(false);
//        }
//    }

//    //Soul이 부족해서 ReRoll 실패
//    private IEnumerator CanNotRoll()
//    {
//        skillSelectTxt.text = "Soul이 부족합니다.";
//        okBtn.interactable = false;
//        cancelBtn.interactable = false;
//#if DELAY
//        yield return GameManager.instance.delayOneReal;
//#else
//        float t = 0;
//        while (t < 1)
//        {
//            t += Time.unscaledDeltaTime;
//            yield return null;
//        }
//#endif
//        okBtn.interactable = true;
//        cancelBtn.interactable = true;
//        SetSkillSelectPnl(false);
//    }

//    public void Cancel()
//    {
//        SetSkillSelectPnl(false);
//    }

    public CircleRenderer GetCircle()
    {
        return circle;
    }

    //public void Change(int id, int position)//스킬 교환, 확인과 효과
    //{
    //    SkillData origin = skills[position].data;

    //}
}
