using UnityEngine;
using System.Collections;
using TMPro;

public class SkillChangeManager : MonoBehaviour
{
    static public SkillChangeManager instance;

    public GameObject changePnl;
    public TextMeshProUGUI costTxt;
    public TextMeshProUGUI levelTxt;

    SkillData data;

    int position;//스킬 슬롯 위치
    int level;//스킬 레벨
    int cost;//소울 비용

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
        level = SkillManager.instance.skills[position].skill.data.level;
        cost = GetChangeCost(level);
        //구매할 수 있는 최댓값의 레벨 탐색
        while (!MoneyManager.instance.IsUseable(cost) && level >= 1)
        {
            cost = GetChangeCost(--level);
        }

        levelTxt.text = string.Format("{0} Level", level);
        costTxt.text = string.Format("{0} Soul", cost);
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
        if(!MoneyManager.instance.UseSoul(cost))
        {
            return;
        }

        changePnl.SetActive(false);
        
        SkillManager.instance.skills[position].Init(data);
        SkillManager.instance.skills[position].LevelUp(level - 1);
        RollManager.instance.SetRollPnl(false, RollManager.instance.isStageUp);
    }

    /// <summary>
    /// 취소 함수
    /// </summary>
    public void Cancel()
    {
        changePnl.SetActive(false);
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
