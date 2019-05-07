using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;
using RogueNaraka.BulletScripts;
using RogueNaraka.SkillScripts;

public class SkillGUI : MonoBehaviour
{
    public static int pointedSkill = -1;
    public int PointedSkill { get { return pointedSkill; } set { pointedSkill = value; } }

    public Pointer pointer;
    public Image img;
    public Image coolImg;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI coolTimeTxt;
    public int position;
    public bool isCool;

    private SkillManager skillManager
    { get { return SkillManager.instance; } }
    private Unit player
    { get { return BoardManager.instance.player; } }

    public Skill skill { get { return _skill; } }
    Skill _skill;

    public void OnEnter()
    {
        pointedSkill = position;
        if (RollManager.instance.selectPnl.activeSelf && _skill)
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

    public void OnExit()
    {
        pointedSkill = -1;
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

    public void OnDown()
    {
        if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause)
        {
            if (_skill.data.size != 0)
            {
                skillManager.circle.SetCircle(_skill.data.size);
                skillManager.circle.SetEnable(true);
                if (skill.data.isCircleToPlayer)
                    skillManager.circle.SetParent(BoardManager.instance.player.cachedTransform);
                else
                    skillManager.circle.SetParent(pointer.cashedTransform);
            }
            Pointer.instance.SetPointer(true);
            Pointer.instance.PositionToMouse();
            ManaScript.instance.SetNeedMana(true, _skill.data.manaCost);
        }
    }

    //IEnumerator doubleClickCorou;

    //IEnumerator DoubleClickCorou()
    //{
    //    float t = 0.2f;
    //    isDoubleClick = false;
    //    do
    //    {
    //        yield return null;
    //        t -= Time.unscaledDeltaTime;
    //        if (isDoubleClick)
    //        {
    //            if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause && (!player.deathable.isDeath || _skill.data.isDeath) && IsMana())
    //                UseSkill();
    //            Pointer.instance.SetPointer(false);
    //            isDoubleClick = false;
    //            isMouseDown = false;
    //        }
    //    } while (t > 0);
    //    yield return null;
    //    doubleClickCorou = null;
    //}

    public void OnDrag()
    {
        if (_skill && _skill.data.id != -1 && ((player && !player.deathable.isDeath) || _skill.data.isDeath) && !GameManager.instance.isPause)
        {
            if (_skill.data.size != 0)
            {
                if (skill.data.isCircleToPlayer)
                    skillManager.circle.SetParent(BoardManager.instance.player.cachedTransform);
                else
                    skillManager.circle.SetParent(pointer.cashedTransform);
            }
            pointer.PositionToMouse();
        }
    }

    public void OnUp()
    {
        skillManager.circle.SetEnable(false);
        ManaScript.instance.SetNeedMana(false);
        if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause && BoardManager.IsMouseInBoard() && ((player && !player.deathable.isDeath) || _skill.data.isDeath) && IsMana())
        {
            UseSkill();
        }
        pointer.SetPointer(false);
    }

    public void OnEnterUp()
    {
        ManaScript.instance.SetNeedMana(false);
        if (_skill && _skill.data.id != -1 && !GameManager.instance.isPause && _skill.data.size == 0 && ((player && !player.deathable.isDeath) || _skill.data.isDeath) && IsMana())
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
            coolImg.enabled = true;
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
        Vector3 mp = GameManager.instance.GetMousePosition() + new Vector2(0, pointer.offset);
        float distance = Vector2.Distance(mp, player.transform.position);
        Vector2 vec = mp - player.transform.position;

        _skill.data.coolTimeLeft = _skill.data.coolTime;

        UseMana();

        if (_skill.data.isCircleToPlayer && distance > _skill.data.size)
        {
            distance = _skill.data.size;
            mp = (Vector2)player.transform.position + vec.normalized * distance;
        }
        mp = BoardManager.instance.ClampToBoard(mp);
        _skill.Use(mp);
        if(_skill.data.useSFX.CompareTo(string.Empty) != 0)
            AudioManager.instance.PlaySFX(_skill.data.useSFX);

        Debug.Log(_skill.data.name + " Skill Used!");
    }
}