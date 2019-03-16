using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;
using RogueNaraka.SkillScripts;

public class SkillGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Pointer pointer;
    public Image img;
    public Image coolImg;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI coolTimeTxt;
    public int position;
    public bool isCool;

    bool isMouseDown;

    private SkillManager skillManager
    { get { return SkillManager.instance; } }
    private Unit player
    { get { return BoardManager.instance.player; } }

    public Skill skill { get { return _skill; } }
    Skill _skill;

    void Update()
    {
        if (isMouseDown)
            OnMouse();
    }

    public void OnMouse()
    {
        if (_skill.data.id != -1 && (!player.deathable.isDeath || _skill.data.isDeath) && !GameManager.instance.isPause)
        {
            //SkillManager.instance.DrawLine(position, true);
            if (_skill.data.isCircleToPlayer)
            {
                skillManager.GetCircle().SetParent(player.transform);
                skillManager.GetCircle().Move(Vector2.zero);
            }
            else
            {
                skillManager.GetCircle().Move(GameManager.GetMousePosition() + new Vector2(0, pointer.offset));
            }
            pointer.PositionToMouse();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(RollManager.instance.selectPnl.activeSelf && _skill)
        {
            RollManager.RollData data = RollManager.instance.datas[RollManager.instance.selected];
            if (data.type == RollManager.ROLL_TYPE.SKILL)
            {
                if (data.id == _skill.data.id)
                {
                    SkillData skill = GameDatabase.instance.skills[data.id];
                    RollManager.instance.manaUI.SetMana(skill, _skill.data.level + 1);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (RollManager.instance.selectPnl.activeSelf && _skill)
        {
            RollManager.RollData data = RollManager.instance.datas[RollManager.instance.selected];
            if (data.type == RollManager.ROLL_TYPE.SKILL)
            {
                SkillData skill = GameDatabase.instance.skills[data.id];
                RollManager.instance.manaUI.SetMana(skill);
            }
        }
    }

    public void OnMouseDown()
    {
        if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause)
        {
            isMouseDown = true;
            //skillManager.DrawLine(position, true);
            //skillManager.SetLine(true);
            if (_skill.data.size > 0)
            {
                skillManager.GetCircle().SetCircle(_skill.data.size);
                skillManager.GetCircle().SetEnable(true);
            }
            Pointer.instance.SetPointer(true);
        }
    }

    public void OnMouseUp()
    {
        isMouseDown = false;
        skillManager.SetLine(false);
        skillManager.GetCircle().SetEnable(false);
        skillManager.GetCircle().transform.SetParent(null);
        if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause && BoardManager.IsMouseInBoard() && (!player.deathable.isDeath || _skill.data.isDeath) && IsMana())
        {
            UseSkill();
        }
        pointer.SetPointer(false);
    }

    public void UseMana()
    {
        player.mpable.AddMp(-_skill.data.manaCost);
    }

    public bool IsMana()
    {
        bool result = player.mpable.currentMp >= _skill.data.manaCost;
        if (!result)
        {
            ManaScript.instance.StartCoroutine(ManaScript.instance.NeedMana(_skill.data.manaCost));
            ManaScript.instance.StartCoroutine(ManaScript.instance.NoMana());
        }
        return result;
    }

    public void ResetSkill()
    {
        isCool = true;
        img.sprite = null;
        img.color = Color.clear;
        coolImg.sprite = null;
        Color c = coolImg.color;
        c.a = 0;
        coolImg.color = c;
        coolImg.enabled = false;
        coolTimeTxt.text = string.Empty;
        levelTxt.enabled = false;
        if (_skill)
        {
            _skill.data.id = -1;
            Destroy(_skill);
        }
    }

    public void Init(SkillData dt)
    {
        if(_skill)
        {
            Destroy(_skill);
        }

        string str = string.Format("RogueNaraka.SkillScripts.{0}", dt.name);
        System.Type type = System.Type.GetType(str);

        _skill = gameObject.AddComponent(type) as Skill;
        _skill.Init((SkillData)dt.Clone(), this);

        img.sprite = _skill.data.spr;
        coolImg.sprite = _skill.data.spr;

        SyncCoolImg();
        SyncCoolText();
        isCool = true;
        img.color = Color.white;
        levelTxt.text = string.Format("+{0}", _skill.data.level);
        levelTxt.enabled = true;
    }

    public void LevelUp(int amount)
    {
        skill.LevelUp(amount);
        SyncCoolImg();
        SyncCoolText();
        levelTxt.text = levelTxt.text = string.Format("+{0}", _skill.data.level);
    }

    [ContextMenu("LevelUpOnce")]
    public void LevelUpOnce()
    {
        LevelUp(1);
    }

    public void SyncCoolImg()
    {
        if (_skill.data.coolTimeLeft > 0)
        {
            if (!coolImg.enabled)
            {
                coolImg.color = new Color(coolImg.color.r, coolImg.color.g, coolImg.color.b, 1);
                coolImg.enabled = true;
                coolImg.type = Image.Type.Filled;
                coolImg.fillOrigin = 2;
                coolImg.fillClockwise = false;
            }
            coolImg.fillAmount = _skill.data.coolTimeLeft / _skill.data.coolTime;
        }
        else
            coolImg.enabled = false;
    }

    public void SyncCoolText()
    {
        coolTimeTxt.text = _skill.data.coolTimeLeft.ToString("##0.00") + "/" + _skill.data.coolTime.ToString("##0.##");
        if (!coolTimeTxt.enabled)
            coolTimeTxt.enabled = true;
    }

    public void UseSkill()
    {
        if (_skill.data.coolTimeLeft > 0)
            return;
        Vector3 mp = BoardManager.GetMousePosition() + new Vector3(0, pointer.offset, 0);
        float distance = Vector2.Distance(mp, player.transform.position);
        Vector2 vec = mp - player.transform.position;

        _skill.data.coolTimeLeft = _skill.data.coolTime;

        UseMana();

        if (_skill.data.isCircleToPlayer && distance > _skill.data.size)
        {
            distance = _skill.data.size;
            mp = (Vector2)player.transform.position + vec.normalized * distance;
        }
        _skill.Use(mp);
        if(_skill.data.useSFX.CompareTo(string.Empty) != 0)
            AudioManager.instance.PlaySFX(_skill.data.useSFX);

        Debug.Log(_skill.data.name + " Skill Used!");
    }
}