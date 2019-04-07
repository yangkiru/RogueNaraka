using UnityEngine;
using System.Collections;
using TMPro;

public class SkillChangeManager : MonoBehaviour
{
    static public SkillChangeManager instance;

    public GameObject changePnl;
    public TextMeshProUGUI costTxt;
    public TextMeshProUGUI levelTxt;

    public int Levels
    {
        set { Level_0 = value; Level_1 = value; Level_2 = value; }
    }

    public int Level
    {
        get {
            switch (this.position)
            {
                case 0: return Level_0;
                case 1: return Level_1;
                case 2: return Level_2;
                default: Debug.LogError("SkillChangeManager:Position Error in Set Level"); return Level_0;
            }
        }
        set
        {
            switch(this.position)
            {
                case 0: Level_0 = value; break;
                case 1: Level_1 = value; break;
                case 2: Level_2 = value; break;
                default: Debug.LogError("SkillChangeManager:Position Error in Set Level"); break;
            }
        }
    }

    public int Level_0
    {
        get { return PlayerPrefs.GetInt("skillChangeLevel_0"); }
        set { PlayerPrefs.SetInt("skillChangeLevel_0", value); }
    }

    public int Level_1
    {
        get { return PlayerPrefs.GetInt("skillChangeLevel_1"); }
        set { PlayerPrefs.SetInt("skillChangeLevel_1", value); }
    }

    public int Level_2
    {
        get { return PlayerPrefs.GetInt("skillChangeLevel_2"); }
        set { PlayerPrefs.SetInt("skillChangeLevel_2", value); }
    }

    SkillData data;

    int position;//스킬 슬롯 위치
    int level;//스킬 레벨
    int cost;//소울 비용

    float originSize = 0;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 스킬을 교체하는 패널을 여는 함수
    /// </summary>
    /// <param name="data">교체하는 스킬</param>
    /// <param name="position">교체될 슬롯</param>
    public void OpenChangePnl(SkillData data, int position)
    {
        Debug.Log("OpenChangePnl");
        changePnl.SetActive(true);
        this.position = position;
        this.data = data;
        Debug.Log(Level);
        if (Level == 0)
        {
            level = Random.Range(1, SkillManager.instance.skills[position].skill.data.level + 1);
            Level = level;
            Debug.Log("Random:"+level);
            //StartCoroutine(LevelTxtCorou(level));
        }
        else
        {
            level = Level;
            //levelTxt.text = string.Format("{0} Level", level);
        }
        
        if(originSize != 0)
            levelTxt.fontSize = originSize;
        StartCoroutine("LevelTxtCorou", level);

        //cost = GetChangeCost(level);
        ////구매할 수 있는 최댓값의 레벨 탐색
        //while (!MoneyManager.instance.IsUseable(cost) && level >= 1)
        //{
        //    cost = GetChangeCost(--level);
        //}


        //costTxt.text = string.Format("{0} Soul", cost);
    }

    IEnumerator LevelTxtCorou(int level)
    {
        originSize = levelTxt.fontSize;
        float maxSize = originSize * 3;
        for (int i = 1; i <= level; i++)
        {
            levelTxt.text = string.Format("{0} Level", i);

            levelTxt.fontSize = levelTxt.fontSize < maxSize ? levelTxt.fontSize * 1.2f : maxSize * 1.2f;

            AudioManager.instance.PlaySFX(string.Format("weapon{0}", Random.Range(1, 4)));

            float t = Input.GetMouseButton(0) ? 0.083f : 0.25f;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
                    levelTxt.fontSize *= 0.99f;
                if (levelTxt.fontSize < originSize)
                    levelTxt.fontSize = originSize;
            } while (t > 0);
        }
        do
        {
            yield return null;
            levelTxt.fontSize *= 0.9f;
        } while (levelTxt.fontSize >= originSize);
        levelTxt.fontSize = originSize;
    }

    /// <summary>
    /// Level 증가, 감소
    /// </summary>
    /// <param name="isUp">참이면 증가</param>
    public void SetLevel(bool isUp)
    {
        int _level = level + (isUp ? 1 : -1);

        if (SkillManager.instance.skills[position].skill.data.level < _level || _level < 1)
            return;
        level = _level;

        cost = GetChangeCost(level);
        costTxt.text = string.Format("{0} Soul", cost);
        levelTxt.text = string.Format("{0} Level", level);
    }

    /// <summary>
    /// 교체 결정 함수
    /// </summary>
    public void Change()
    {
        //if(!MoneyManager.instance.UseSoul(cost))
        //{
        //    return;
        //}

        changePnl.SetActive(false);
        StopCoroutine("LevelTxtCorou");
        AudioManager.instance.PlaySFX("skillEquip");

        SkillManager.instance.skills[position].Init(data);
        SkillManager.instance.skills[position].LevelUp(level - 1);
        RollManager.instance.SetRollPnl(false);
    }

    /// <summary>
    /// 취소 함수
    /// </summary>
    public void Cancel()
    {
        changePnl.SetActive(false);
        StopCoroutine("LevelTxtCorou");
    }

    /// <summary>
    /// 피보나치 수열
    /// </summary>
    /// <param name="level">스킬 레벨</param>
    /// <returns></returns>
    public int GetChangeCost(int level)
    {
        switch (level)
        {
            case 0:
            case 1:
                return 0;
            case 2:
                return 10;
            case 3:
                return 20;
            default:
                return GetChangeCost(level - 2) + GetChangeCost(level - 1);
        }
    }
}
