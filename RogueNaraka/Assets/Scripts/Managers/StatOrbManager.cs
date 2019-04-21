﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BansheeGz.BGSpline.Components;
using UnityEngine.UI;
using TMPro;

public class StatOrbManager : MonoBehaviour
{
    public static StatOrbManager instance;
    public GameObject orbPrefab;
    public GameObject pnl;
    public GameObject endPoint;

    public Fade fade;

    public ParticleSystem bombParticle;

    public ObjectPool orbPool;
    public Image icon;
    public Image smallIcon;
    public Sprite[] icons;
    public Sprite[] smallIcons;

    public TextMeshProUGUI statNameTxt;
    public TextMeshProUGUI statValueTxt;

    public List<StatOrb> list = new List<StatOrb>();

    STAT currentStat;
    Stat rndStat;
    public Stat stat { get { return _stat; } } 
    Stat _stat;

    int used;
    int current;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            orbPool.EnqueueObjectPool(Instantiate(orbPrefab));
        }
    }

    public void SpawnOrb(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject orb = orbPool.DequeueObjectPool();
            orb.transform.localPosition = Vector3.zero;
            list.Add(orb.GetComponent<StatOrb>());
            orb.SetActive(true);
        }
    }

    public void Shoot(int n, float delay)
    {
        if(list.Count >= n)
            StartCoroutine(ShootCorou(n, delay));
    }

    public void Shoot(StatOrb orbRoot)
    {
        orbRoot.startPoint.transform.position = orbRoot.orb.transform.position;
        orbRoot.endPoint.transform.position = endPoint.transform.position;
        orbRoot.trs.MoveObject = true;
        orbRoot.trs.DistanceRatio = 0;
        orbRoot.trs.SetOnOverflow(OnOverflow, orbRoot.gameObject);
        orbRoot.rigid.velocity = Vector2.zero;
        AudioManager.instance.PlaySFX("statFire");
    }

    IEnumerator ShootCorou(int n, float delay)
    {
        for (int i = 0; i < n; i++)
        {
            Shoot(list[list.Count - 1]);
            list.RemoveAt(list.Count - 1);
            //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
            float t = delay;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
            }
        }
    }

    void OnOverflow(GameObject obj)
    {
        _stat.AddOrigin(currentStat, 1);
        StatTxtUpdate();
        used++;
        current--;
        IconEffect();
        bombParticle.Play();
        if(used == rndStat.statPoints || (currentStat == STAT.MPREGEN && stat.mpRegen == stat.mpRegenMax))
        {
            StartCoroutine(OnLastOverflow());
        }
        AudioManager.instance.PlaySFX("statDestroy");
        orbPool.EnqueueObjectPool(obj);
    }



    IEnumerator OnLastOverflow()
    {
        //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
        float t = 1;
        while (t > 0)
        {
            yield return null;
            t -= Time.deltaTime;
        }
        _stat.currentHp = _stat.GetCurrent(STAT.HP);
        _stat.currentMp = _stat.GetCurrent(STAT.MP);
        pnl.SetActive(false);
        Stat.StatToData(_stat);
        Stat.StatToData(null, "randomStat");
        
        for(int i = 0; i < list.Count; i++)
        {
            orbPool.EnqueueObjectPool(list[i].gameObject);
        }

        GameManager.instance.StatTextUpdate(stat);

        if (GameManager.instance.IsFirstGame)
            RollManager.instance.FirstGame();
        else
            RollManager.instance.FirstRoll();
        //GameManager.instance.RunGame(stat);
    }

    float iconTime;
    float size = 1;
    IEnumerator iconEffectCorou;

    void IconEffect()
    {
        if(size < 2)
            size += 0.05f;
        icon.rectTransform.localScale = new Vector3(size, size, 0);
        iconTime = 0.5f;
        if (iconEffectCorou == null)
        {
            iconEffectCorou = IconEffectCorou();
            StartCoroutine(iconEffectCorou);
        }
    }
    IEnumerator IconEffectCorou()
    {
        while(iconTime > 0)
        {
            yield return null;
            iconTime -= Time.deltaTime;
            icon.rectTransform.localScale = Vector3.Lerp(icon.rectTransform.localScale, Vector3.one, 1 - iconTime * 2);
        }
        icon.rectTransform.localScale = Vector3.one;
        iconEffectCorou = null;
    }

    public void SetStat(STAT type)
    {
        currentStat = type;
        int i = (int)type;
        icon.sprite = icons[i];
        smallIcon.sprite = smallIcons[i];
        StatTxtUpdate();
    }

    public void SetActive(bool value, Stat rndStat = null, Stat stat = null)
    {
        if (value && rndStat != null && stat != null)
        {
            used = 0;
            this.rndStat = rndStat;
            this._stat = stat;
            StatTxtUpdate();
            pnl.SetActive(true);
            SpawnOrb(rndStat.statPoints);
            StartCoroutine(StatCorou(rndStat));
            GameManager.instance.SetPause(false);

            TutorialManager.instance.StartTutorial(1);
            bombParticle.gameObject.SetActive(true);

            fade.FadeIn();
        }
        else if (!value)
        {
            pnl.SetActive(false);
            fade.FadeOut();
        }
    }

    public void OnFadeOut()
    {
        bombParticle.gameObject.SetActive(false);
    }

    IEnumerator StatCorou(Stat stat)
    {
        for(int i = 0; i <= (int)STAT.MPREGEN; i++)
        {
            float t = 0.5f;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
            }
            //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
            StartCoroutine(IconShake(0.1f, 0.1f, 0.001f));
            SetStat((STAT)i);
            AudioManager.instance.PlaySFX("statChange");

            int amount = (int)stat.GetOrigin((STAT)i);
            current = amount;
            if (amount > 0)
            {
                float delay = Mathf.Pow(0.75f, amount);

                Shoot(amount, delay);

                while (current > 0)
                {
                    yield return null;
                }
            }
        }
    }

    private IEnumerator IconShake(float time, float power, float gap)
    {
        float t1 = 0, t2 = 0;
        if (gap <= 0)
            gap = 0.001f;
        Vector3 origin = icon.rectTransform.position;
        while (t1 <= time)
        {
            Vector3 random = origin + new Vector3(Random.Range(-power, power), Random.Range(-power, power), origin.z);
            icon.rectTransform.position = random;

            while (t2 <= gap)
            {
                yield return null;
                t1 += Time.deltaTime;
                t2 += Time.deltaTime;
            }
            t2 = 0;

            icon.rectTransform.position = origin;
        }
    }

    void StatTxtUpdate()
    { 
        statNameTxt.text = string.Format("{0}\n({1})", GameDatabase.instance.statLang[(int)GameManager.language].items[(int)currentStat], currentStat.ToString());
        statValueTxt.text = string.Format("{0}/{1}", _stat.GetOrigin(currentStat), _stat.GetMax(currentStat));
    }
}
