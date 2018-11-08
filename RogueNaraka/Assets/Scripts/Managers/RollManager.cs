using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollManager : MonoBehaviour {
    public InfiniteScroll scroll;
    public Button rejectBtn;
    public Image[] showCases;
    public GameObject rollPnl;
    public GameObject selectPnl;
    public Image selectedImg;
    public Image dragImg;
    public TextMeshProUGUI typeTxt;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI descTxt;
    public TextMeshProUGUI[] statTxts;

    public int selected;
    public int stopped;
    public bool isClickable;
    public RollData[] datas;
    private int rollCount;
    private bool isSkillSelected;

    public bool isStatTxt;
    //선택된 SkillUI 위치 및 아이템 획득 여부
    private int target;

    public static RollManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
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
        SetSelectPnl(false);
    }

    void SetStatTxt(bool active, int position = -1)
    {
        if (!isStatTxt)
            return;
        if (position == -1)
        {
            for (int i = 0; i < statTxts.Length; i++)
            {
                if (active && datas[i].type == ROLL_TYPE.STAT)
                {
                    statTxts[i].text = string.Format("+{0}", datas[i].id + 1);
                    statTxts[i].alpha = 1;
                }
                else
                    statTxts[i].alpha = 0;
            }
        }
        else
        {
            if (active && datas[position].type == ROLL_TYPE.STAT)
            {
                statTxts[position].text = string.Format("+{0}", datas[position].id + 1);
                statTxts[position].alpha = 1;
            }
            else
                statTxts[position].alpha = 0;
        }
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

    bool isReRoll;//참이면 ReRoll 재 호출시 ReRoll 진행
    public void ReRoll(TextMeshProUGUI reRollTxt)
    {
        if (scroll.rolling <= 0)
        {
            if (!isReRoll)
            {
                LoadRollCount();
                reRollTxt.text = string.Format("{0}Soul", rollCount * 10);
                isReRoll = true;
            }
            else
            {
                reRollTxt.text = "ReRoll";
                isReRoll = false;
                MoneyManager.instance.UseSoul(rollCount * 10);
                isClickable = false;
                LoadRollCount();
                int last = stopped;
                stopped = Random.Range(0, 10);//새로운 selected
                rollCount++;
                PlayerPrefs.SetInt("rollCount", rollCount);//Roll Count 저장
                PlayerPrefs.SetInt("stopped", stopped);//저장

                int spin = Random.Range(2, 4);//2~3바퀴
                scroll.Spin(spin * 10 + stopped - last);
                StartCoroutine(CheckRollEnd());
            }
        }
    }

    bool isPassed;
    public void Pass()
    {
        if (!isPassed)
        {
            isPassed = true;
        }
        else
            SetRollPnl(false);
    }

    public void SetSelectPnl(bool value)
    {
        selectPnl.SetActive(value);
        if (!value)
        {
            target = -1;
            selected = -1;
        }
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
            for(int i = 0; i < datas.Length; i++)
            {
                switch(datas[i].type)
                {
                    case ROLL_TYPE.ITEM:
                        if (datas[i].id >= GameDatabase.instance.items.Length)
                            return false;
                        break;
                    case ROLL_TYPE.SKILL:
                        if (datas[i].id >= GameDatabase.instance.skills.Length)
                            return false;
                        break;
                    case ROLL_TYPE.STAT:
                        break;
                    case ROLL_TYPE.PASSIVE:
                        break;
                }
            }
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

    private bool IsSelectable(int position)
    {
        if (stopped == position || (stopped + 1) % 10 == position || (stopped + 2) % 10 == position)
            return true;
        else
            return false;
    }

    public void Select(int position)
    {
        if (isClickable && IsSelectable(position))
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
                    descTxt.text = skill.desc;
                    break;
                case ROLL_TYPE.STAT:
                    selectedImg.sprite = GetSprite(data);
                    typeTxt.text = "Stat";
                    string point = "Point";
                    if (data.id + 1 > 1)
                        point += "s";
                    nameTxt.text = (data.id + 1) + point;
                    descTxt.text = "You will get " + (data.id + 1) + "Stat " + point.ToLower() + ".";
                    //descTxt.text = (data.id + 1) + "의 스탯 포인트를 획득한다.";
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

    void Update()
    {
        if (isMouseDown)
            OnMouse();
    }

    public void OnMouseClick()
    {
        if (datas[selected].type == ROLL_TYPE.STAT)
            Ok();
    }

    bool isMouseDown;
    public void OnMouseDown()
    {
        if (datas[selected].type != ROLL_TYPE.STAT)
        {
            isMouseDown = true;
            dragImg.sprite = GetSprite(datas[selected]);
            dragImg.gameObject.SetActive(true);
        }
    }

    public void OnMouse()
    {
        dragImg.sprite = GetSprite(datas[selected]);
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragImg.rectTransform.position = pos;
        Vector3 local = dragImg.rectTransform.localPosition;
        local.z = 2;
        dragImg.rectTransform.localPosition = local;
    }

    public void OnMouseUp()
    {
        if(target != -1)
            Ok();
        isMouseDown = false;
        dragImg.sprite = null;
        dragImg.gameObject.SetActive(false);
    }

    public void Ok()
    {
        RollData data = datas[selected];
        switch (data.type)
        {
            case ROLL_TYPE.SKILL:
                SkillData skill = GameDatabase.instance.skills[data.id];
                SkillManager.instance.SetSkill(datas[selected].id, target);
                SetRollPnl(false);
                break;
            case ROLL_TYPE.STAT:
                LevelUpManager.instance.SetStatPnl(true, data.id + 1);
                break;
            case ROLL_TYPE.ITEM:
                Item.instance.SyncData(data.id);
                SetRollPnl(false);
                break;
            case ROLL_TYPE.PASSIVE:
                selectedImg.sprite = GetSprite(data);
                typeTxt.text = "Passive";
                nameTxt.text = "패시브";
                descTxt.text = "패시브";
                break;
        }
    }

    /// <summary>
    /// SkillUI를 클릭하면 호출
    /// </summary>
    /// <param name="position"></param>
    public void SelectSkill(int position)
    {
        if (isClickable && selected != -1 && datas[selected].type == ROLL_TYPE.SKILL)
        {
            Debug.Log("Skill Added,position:" + position + " id:" + datas[selected].id);
            target = position;
        }
    }

    /// <summary>
    /// ItemUI를 클릭하면 호출
    /// </summary>
    public void SelectItem(bool value)
    {
        if (selected != -1 && datas[selected].type == ROLL_TYPE.ITEM)
        {
            if (value)//선택 전
            {
                Debug.Log("Item Added");
                target = 0;
            }
            else
            {
                Debug.Log("Item Canceled");
                target = -1;
            }
        }
    }

    public Sprite GetSprite(RollData data)
    {
        {
            Sprite result = null;
            try
            {
                switch (data.type)
                {
                    case ROLL_TYPE.SKILL:
                        result = GameDatabase.instance.skills[data.id].spr;
                        break;
                    case ROLL_TYPE.STAT:
                        result = GameDatabase.instance.statSprite;
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
            catch
            {
                return null;
            }
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
                result.id = Random.Range(0, 3);
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

    public bool SetSprite(int position, Sprite spr)
    {
        if (spr == null)
            return false;
        showCases[position].sprite = spr;
        return true;
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
        SetStatTxt(true);
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
