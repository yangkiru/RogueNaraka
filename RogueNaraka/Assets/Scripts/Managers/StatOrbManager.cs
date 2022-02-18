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

    public Fade fade;

    public ParticleSystem bombParticle;

    public ObjectPool orbPool;
    public Image icon;
    public Image smallIcon;
    public Sprite[] icons;
    public Sprite[] smallIcons;

    public TextMeshProUGUI statNameTxt;
    public TextMeshProUGUI statValueTxt;

    private List<List<StatOrb>> statOrblistForShoot = new List<List<StatOrb>>();
    private int statOrblistForShootTotalCount { 
        get {
            int count = 0;
            for(int i = 0; i < this.statOrblistForShoot.Count; ++i) {
                count += this.statOrblistForShoot[i].Count;
            }
            return count;
        }
    }

    STAT currentStat;
    Stat resultStatOfShootStatOrb;
    public Stat MaxStat { get { return this.maxStat; } } 
    Stat maxStat;

    int used;
    int current;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            orbPool.EnqueueObjectPool(Instantiate(orbPrefab));
        }
        for (int i = 0; i <= (int)STAT.MR; ++i) {
            this.statOrblistForShoot.Add(new List<StatOrb>());
        }
    }

    public void SpawnOrb(Stat _resultStatOfShootStatOrb)
    {
        int addStatValue = 0;
        int statPoints = 0;

        for(int i = 0; i <= (int)STAT.MR; ++i) {
            statPoints = (int)_resultStatOfShootStatOrb.GetOrigin((STAT)i);
            addStatValue = GetFirstStatValueForStatOrb(_resultStatOfShootStatOrb.statPoints);

            for (int remainStatPoints = statPoints; remainStatPoints > 0; remainStatPoints -= addStatValue) {
            if(remainStatPoints < addStatValue) {
                    addStatValue = GetStatValueForStatOrb(remainStatPoints);
                }
                StatOrb orb = orbPool.DequeueObjectPool().GetComponent<StatOrb>();
                orb.Init(addStatValue);
                orb.transform.localPosition = Vector3.zero;
                orb.gameObject.SetActive(true);
                orb.GetComponent<StatOrb>().rigid.velocity = Random.insideUnitCircle * 5f;
                statOrblistForShoot[i].Add(orb);
            }
        }
    }

    public void Shoot(int n, float delay, STAT _type)
    {
        
        if(statOrblistForShootTotalCount >= n) {
            StartCoroutine(ShootCorou(n, delay, _type));
        }
    }

    public void Shoot(StatOrb orbRoot)
    {
        orbRoot.startPoint.transform.position = orbRoot.StatOrbImage.transform.position;
        orbRoot.endPoint.transform.position = endPoint.transform.position;
        orbRoot.trs.MoveObject = true;
        orbRoot.trs.DistanceRatio = 0;
        orbRoot.trs.SetOnOverflow(OnOverflow, orbRoot.gameObject);
        orbRoot.rigid.velocity = Vector2.zero;
        AudioManager.instance.PlaySFX("statFire");
    }

    IEnumerator ShootCorou(int n, float delay, STAT _type)
    {
        for (int i = 0; i < n; i++)
        {
            Shoot(this.statOrblistForShoot[(int)_type][this.statOrblistForShoot[(int)_type].Count - 1]);
            this.statOrblistForShoot[(int)_type].RemoveAt(this.statOrblistForShoot[(int)_type].Count - 1);
            //CameraShake.instance.Shake(0.1f, 0.1f, 0.001f);
            float t = delay;
            while (t > 0)
            {
                yield return null;
                t -= Time.deltaTime;
            }
        }
    }

    void OnOverflow(GameObject _objOfOrb)
    {
        StatOrb orb = _objOfOrb.GetComponent<StatOrb>();
        this.maxStat.AddOrigin(currentStat, orb.Value);
        StatTxtUpdate();
        used += orb.Value;
        current--;
        IconEffect();
        bombParticle.Play();
        if(used == this.resultStatOfShootStatOrb.statPoints || (currentStat == STAT.MR && this.maxStat.mpRegen == this.maxStat.mpRegenMax))
        {
            StartCoroutine(OnLastOverflow());
        }
        AudioManager.instance.PlaySFX("statDestroy");
        orbPool.EnqueueObjectPool(_objOfOrb);
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
        this.maxStat.currentHp = this.maxStat.GetCurrent(STAT.HP);
        this.maxStat.currentMp = this.maxStat.GetCurrent(STAT.MP);
        pnl.SetActive(false);
        Stat.StatToData(this.maxStat);
        Stat.StatToData(null, "randomStat");
        
        for(int statIdx = 0; statIdx < statOrblistForShoot.Count; statIdx++)
        {
            for(int i = 0; i < statOrblistForShoot[statIdx].Count; ++i) {
                orbPool.EnqueueObjectPool(statOrblistForShoot[statIdx][i].gameObject);
            }
        }

        GameManager.instance.StatTextUpdate(this.maxStat);

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
        size = 1;
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

    public void SetActive(bool _value, Stat _resultStatOfShootStatOrb = null, Stat _maxStat = null)
    {
        if (_value && _resultStatOfShootStatOrb != null && _maxStat != null)
        {
            used = 0;
            this.resultStatOfShootStatOrb = _resultStatOfShootStatOrb;
            this.maxStat = _maxStat;
            StatTxtUpdate();
            pnl.SetActive(true);
            SpawnOrb(_resultStatOfShootStatOrb);
            StartCoroutine(StatCorou(_resultStatOfShootStatOrb));
            GameManager.instance.SetPause(false);

            //TutorialManager.instance.StartTutorial(1);
            bombParticle.gameObject.SetActive(true);

            fade.FadeIn();
        }
        else if (!_value)
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
        for(int i = 0; i <= (int)STAT.MR; i++)
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

            int amount = this.statOrblistForShoot[i].Count;
            current = amount;
            if (amount > 0)
            {
                float delay = Mathf.Pow(0.75f, amount);

                Shoot(amount, delay, (STAT)i);

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
        statValueTxt.text = string.Format("{0}/{1}", this.maxStat.GetOrigin(currentStat), this.maxStat.GetMax(currentStat));
    }
    
    private int GetFirstStatValueForStatOrb(int _statPoint) {
        int idx_forFirstStat = _statPoint / 100;
        const int MAX_INDEX = 10;
        idx_forFirstStat = idx_forFirstStat > MAX_INDEX ? MAX_INDEX : idx_forFirstStat;
        switch(idx_forFirstStat) {
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 4;
            case 3:
                return 6;
            case 4:
                return 8;
            case 5:
                return 10;
            case 6:
                return 12;
            case 7:
                return 14;
            case 8:
                return 16;
            case 9:
                return 18;
            case 10:
                return 20;
        }
        throw new System.ArgumentException(string.Format("Invalid StatPoint! : {0}", _statPoint));
    }

    private int GetStatValueForStatOrb(int _statPoint) {
        int statValue = (_statPoint / 2) * 2;
        statValue = statValue <= 0 ? 1 : statValue;
        return statValue;
    }
}
