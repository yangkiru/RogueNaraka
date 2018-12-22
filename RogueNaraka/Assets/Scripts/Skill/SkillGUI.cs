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
    public Pointer pointer;
    public Image img;
    public Image coolImg;
    public TextMeshProUGUI levelTxt;
    public Text coolTimeTxt;
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

    public void OnMouseDown()
    {
        if (_skill.data.id != -1 && !GameManager.instance.isPause)
        {
            isMouseDown = true;
            //skillManager.DrawLine(position, true);
            //skillManager.SetLine(true);
            if (_skill.data.size > 0)
            {
                skillManager.GetCircle().SetCircle(_skill.data.size);
                skillManager.GetCircle().SetEnable(true);
            }
            pointer.SetPointer(true);
        }
    }

    public void OnMouseUp()
    {
        isMouseDown = false;
        skillManager.SetLine(false);
        skillManager.GetCircle().SetEnable(false);
        skillManager.GetCircle().transform.SetParent(null);
        if (_skill.data.id != -1 && !GameManager.instance.isPause && BoardManager.IsMouseInBoard() && (!player.deathable.isDeath || _skill.data.isDeath) && IsMana())
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
        levelTxt.gameObject.SetActive(false);
        if (_skill)
            Destroy(_skill);
    }

    public void Init(SkillData dt)
    {
        if(_skill)
        {
            Destroy(_skill);
        }

        System.Type type = System.Type.GetType(((SKILL_ID)dt.id).ToString());
        _skill = gameObject.AddComponent(type) as Skill;

        _skill.Init((SkillData)dt.Clone());

        img.sprite = _skill.data.spr;
        coolImg.sprite = _skill.data.spr;

        SyncCoolImg();
        SyncCoolText();
        isCool = true;
        img.color = Color.white;
        levelTxt.text = string.Format("+{0}", _skill.data.level);
        levelTxt.gameObject.SetActive(true);
    }

    public void LevelUp(int amount)
    {
        skill.LevelUp(amount);
        SyncCoolImg();
        SyncCoolText();
        levelTxt.text = levelTxt.text = string.Format("+{0}", _skill.data.level);
    }

    private void SyncCoolImg()
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

    private void SyncCoolText()
    {
        coolTimeTxt.text = _skill.data.coolTimeLeft.ToString("##0.00") + "/" + _skill.data.coolTime.ToString("##0.##");
        if (!coolTimeTxt.enabled)
            coolTimeTxt.enabled = true;
    }

    public IEnumerator CoolTime()
    {
        while (_skill.data.coolTimeLeft > 0)
        {
            yield return null;
            _skill.data.coolTimeLeft -= Time.deltaTime;
            SyncCoolImg();
            SyncCoolText();
        }
        _skill.data.coolTimeLeft = 0;
        SyncCoolImg();
        SyncCoolText();
    }

    public void UseSkill()
    {
        if (_skill.data.coolTimeLeft > 0)
            return;
        Vector3 mp = BoardManager.GetMousePosition() + new Vector3(0, pointer.offset, 0);
        float distance = Vector2.Distance(mp, player.transform.position);
        Vector2 vec = mp - player.transform.position;

        if (_skill.data.coolTime > 0)
        {
            _skill.data.coolTimeLeft = _skill.data.coolTime;
            StartCoroutine(CoolTime());
        }

        UseMana();

        if (_skill.data.isCircleToPlayer && distance > _skill.data.size)
        {
            distance = _skill.data.size;
            mp = (Vector2)player.transform.position + vec.normalized * distance;
        }
        _skill.Use(mp);

        Debug.Log(_skill.data.name + " Skill Used!");
    }

    

    //void BloodSwamp(Vector3 mp)
    //{
    //    for (int i = 0; i < data.values[0].value; i++)//values[1] == blood spawn amount
    //    {
    //        float rndAngle = Random.Range(0, 360);
    //        Vector2 rndPos = new Vector2(Random.Range(-data.size +1.5f, data.size -1.5f), Random.Range(-data.size +1.5f, data.size -1.5f));
    //        Bullet blood = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<OldBullet>();
    //        BulletData newData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
    //        newData.dealSpeed = data.values[1].value;//dealSpeed
    //        blood.Init(newData, 0, true);
    //        blood.OnDamaged += SpawnBloodBubble;
    //        StartCoroutine(MeltDown(blood.renderer, newData.abilities[0].value));
    //        blood.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
    //        blood.Attack((Vector2)mp + rndPos, Vector2.zero, Vector2.zero, 0, 0, null, player);
    //    }
    //}

    //void SpawnBloodBubble(Bullet parent, Unit target)
    //{
    //    Bullet bubble = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
    //    Vector2 rndPos = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
    //    bubble.Init(data.bulletIds[1], 0, false);
    //    bubble.OnDamaged += OnBloodBubbleHit;
    //    bubble.Attack((Vector2)target.transform.position + rndPos, Vector2.zero, (player.transform.position - target.transform.position).normalized, 2, 0, null, target);
    //}

    //void OnBloodBubbleHit(Bullet parent, Unit target)
    //{
    //    if (target != null)
    //    {
    //        if(!parent.owner.isDeath)
    //            parent.owner.GetDamage(data.values[2].value);//values[2] == blood life steal
    //        if(!target.isDeath)
    //            target.HealHealth(data.values[2].value);
    //    }
    //}

    //void Genesis(Vector3 mp)
    //{
    //    Bullet beam = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<OldBullet>();
    //    BulletData beamData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());

    //    Bullet hole = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<OldBullet>();
    //    BulletData holeData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[1]].Clone());

    //    beamData.abilities[0].value += data.values[0].value;//time
    //    beamData.shake.time = beamData.abilities[0].value;
    //    float originalHoleTime = holeData.abilities[0].value;
    //    holeData.abilities[0].value += beamData.abilities[0].value;

    //    beam.Init(beamData, data.values[1].value * player.data.stat.tec, true);
    //    hole.Init(holeData, 0, true);
    //    //renderer, 기존 hole 시간, 빔 시간 - 기존 hole 시간
    //    StartCoroutine(MeltDown(hole.renderer, originalHoleTime, beamData.abilities[0].value));

    //    beam.Attack((Vector2)mp + new Vector2(0, 1.8f), Vector2.zero, Vector2.left, 0, 0, null, player);
    //    hole.Attack((Vector2)mp + new Vector2(0, 0.5f), Vector2.zero, Vector2.left, 0, 0, null, player);
    //}

    //void ScarecrowSoldier(Vector3 mp)
    //{
    //    //Unit soldier = BoardManager.instance.enemyPool.DequeueObjectPool().GetComponent<Unit>();
    //    Unit soldier = Instantiate(data.units[0], mp, Quaternion.identity).GetComponent<Unit>();
    //    UnitData newData = (UnitData)GameDatabase.instance.spawnables[0].Clone();

    //    newData.stat.dmg = data.values[1].value;//dmg
    //    newData.stat.hp = data.values[2].value;//hp
    //    //soldier.transform.position = mp;
    //    soldier.SyncData(newData, true);
    //    soldier.gameObject.SetActive(true);
    //    soldier.StartCoroutine(soldier.TimeLimit(data.values[0].value));//time
    //}

    //IEnumerator MeltDown(SpriteRenderer renderer, float time, float wait = 0)
    //{
    //    yield return new WaitForSeconds(wait);
    //    float alpha = renderer.color.a;
    //    float down = alpha / time;
    //    Color color = Color.white;
    //    while (time > 0)
    //    {
    //        color = renderer.color;
    //        alpha -= down * Time.deltaTime;
    //        color.a = alpha;
    //        time -= Time.deltaTime;
    //        renderer.color = color;
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(0.1f);
    //    color.a = 1;
    //    renderer.color = color;
    //}
}