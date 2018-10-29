using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollManager : MonoBehaviour {
    public InfiniteScroll scroll;
    public Button rejectBtn;
    public Button reRollBtn;
    public Button okBtn;
    public Image[] showCases;
    public GameObject rollPnl;
    public GameObject selectPnl;
    public Image selectedImg;
    public TextMeshProUGUI typeTxt;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI descTxt;

    public int selected;
    public int stopped;
    public bool isClickable;
    public RollData[] datas;
    private int rollCount;
    private bool isSkillSelected;
    /// <summary> 선택된 SkillUI 위치 및 아이템 획득 여부
    /// </summary>
    private int target;
    

    //RollManager는 시작시 active상태가 false이기 때문에 instance를 사용할 수 없다.

    public void Init()
    {
        datas = new RollData[showCases.Length];
        for (int i = 0; i < 10; i++)
        {
            showCases[i].enabled = false;
            datas[i].id = -1;
        }
        isClickable = false;
        stopped = -1;
        selected = -1;
        rollCount = 0;
        scroll.Init();
        okBtn.interactable = false;
        SetSelectPnl(false);
    }

    public void SetRollPnl(bool value)
    {
        if (value)
        {
            rollPnl.SetActive(value);
            Init();
            SetShowCase();
        }
        else
        {
            Reset();
            LevelUpManager.instance.StartCoroutine(LevelUpManager.instance.EndLevelUp());
            rollPnl.SetActive(value);
        }
    }

    public void SetSelectPnl(bool value)
    {
        selectPnl.SetActive(value);
        target = -1;
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

    private void LoadRollCount()
    {
        rollCount = PlayerPrefs.GetInt("rollCount");
    }

    private bool LoadStopped()
    {
        if (PlayerPrefs.GetInt("stopped") != -1)
        {
            stopped = PlayerPrefs.GetInt("stopped");
            Debug.Log("stopped Loaded:" + stopped);
            return true;
        }
        return false;
    }

    public void Reset()
    {
        PlayerPrefs.SetString("rollDatas", string.Empty);
        PlayerPrefs.SetInt("rollCount", 0);
        PlayerPrefs.SetInt("stopped", -1);
    }

    public SkillData GetSkillData(RollData data)
    {
        return GameDatabase.instance.skills[data.id];
    }

    public void Select(int position)
    {
        if (isClickable)
        {
            RollData data = datas[position];
            selected = position;
            switch (data.type)
            {
                case ROLL_TYPE.SKILL:
                    SkillData skill = GameDatabase.instance.skills[data.id];
                    selectedImg.sprite = GetSprite(data);
                    typeTxt.text = "Skill";
                    nameTxt.text = skill.name;
                    descTxt.text = skill.description;
                    break;
                case ROLL_TYPE.STAT:
                    selectedImg.sprite = GetSprite(data);
                    typeTxt.text = "Stat";
                    string point = "Point";
                    if (data.id + 1 > 1)
                        point += "s";
                    nameTxt.text = (data.id + 1).ToString() + point;
                    descTxt.text = (data.id + 1).ToString() + point + "의 스탯 포인트를 획득한다.";
                    break;
                case ROLL_TYPE.ITEM:
                    selectedImg.sprite = GetSprite(data);
                    typeTxt.text = "ITEM";
                    ItemData item = GameDatabase.instance.items[data.id];
                    ItemSpriteData itemSpr = GameDatabase.instance.itemSprites[Item.instance.sprIds[item.id]];
                    if (Item.instance.isKnown[item.id])
                    {
                        nameTxt.text = item.name;
                        descTxt.text = item.GetDescription();
                    }
                    else
                    {
                        nameTxt.text = itemSpr.name;
                        descTxt.text = itemSpr.description;
                    }
                    break;
                case ROLL_TYPE.PASSIVE:
                    selectedImg.sprite = GetSprite(data);
                    typeTxt.text = "Passive";
                    nameTxt.text = "패시브";
                    descTxt.text = "패시브";
                    break;
            }
            SetSelectPnl(true);
        }
    }

    /// <summary>
    /// SkillUI를 클릭하면 호출
    /// </summary>
    /// <param name="position"></param>
    public void SelectSkill(int position)
    {
        if (selected != -1 && datas[selected].type == ROLL_TYPE.SKILL)
        {
            Debug.Log("Skill Added,position:" + position + " id:" + datas[selected].id);
            target = position;
            okBtn.interactable = true;
        }
    }

    /// <summary>
    /// ItemUI를 클릭하면 호출
    /// </summary>
    public void SelectItem()
    {
        if (selected != -1 && datas[selected].type == ROLL_TYPE.ITEM)
        {
            if(target == -1)//선택 전
            {
                Debug.Log("Item Added");
                target = 0;
                okBtn.interactable = true;
            }
            else
            {
                Debug.Log("Item Canceled");
                target = -1;
                okBtn.interactable = false;
            }
        }
    }

    public void Ok()
    {
        RollData data = datas[selected];
        switch (data.type)
        {
            case ROLL_TYPE.SKILL:
                SkillData skill = GameDatabase.instance.skills[data.id];
                SkillManager.instance.SetSkill(datas[selected].id, target);
                break;
            case ROLL_TYPE.STAT:
                selectedImg.sprite = GetSprite(data);
                typeTxt.text = "Stat";
                string point = "Point";
                if (data.id + 1 > 1)
                    point += "s";
                nameTxt.text = (data.id + 1).ToString() + point;
                descTxt.text = "의 스탯 포인트를 획득한다.";
                break;
            case ROLL_TYPE.ITEM:
                Item.instance.SyncData(data.id);
                break;
            case ROLL_TYPE.PASSIVE:
                selectedImg.sprite = GetSprite(data);
                typeTxt.text = "Passive";
                nameTxt.text = "패시브";
                descTxt.text = "패시브";
                break;
        }
        SetRollPnl(false);
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
                result.id = Random.Range(0, 2);
                break;
            case ROLL_TYPE.ITEM:
                result.id = Random.Range(0, GameDatabase.instance.items.Length);
                break;
            case ROLL_TYPE.PASSIVE:
                result = GetRandom();
                //result.id = Random.Range(0, GameDatabase.instance.skills.Length);//패시브의 길이로 수정
                break;
        }
        return result;
    }

    public void SetSprite(int position, Sprite spr)
    {
        showCases[position].sprite = spr;
    }

    /// <summary>
    /// RandomSelect Skills
    /// </summary>
    public void Roll()
    {
        isClickable = false;
        LoadRollCount();
        if (!LoadStopped())//로드에 실패하면
        {
            stopped = Random.Range(0, 10);//새로운 selected
            rollCount++;
            PlayerPrefs.SetInt("rollCount", rollCount);//Roll Count 저장
            PlayerPrefs.SetInt("stopped", stopped);//저장
        }
        //Save Here
        int spin = Random.Range(2, 4);//2~3바퀴
        scroll.Spin(spin * 10 + stopped);
        StartCoroutine(CheckRollEnd());
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
        Roll();
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

        public override bool Equals(object obj)
        {
            if (!(obj is RollData))
            {
                return false;
            }

            var data = (RollData)obj;
            return type == data.type &&
                   id == data.id;
        }

        public override int GetHashCode()
        {
            var hashCode = 961388853;
            hashCode = hashCode * -1521134295 + type.GetHashCode();
            hashCode = hashCode * -1521134295 + id.GetHashCode();
            return hashCode;
        }

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
