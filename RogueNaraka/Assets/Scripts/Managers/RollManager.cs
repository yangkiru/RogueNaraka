using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollManager : MonoBehaviour {
    public InfiniteScroll scroll;
    public Button rejectBtn;
    public Button reRollBtn;
    public Image[] showCases;

    private RollData[] datas;
    private int selected;
    private int rollCount;
    private bool isClickable;

    private void LoadRollCount()
    {
        rollCount = PlayerPrefs.GetInt("rollCount");
    }

    private bool LoadSelected()
    {
        if (PlayerPrefs.GetInt("selected") != -1)
        {
            selected = PlayerPrefs.GetInt("selected");
            Debug.Log("selected Loaded:" + selected);
            return true;
        }
        return false;
    }

    public void ResetShowCase()
    {
        PlayerPrefs.SetString("showCase", string.Empty);
        PlayerPrefs.SetInt("rollCount", 0);
        ResetSelected();
    }

    public void ResetSelected()
    {
        PlayerPrefs.SetInt("selected", -1);
    }

    public SkillData GetSkillData(RollData data)
    {
        return GameDatabase.instance.skills[data.id];
    }

    public Sprite GetSprite(RollData data)
    {
        {
            Sprite result = null;
            switch (data.type)
            {
                case ROLL_TYPE.SKILL:
                    result = GameDatabase.instance.skills[data.id].spr;
                    break;
                case ROLL_TYPE.STAT:
                    result = null;//수정 필요
                    break;
                case ROLL_TYPE.ITEM:
                    result = GameDatabase.instance.itemSprites[Item.instance.sprIds[data.id]].spr;
                    break;
                case ROLL_TYPE.PASSIVE:
                    result = null;//수정 필요
                    break;
            }
            return result;
        }
    }
    public RollData GetRandom()
    {
        RollData result = new RollData();
        result.type = (ROLL_TYPE)Random.Range(0, 4);
        switch (result.type)
        {
            case ROLL_TYPE.SKILL:
                result.id = Random.Range(0, GameDatabase.instance.skills.Length);
                break;
            case ROLL_TYPE.STAT:
                result.id = Random.Range(0, 2);//스탯 이미지의 길이로 수정
                break;
            case ROLL_TYPE.ITEM:
                result.id = Random.Range(0, GameDatabase.instance.items.Length);
                break;
            case ROLL_TYPE.PASSIVE:
                result.id = Random.Range(0, GameDatabase.instance.skills.Length);//패시브의 길이로 수정
                break;
        }
        return result;
    }

    public void SetSprite(int position, Sprite spr)
    {
        showCases[position].sprite = spr;
    }

    public void SyncShowCase(int position)
    {
        showCases[position].sprite = GetSprite(datas[position]);
        //SetShowCaseEnable(position, true);
    }

    public void SetShowCaseEnable(int position, bool value)
    {
        showCases[position].enabled = value;
    }

    /// <summary>
    /// RandomSelect Skills
    /// </summary>
    public void RollSkill()
    {
        isClickable = false;
        LoadRollCount();
        if (!LoadSelected())//로드에 실패하면
        {
            selected = Random.Range(0, 10);//새로운 selected
            rollCount++;
            PlayerPrefs.SetInt("rollCount", rollCount);//Roll Count 저장
            PlayerPrefs.SetInt("selected", selected);//저장
            if (Random.Range(0, 100) < 1)//1%
                SetAddOne(true);
            else
                SetAddOne(false);
        }
        //Save Here
        int spin = Random.Range(2, 4);//2~3바퀴
        if (selected != -1)
            scroll.Spin(spin * 10 + selected - selected);
        else
            scroll.Spin(spin * 10 + selected);
        StartCoroutine(CheckRollEnd());
    }

    public void SetAddOne(bool value)
    {
        if (value)
            PlayerPrefs.SetInt("isAddOne", 1);
        else
            PlayerPrefs.SetInt("isAddOne", 0);
    }

    /// <summary>
    /// 쇼케이스 설정
    /// </summary>
    public void SetShowCase()
    {
        if (!LoadDatas())
        {
            for (int i = 0; i < showCases.Length; i++)
            {
                RollData rnd;
                for (int j = 0; j < 50; j++)
                {
                    rnd = GetRandom();
                    if (IsSetable(i, rnd))
                    {
                        showCases[i].enabled = true;
                        datas[i] = rnd;
                    }
                }
                SetSprite(i, GetSprite(datas[i]));
            }
            string datasJson = JsonHelper.ToJson<RollData>(datas);
            PlayerPrefs.SetString("rollDatas", datasJson);
        }
        else
        {
            for (int i = 0; i < showCases.Length; i++)
            {
                showCases[i].enabled = true;
                SetSprite(i, GetSprite(datas[i]));
            }
        }
        RollSkill();
    }

    /// <summary>
    /// 데이터 로드
    /// </summary>
    /// <returns></returns>
    private bool LoadDatas()
    {
        string datasJson = PlayerPrefs.GetString("rollDatas");
        if (datasJson != string.Empty)
        {
            datas = JsonHelper.FromJson<RollData>(datasJson);
            Debug.Log("RollDatas Loaded:" + datasJson);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 회전 끝났는지 체크
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckRollEnd()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        while (true)
        {
            if (scroll.rolling <= 0)//회전하는 코루틴 개수
                break;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        OnRollEnd();
    }

    /// <summary>
    /// 회전 끝나면 실행
    /// </summary>
    public void OnRollEnd()
    {
        isClickable = true;
        if (PlayerPrefs.GetInt("isAddOne") == 1)
            AddOne();
    }

    /// <summary>
    /// 랜덤 확률로 +1
    /// </summary>
    public void AddOne()
    {
        selected++;
        StartCoroutine(SpeedUp());
        scroll.Spin(1);
        Debug.Log("addone");
    }

    /// <summary>
    /// 가속
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpeedUp()
    {
        Debug.Log("SpeedUp");
        yield return new WaitForSecondsRealtime(0.1f);
        while (scroll.rolling > 0)
        {
            scroll.SpeedUp(1.02f);
            yield return null;
        }
        scroll.SpeedReset();
    }

    /// <summary>
    /// 스킬들의 중복검사
    /// </summary>
    /// <param name="position">쇼케이스의 위치</param>
    /// <param name="id">스킬의 ID</param>
    /// <returns></returns>
    public bool IsSetable(int position, RollData data)
    {
        if (position == 0)
            return true;
        else if (position == 1)
            return datas[position - 1] != data;
        else if (position == datas.Length - 2)
            return datas[0] != data && datas[position - 2] != data && datas[position - 1] != data;
        else if (position == datas.Length - 1)
            return datas[0] != data && datas[1] != data && datas[position - 2] != data && datas[position - 1] != data;
        else
            return datas[position - 2] != data && datas[position - 1] != data;
    }

    public SkillData GetSkillData(int id)
    {
        return GameDatabase.instance.skills[id];
    }

    public enum ROLL_TYPE
    { SKILL, STAT, ITEM, PASSIVE}

    [System.Serializable]
    public struct RollData
    {
        public ROLL_TYPE type;
        public int id;

        public static bool operator ==(RollData d1, RollData d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(RollData d1, RollData d2)
        {
            return !d1.Equals(d2);
        }
    }
}
