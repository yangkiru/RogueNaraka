using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill : MonoBehaviour, IBeginDragHandler, IDragHandler, 
    IEndDragHandler
{

    public Image img;
    public Image coolImg;
    public Text levelTxt;
    public Text coolTimeTxt;
    public SkillData data;
    public int position;
    public bool isCool;

    private SkillManager skillManager
    { get { return SkillManager.instance; } }
    private Player player
    { get { return Player.instance; } }
    //public bool isEnter;

    public void OnEnable()
    {
        if (data.coolTimeLeft > 0)
            StartCoroutine(CoolTime());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data.id != -1)
        {
            skillManager.DrawLine(position, true);
            skillManager.SetLine(true);
            if (data.distance > 0)
            {
                skillManager.GetCircle().SetCircle(data.distance);
                skillManager.GetCircle().SetEnable(true);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (data.id != -1 && (!player.isDeath || data.isDeath))
        {
            SkillManager.instance.DrawLine(position, true);
            if (data.isCircleToPlayer)
            {
                skillManager.GetCircle().SetParent(player.transform);
                skillManager.GetCircle().Move(Vector2.zero);
            }
            else
            {
                skillManager.GetCircle().MoveCircleToMouse();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        skillManager.SetLine(false);
        skillManager.GetCircle().SetEnable(false);
        skillManager.GetCircle().transform.SetParent(null);
        if (data.id != -1 && BoardManager.IsMouseInBoard() && (!player.isDeath || data.isDeath) && IsMana())
        {
            if (data.coolTimeLeft <= 0)
                UseSkill();
            else
                Debug.Log(name + " CoolTime! : " + data.coolTimeLeft);
        }
    }
    
    public void UseMana()
    {
        player.UseMana(data.manaCost);
    }

    public bool IsMana()
    {
        return player.mana >= data.manaCost;
    }

    public void UseSkill()
    {
        Vector3 mp = BoardManager.GetMousePosition();
        float distance = Vector2.Distance(mp, player.transform.position);
        Vector2 vec = mp - player.transform.position;
        Vector2 pos = mp;

        if (data.coolTime > 0)
        {
            data.coolTimeLeft = data.coolTime;
            coolImg.enabled = true;
            StartCoroutine(CoolTime());
        }

        UseMana();

        if (distance > data.distance)
        {
            distance = data.distance;
            pos = (Vector2)player.transform.position + vec.normalized * distance;
        }
        switch (data.id)
        {
            case 0://Roll
                Roll(pos);
                break;
            case 1://Bandage
                Bandage(data.values[0].value);
                break;
        }

        Debug.Log(data.name + " Skill Used!");
    }

    public void Init()
    {
        isCool = true;
        img.sprite = null;
        img.color = Color.clear;
        coolImg.sprite = null;
        coolImg.enabled = false;
        coolTimeTxt.text = string.Empty;
        levelTxt.enabled = false;
        ResetData();
        //Debug.Log(name + " Init");
    }

    public void SyncData(SkillData dt)
    {
        data = dt;
        
        img.sprite = data.spr;
        coolImg.sprite = data.spr;

        SyncCoolImg();
        SyncCoolText();
        isCool = true;
        img.color = Color.white;
        levelTxt.text = data.level.ToString();
        levelTxt.enabled = true;
    }

    public void ResetData()
    {
        data.Reset();
    }

    public void SyncData(int id)
    {
        SyncData(GameDatabase.instance.skills[id]);
    }

    [ContextMenu("LevelUp")]
    public void LevelUpOnce()
    {
        LevelUp(1);
    }
    public void LevelUp(int amount)
    {
        data.level += amount;
        levelTxt.text = data.level.ToString();
        for(int i = 0; i < amount; i++)
        {
            data.manaCost += data.levelUp.manaCost;
            data.distance += data.levelUp.distance;
            AddEffect(data.effects, data.levelUp.effects);
        }
        SyncCoolText();
        SyncCoolImg();
    }

    public void AddEffect(EffectData[] def, EffectData[] ef)
    {
        for (int i = 0; i < ef.Length; i++)
        {
            bool isFound = false;
            for (int j = 0; j < def.Length; j++)
            {
                if (ef[i].type == def[j].type)
                {
                    isFound = true;
                    def[j].value += ef[i].value;
                    def[j].time += ef[i].time;
                    def[j].isInfinity = ef[i].isInfinity;
                }
                if (!isFound)
                {
                    def = new EffectData[def.Length + 1];
                    def[def.Length - 1] = ef[i];
                }
            }
        }
    }

    private void SyncCoolImg()
    { 
        if (data.coolTimeLeft > 0)
        {
            if (!coolImg.enabled)
            {
                coolImg.enabled = true;
                coolImg.type = Image.Type.Filled;
                coolImg.fillOrigin = 2;
                coolImg.fillClockwise = false;
            }
            coolImg.fillAmount = data.coolTimeLeft / data.coolTime;
        }
        else
            coolImg.enabled = false;
    }

    private void SyncCoolText()
    {
        coolTimeTxt.text = data.coolTimeLeft.ToString("N1") + "/" + data.coolTime.ToString("N1");
        if (!coolTimeTxt.enabled)
            coolTimeTxt.enabled = true;
    }

    public IEnumerator CoolTime()
    {
        while (data.coolTimeLeft > 0)
        {
            yield return null;
            data.coolTimeLeft -= Time.deltaTime;
            SyncCoolImg();
            SyncCoolText();
        }
        data.coolTimeLeft = 0;
        SyncCoolImg();
        SyncCoolText();
    }

    float originSpeed = 0;
    public void Roll(Vector3 mp)
    {
        Debug.Log("Roll!");
        Player player = Player.instance;
        originSpeed = player.GetOriginSpeed();
        player.attackable = false;
        player.SetSpeed(originSpeed * 2);
        player.isAutoMove = false;
        player.Move(mp);
        player.agent.OnDestinationInvalid += EndRoll;
        player.agent.OnDestinationReached += EndRoll;
    }

    public void EndRoll()
    {
        Debug.Log("End Roll!");
        Player player = Player.instance;
        player.attackable = true;
        player.SetSpeed(originSpeed);
        player.isAutoMove = true;
        player.agent.OnDestinationInvalid -= EndRoll;
        player.agent.OnDestinationReached -= EndRoll;
    }

    public void Bandage(float amount)
    {
        Player.instance.HealHealth(amount);
    }
}
