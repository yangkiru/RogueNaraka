using System.Collections;
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

    public ObjectPool orbPool;
    public Image icon;
    public Sprite[] icons;

    public TextMeshProUGUI statNameTxt;
    public TextMeshProUGUI statValueTxt;

    public List<StatOrb> list = new List<StatOrb>();

    STAT currentStat;
    Stat rndStat;
    Stat stat;

    int used;

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
                t -= Time.unscaledDeltaTime;
            }
        }
    }

    void OnOverflow(GameObject obj)
    {
        stat.AddOrigin(currentStat, 1);
        StatTxtUpdate();
        used++;
        IconEffect();
        if(used == rndStat.statPoints)
        {
            StartCoroutine(OnLastOverflow());
        }
        orbPool.EnqueueObjectPool(obj);
    }

    IEnumerator OnLastOverflow()
    {
        //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
        float t = 1;
        while (t > 0)
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        }
        stat.currentHp = stat.GetCurrent(STAT.HP);
        stat.currentMp = stat.GetCurrent(STAT.MP);
        pnl.SetActive(false);
        Stat.StatToData(stat);
        Stat.StatToData(null, "randomStat");
        
        GameManager.instance.RunGame(stat);
    }

    float iconTime;
    float size = 1;
    IEnumerator iconEffectCorou;

    void IconEffect()
    {
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
            iconTime -= Time.unscaledDeltaTime;
            icon.rectTransform.localScale = Vector3.Lerp(icon.rectTransform.localScale, Vector3.one, 1 - iconTime * 2);
        }
        icon.rectTransform.localScale = Vector3.one;
        iconEffectCorou = null;
    }

    public void SetStat(STAT type)
    {
        currentStat = type;
        icon.sprite = icons[(int)type];
        StatTxtUpdate();
    }

    public void SetActive(bool value, Stat rndStat = null, Stat stat = null)
    {
        if (value && rndStat != null && stat != null)
        {
            used = 0;
            this.rndStat = rndStat;
            this.stat = stat;
            StatTxtUpdate();
            pnl.SetActive(true);
            SpawnOrb(rndStat.statPoints);
            StartCoroutine(StatCorou(rndStat));
        }
        else if (!value)
            pnl.SetActive(false);
    }

    IEnumerator StatCorou(Stat stat)
    {
        for(int i = 0; i <= (int)STAT.MPREGEN; i++)
        {
            float t = 0.5f;
            while (t > 0)
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            }
            //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
            StartCoroutine(IconShake(0.1f, 0.1f, 0.001f));
            SetStat((STAT)i);

            int amount = (int)stat.GetOrigin((STAT)i);

            if (amount > 0)
            {
                float delay = Mathf.Pow(0.75f, amount);

                Shoot(amount, delay);

                t = delay;
                while (t > 0)
                {
                    yield return null;
                    t -= Time.unscaledDeltaTime;
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
        statValueTxt.text = string.Format("{0}/{1}", stat.GetOrigin(currentStat), stat.GetMax(currentStat));
    }
}
