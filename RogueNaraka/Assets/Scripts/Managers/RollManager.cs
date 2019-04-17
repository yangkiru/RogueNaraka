using RogueNaraka.SkillScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollManager : MonoBehaviour {
    public InfiniteScroll scroll;
    public Button rejectBtn;
    public Image[] showCases;
    public GameObject pauseBtn;
    public GameObject rollPnl;
    public GameObject selectPnl;
    public GameObject soulAlertPnl;
    public Image dragImg;
    public TextMeshProUGUI typeTxt;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI descTxt;
    public TextMeshProUGUI reRollTxt;
    public TextMeshProUGUI[] statTxts;

    public Fade fade;
    public ManaUI manaUI;

    public Vector3 restPosition;

    public int selected;
    public int stopped;
    public bool isClickable;
    public bool isPause { get; set; }
    public RollData[] datas;
    /// <summary>
    /// reRoll 횟수
    /// </summary>
    private int rollCount;
    /// <summary>
    /// roll 실행 횟수
    /// </summary>
    public static int LeftRoll
    {
        get { return PlayerPrefs.GetInt("leftRoll"); }
        set { PlayerPrefs.SetInt("leftRoll", value); }
    }

    static public bool IsFirstRoll
    {
        get { return PlayerPrefs.GetInt("isFirstRoll") == 1; }
        set { PlayerPrefs.SetInt("isFirstRoll", value ? 1 : 0); }
    }

    private bool isSkillSelected;

    private int target;//선택된 SkillGUI

    public static RollManager instance;

    public enum ROLL_TYPE { ALL, SKILL, STAT, ITEM }
    ROLL_TYPE[] lastMode;

    public event System.Action onFadeOut;

    void Awake()
    {
        instance = this;
        Init();
    }
    public void Init()
    {
        if(datas.Length < showCases.Length)
            datas = new RollData[showCases.Length];
        for (int i = 0; i < 10; i++)
        {
            showCases[i].enabled = false;
            datas[i].id = -1;
        }
        isClickable = false;
        isPassed = false;
        stopped = -1;
        selected = -1;
        rollCount = 0;
        isReRoll = false;
        scroll.Init();
        SetSelectPnl(false);
    }

    /// <summary>
    /// 쇼케이스 설정
    /// </summary>
    public void SetShowCase(params ROLL_TYPE[] mode)
    {
        Init();
        lastMode = mode;
        if (!LoadDatas())
        {
            SkillChangeManager.instance.Levels = 0;
            for (int i = 0; i < showCases.Length; i++)
            {
                RollData rnd = new RollData();
                for (int j = 0; j < 50; j++)
                {
                    rnd = GetRandom(rnd, mode);
                    if (IsSetable(i, rnd))
                        break;
                }
                showCases[i].enabled = true;
                datas[i] = rnd;
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
    }

    public void SetRollPnl(bool value)
    {
        if (value)
        {
            GameManager.instance.SetPauseBtn(true);
            rollPnl.SetActive(value);
            reRollTxt.text = "ReRoll";
            fade.FadeIn();
            if (Pointer.instance)
                Pointer.instance.SetPointer(false);
        }
        else
        {
            ResetData();

            SkillManager.instance.Save();
            //if (!isStageUp)
            //{
            //    PlayerPrefs.SetInt("isFirstRoll", 0);
            //    SkillManager.instance.Save();
            //    Item.instance.Save();
            //}
            //if (isStageUp)
            //{
            //    BoardManager.instance.StageUp();
            //    //BoardManager.instance.Save();
            //    PlayerPrefs.SetInt("isLevelUp", 0);
            //    //LevelUpManager.instance.StartCoroutine(LevelUpManager.instance.EndLevelUp());
            //    GameManager.instance.Save();
            //}

            //StatManager.instance.SetStatPnl(false);
            Debug.Log(LeftRoll);
            if (--LeftRoll <= 0)
            {
                LeftRoll = 0;
                fade.FadeOut();
            }
            else
            {
                SetShowCase(lastMode);
                Roll();
            }

        }
    }

    public void FirstRoll()
    {
        if (RollManager.LeftRoll == 0)
            RollManager.LeftRoll = 3;
        RollManager.instance.SetShowCase(RollManager.ROLL_TYPE.SKILL);
        RollManager.instance.SetRollPnl(true);
        RollManager.instance.Roll();
        RollManager.instance.SetOnFadeOut(RollManager.instance.GameStart);
    }

    public void GameStart()
    {
        IsFirstRoll = false;
        Stat stat = Stat.DataToStat();
        if (stat != null)
            GameManager.instance.RunGame(stat);
        //else
        //    BoardManager.instance.fade.FadeIn();
    }

    public void StageStart()
    {
        BoardManager.instance.StageUp();
        BoardManager.instance.InitBoard();
        LevelUpManager.IsLevelUp = false;
    }

    public void SetOnFadeOut(System.Action onFadeOut)
    {
        this.onFadeOut = onFadeOut;
    }

    public void OnFadeOut()
    {
        rollPnl.SetActive(false);
        StatManager.instance.statPnl.SetActive(false);
        if (onFadeOut != null)
            onFadeOut.Invoke();
        //if (isStageUp)
        //    BoardManager.instance.InitBoard();
        //else
        //{
        //    Stat stat = Stat.DataToStat();
        //    if (stat != null)
        //        GameManager.instance.RunGame(stat);
        //    //else
        //    //    BoardManager.instance.fade.FadeIn();
        //}
    }

    bool isReRoll;//참이면 ReRoll 재 호출시 ReRoll 진행
    public void ReRoll()
    {
        if (scroll.rolling <= 0)
        {
            if (!isReRoll)
            {
                LoadRollCount();
                int amount = rollCount * 10;
                
                if(MoneyManager.instance.soul >= amount)
                    isReRoll = true;
                if (reRollTxt.text.CompareTo("ReRoll") == 0)
                    reRollTxt.text = string.Format("{0}Soul", amount);
                else
                {
                    StartCoroutine(OnFail());
                }
            }
            else
            {
                reRollTxt.text = "ReRoll";
                isReRoll = false;
                MoneyManager.instance.UseSoul(rollCount * 10);
                isClickable = false;
                LoadRollCount();
                int last = stopped;
                do
                {
                    stopped = Random.Range(0, 10);//새로운 selected
                } while (last == stopped);
                rollCount++;
                PlayerPrefs.SetInt("rollCount", rollCount);//Roll Count 저장
                PlayerPrefs.SetInt("stopped", stopped);//저장
                SkillChangeManager.instance.Levels = 0;
                int spin = 1;
                scroll.Spin(spin * 10 + stopped - last);
                SetSelectPnl(false);
                StartCoroutine(CheckRollEnd());
            }
        }
    }

    IEnumerator OnFail()
    {
        float t = 1f;
        reRollTxt.text = "Fail";
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        soulAlertPnl.SetActive(true);
        reRollTxt.text = "ReRoll";
    }

    bool isPassed;
    public void Pass()
    {
        if (!isPassed)
        {
            isPassed = true;
        }
        else
        {
            isPassed = false;
            SetRollPnl(false);
        }
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
                    case ROLL_TYPE.ALL:
                        datas[i] = GetRandom(datas[i], ROLL_TYPE.ALL);
                        break;
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
                    //case ROLL_TYPE.PASSIVE:
                    //    break;
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

    public void ResetData()
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
        //if (stopped == position || (stopped + 1) % 10 == position || (stopped + 2) % 10 == position)
        if ((stopped + 1) % 10 == position)
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
                    //selectedImg.sprite = GetSprite(data);
                    typeTxt.text = GameManager.language == Language.Korean ? "기술" : "Skill";
                    nameTxt.text = skill.GetName();
                    descTxt.text = skill.GetDescription();
                    manaUI.SetMana(skill);
                    break;
                case ROLL_TYPE.STAT:
                    //selectedImg.sprite = GetSprite(data);
                    manaUI.gameObject.SetActive(false);
                    typeTxt.text = GameManager.language == Language.Korean ? "능력치" : "Stat";
                    string point = "Point";
                    if (data.id + 1 > 1)
                        point += "s";
                    nameTxt.text = (data.id + 1) + point;
                    switch (GameManager.language)
                    {
                        default:
                            descTxt.text = string.Format("You will get {0} Stat {1}.", (data.id + 1), point);
                            break;
                        case Language.Korean:
                            descTxt.text = string.Format("{0}의 스탯 포인트를 획득한다.", (data.id + 1));
                            break;
                    }
                    break;
                case ROLL_TYPE.ITEM:
                    manaUI.gameObject.SetActive(false);
                    //selectedImg.sprite = GetSprite(data);
                    typeTxt.text = GameManager.language == Language.Korean ? "소모품" : "ITEM";
                    ItemData item = GameDatabase.instance.items[data.id];
                    ItemSpriteData itemSpr = GameDatabase.instance.itemSprites[Item.instance.sprIds[item.id]];
                    //if (Item.instance.isKnown[item.id])
                    //{
                    nameTxt.text = item.GetName();
                    descTxt.text = item.GetDescription();
                    //}
                    //else
                    //{
                    //    nameTxt.text = string.Format(format, item.GetName());
                    //    descTxt.text = string.Format(format, item.GetDescription());
                    //}
                    break;
                //case ROLL_TYPE.PASSIVE:
                //    manaUI.gameObject.SetActive(false);
                //    //selectedImg.sprite = GetSprite(data);
                //    typeTxt.text = "Passive";
                //    nameTxt.text = "패시브";
                //    descTxt.text = "패시브";
                //    break;
            }

            SetSelectPnl(true);
        }
    }

    void Update()
    {
        if (isMouseDown)
            OnMouse();
    }

    public void OnMouseClick(int position)
    {
        if (selected == position && datas[selected].type == ROLL_TYPE.STAT)
            Ok();
    }

    bool isMouseDown;
    
    public void OnMouseDown(int position)
    {
        if (selected != position)
            return;
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
        Vector3 pos = GameManager.instance.GetMousePosition();
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
        dragImg.rectTransform.position = restPosition;
        dragImg.sprite = null;
        dragImg.gameObject.SetActive(false);
    }

    public void Ok()
    {
        RollData rollData = datas[selected];
        switch (rollData.type)
        {
            case ROLL_TYPE.SKILL:
                SkillData selectedSkill = GameDatabase.instance.skills[rollData.id];//선택된 스킬 데이터
                Skill slotSkill = SkillManager.instance.skills[target].skill;//해당 슬롯에 장착된 스킬
                if (slotSkill == null || slotSkill.data.id == -1)//슬롯이 비었으면
                {
                    SkillManager.instance.SetSkill(selectedSkill, target);
                    SetRollPnl(false);
                }
                else if (slotSkill.data.id == selectedSkill.id)//같은 스킬이면
                {
                    SkillManager.instance.SetSkill(selectedSkill, target);
                    SetRollPnl(false);
                }
                else//다른 스킬이면
                {
                    if (slotSkill.data.level == 1)
                    {
                        SkillManager.instance.SetSkill(selectedSkill, target);
                        SetRollPnl(false);
                    }
                    else
                        SkillChangeManager.instance.OpenChangePnl(selectedSkill, target);//스킬 교체 패널 오픈
                }
                //SetRollPnl(false, isStageUp);
                break;
            case ROLL_TYPE.STAT:
                StatManager.instance.SetStatPnl(true, rollData.id + 1);
                break;
            case ROLL_TYPE.ITEM:
                Item.instance.EquipItem(rollData.id);
                SetRollPnl(false);
                break;
            //case ROLL_TYPE.PASSIVE:
            //    //selectedImg.sprite = GetSprite(data);
            //    typeTxt.text = "Passive";
            //    nameTxt.text = "패시브";
            //    descTxt.text = "패시브";
            //    break;
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
                    //case ROLL_TYPE.PASSIVE:
                    //    result = null;//수정 필요
                    //    break;
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
    public RollData GetRandom(RollData result, params ROLL_TYPE[] modes)
    {

        int rnd = Random.Range(0, modes.Length);
        result.type = modes[rnd];
        switch (modes[rnd])
        {
            case ROLL_TYPE.ALL:
                int rndMode = Random.Range(1, (int)ROLL_TYPE.ITEM + 1);
                return GetRandom(result, (ROLL_TYPE)rndMode);
            case ROLL_TYPE.SKILL:
                do
                {
                    result.id = Random.Range(0, GameDatabase.instance.skills.Length);
                } while (!SkillData.IsBought(result.id) && !GameDatabase.instance.skills[result.id].isBasic);
                break;
            case ROLL_TYPE.STAT:
                result.id = Random.Range(0, 3);
                break;
            case ROLL_TYPE.ITEM:
                result.id = Random.Range(0, GameDatabase.instance.items.Length);
                break;
            //case ROLL_TYPE.PASSIVE:
            //    result = GetRandom();
            //    //result.id = Random.Range(0, GameDatabase.instance.skills.Length);//패시브의 길이로 수정
            //    break;
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
        SetSelectPnl(false);
        isClickable = false;
        LoadRollCount();
        if (!LoadStopped())//로드에 실패하면
        {
            stopped = Random.Range(0, 10);//새로운 selected
            rollCount++;
            PlayerPrefs.SetInt("rollCount", rollCount);//Roll Count 저장
            PlayerPrefs.SetInt("stopped", stopped);//저장
        }
        scroll.Spin(10 + stopped);
        StartCoroutine(CheckRollEnd());
    }

    /// <summary>
    /// 회전 끝났는지 체크
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckRollEnd()
    {
        yield return null;
        do
        {
            yield return null;
        } while (scroll.rolling > 0);
        OnRollEnd();
    }

    /// <summary>
    /// 회전 끝나면 실행
    /// </summary>
    public void OnRollEnd()
    {
        isClickable = true;
        StartCoroutine(IconEffectCorou());
        Select((stopped + 1) % 10);
        
        AudioManager.instance.PlaySFX("skillStop");
        switch(datas[(stopped + 1) % 10].type)
        {
            case ROLL_TYPE.SKILL:
                TutorialManager.instance.StartTutorial(2);
                break;
            case ROLL_TYPE.ITEM:
                TutorialManager.instance.StartTutorial(5);
                break;
        }
    }

    IEnumerator IconEffectCorou()
    {
        float size = 1.5f;
        float t = 0.5f;
        RectTransform imgRect = showCases[(stopped + 1) % 10].rectTransform;
        imgRect.localScale = new Vector3(size, size, 0);
        while (t > 0)
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
            imgRect.localScale = Vector3.Lerp(imgRect.localScale, Vector3.one, 1 - t * 2);
        }
        imgRect.localScale = Vector3.one;
    }

    void SetStatTxt(bool active, int position = -1)
    {
        if (position == -1)
        {
            for (int i = 0; i < statTxts.Length; i++)
            {
                if (active && datas[i].type == ROLL_TYPE.STAT)
                {
                    statTxts[i].text = string.Format("+{0}", datas[i].id + 1);
                    statTxts[i].gameObject.SetActive(true);
                }
                else
                    statTxts[i].gameObject.SetActive(false);
            }
        }
        else
        {
            if (active && datas[position].type == ROLL_TYPE.STAT)
            {
                statTxts[position].text = string.Format("+{0}", datas[position].id + 1);
                statTxts[position].gameObject.SetActive(true);
            }
            else
                statTxts[position].gameObject.SetActive(false);
        }
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
