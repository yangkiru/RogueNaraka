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
            if (data.size > 0)
            {
                skillManager.GetCircle().SetCircle(data.size);
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
        for (int i = 0; i < amount; i++)
        {
            data.manaCost += data.levelUp.manaCost;
            data.size += data.levelUp.distance;
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
            thunder.Init(data.bulletIds[rndDirection], player.data.stat.tec, true);
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
        newData.abilities[0].value = data.values[0].value;
        newData.effects[0].value = data.effects[0].value;
        newData.effects[0].time = data.effects[0].time;
        ice.Init(newData, player.data.stat.tec, true);
        ice.Attack((Vector2)mp, Vector2.zero, Vector2.zero, 0, 0, null, player);
    }
}
