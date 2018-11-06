using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{

    public Image img;
    public Image coolImg;
    public Text levelTxt;
    public Text coolTimeTxt;
    public SkillData data;
    public int position;
    public bool isCool;

    bool isMouseDown;

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

    void Update()
    {
        if (isMouseDown)
            OnMouse();
    }

    public void OnMouse()
    {
        if (data.id != -1 && (!player.isDeath || data.isDeath))
        {
            //SkillManager.instance.DrawLine(position, true);
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

    public void OnMouseDown()
    {
        if (data.id != -1)
        {
            isMouseDown = true;
            //skillManager.DrawLine(position, true);
            //skillManager.SetLine(true);
            if (data.size > 0)
            {
                skillManager.GetCircle().SetCircle(data.size);
                skillManager.GetCircle().SetEnable(true);
            }
        }
    }

    public void OnMouseUp()
    {
        isMouseDown = false;
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

    public void Init()
    {
        isCool = true;
        img.sprite = null;
        img.color = Color.clear;
        coolImg.sprite = null;
        coolImg.color = Color.clear;
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
        for (int i = 0; i < amount; i++)
        {
            data.manaCost += data.levelUp.manaCost;
            data.size += data.levelUp.size;
            for (int j = 0; j < data.levelUp.values.Length; j++)
            {
                bool isFind = false;
                for (int k = 0; k < data.values.Length; k++)
                {
                    if (data.values[k].name.CompareTo(data.levelUp.values[j].name) == 0)
                    {
                        isFind = true;
                        data.values[k].value += data.levelUp.values[j].value;
                    }
                }
                if (!isFind)
                {
                    System.Array.Resize<ValueData>(ref data.values, data.values.Length + 1);
                    data.values[data.values.Length - 1] = data.levelUp.values[j];
                }
            }
            AddEffect(data.effects, data.levelUp.effects);
        }
        SyncCoolText();
        SyncCoolImg();
    }

    public void AddEffect(EffectData[] def, EffectData[] lef)
    {
        for (int i = 0; i < lef.Length; i++)
        {
            bool isFound = false;
            for (int j = 0; j < def.Length; j++)
            {
                if (lef[i].type == def[j].type)
                {
                    isFound = true;
                    def[j].value += lef[i].value;
                    def[j].time += lef[i].time;
                    def[j].isInfinity = lef[i].isInfinity;
                }
                if (!isFound)
                {
                    System.Array.Resize<EffectData>(ref data.effects, data.effects.Length + 1);
                    def[def.Length - 1] = (EffectData)(lef[i].Clone());
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
                coolImg.color = new Color(coolImg.color.r, coolImg.color.g, coolImg.color.b, 1);
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
        coolTimeTxt.text = data.coolTimeLeft.ToString("##0.00") + "/" + data.coolTime.ToString("##0.##");
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

    public void UseSkill()
    {
        Vector3 mp = BoardManager.GetMousePosition();
        float distance = Vector2.Distance(mp, player.transform.position);
        Vector2 vec = mp - player.transform.position;
        Vector2 pos = mp;

        if (data.coolTime > 0)
        {
            data.coolTimeLeft = data.coolTime;
            StartCoroutine(CoolTime());
        }

        UseMana();

        if (distance > data.size)
        {
            distance = data.size;
            pos = (Vector2)player.transform.position + vec.normalized * distance;
        }
        switch (data.id)
        {
            case (int)SKILL_ID.THUNDER_STRIKE:
                StartCoroutine(ThunderStrike(mp));
                break;
            case (int)SKILL_ID.ICE_BREAK:
                IceBreak(mp);
                break;
            case (int)SKILL_ID.GENESIS:
                Genesis(mp);
                break;
            case (int)SKILL_ID.BLOOD_SWAMP:
                BloodSwamp(mp);
                break;
        }

        Debug.Log(data.name + " Skill Used!");
    }

    IEnumerator ThunderStrike(Vector3 mp)
    {
        for (int i = 0; i < data.values[0].value; i++)
        {
            Vector2 rnd = new Vector2(Random.Range(-data.size, data.size), Random.Range(-data.size, data.size));
            Bullet thunder = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            int rndDirection = Random.Range(0, 2);
            thunder.Init(data.bulletIds[rndDirection], GameManager.GetStat(STAT.TEC), true);
            float rndAngle = Random.Range(0, 360);
            thunder.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
            thunder.Attack((Vector2)mp + rnd, Vector2.zero, Vector2.zero, 0, 0, null, player);
            float delay = data.values[1].value > 0 ? data.values[1].value : 0;

            yield return new WaitForSeconds(delay);
        }
    }

    void IceBreak(Vector3 mp)
    {
        Bullet ice = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        BulletData newData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
        newData.abilities[0].value += data.values[0].value;//time
        newData.effects[0].value += data.effects[0].value;//ice
        ice.Init(newData, 0, true);
        StartCoroutine(MeltDown(ice.renderer, newData.abilities[0].value));
        ice.Attack((Vector2)mp, Vector2.zero, Vector2.zero, 0, 0, null, player);
    }

    void BloodSwamp(Vector3 mp)
    {
        for (int i = 0; i < data.values[1].value; i++)//values[1] == blood spawn amount
        {
            float rndAngle = Random.Range(0, 360);
            Vector2 rndPos = new Vector2(Random.Range(-data.size * 0.3f, data.size * 0.3f), Random.Range(-data.size * 0.3f, data.size * 0.3f));
            Bullet blood = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
            BulletData newData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());
            newData.abilities[0].value += data.values[0].value;//time
            newData.dealSpeed = data.values[2].value;//dealSpeed
            blood.Init(newData, 0, true);
            blood.OnDamaged += SpawnBloodBubble;
            StartCoroutine(MeltDown(blood.renderer, newData.abilities[0].value));
            blood.transform.rotation = Quaternion.Euler(0, 0, rndAngle);
            blood.Attack((Vector2)mp + rndPos, Vector2.zero, Vector2.zero, 0, 0, null, player);
        }
    }

    void SpawnBloodBubble(Bullet parent, Unit target)
    {
        Bullet bubble = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        Vector2 rndPos = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
        bubble.Init(data.bulletIds[1], 0, false);
        bubble.OnDamaged += OnBloodBubbleHit;
        bubble.Attack((Vector2)target.transform.position + rndPos, Vector2.zero, (player.transform.position - target.transform.position).normalized, 2, 0, null, target);
    }

    void OnBloodBubbleHit(Bullet parent, Unit target)
    {
        if (target != null)
        {
            if(!parent.owner.isDeath)
                parent.owner.GetDamage(data.values[3].value);//values[3] == blood life steal
            if(!target.isDeath)
                target.HealHealth(data.values[3].value);
        }
    }

    void Genesis(Vector3 mp)
    {
        Bullet beam = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        BulletData beamData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[0]].Clone());

        Bullet hole = BoardManager.instance.bulletPool.DequeueObjectPool().GetComponent<Bullet>();
        BulletData holeData = (BulletData)(GameDatabase.instance.bullets[data.bulletIds[1]].Clone());

        beamData.abilities[0].value += data.values[0].value;//time
        float originalHoleTime = holeData.abilities[0].value;
        holeData.abilities[0].value += beamData.abilities[0].value;

        beam.Init(beamData, data.values[1].value * player.data.stat.tec, true);
        hole.Init(holeData, 0, true);
        //renderer, 기존 hole 시간, 빔 시간 - 기존 hole 시간
        StartCoroutine(MeltDown(hole.renderer, originalHoleTime, beamData.abilities[0].value));

        beam.Attack((Vector2)mp + new Vector2(0, 1.8f), Vector2.zero, Vector2.left, 0, 0, null, player);
        hole.Attack((Vector2)mp + new Vector2(0, 0.5f), Vector2.zero, Vector2.left, 0, 0, null, player);
    }

    IEnumerator MeltDown(SpriteRenderer renderer, float time, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        float alpha = renderer.color.a;
        float down = alpha / time;
        Color color = Color.white;
        while (time > 0)
        {
            color = renderer.color;
            alpha -= down * Time.deltaTime;
            color.a = alpha;
            time -= Time.deltaTime;
            renderer.color = color;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        color.a = 1;
        renderer.color = color;
    }
}
