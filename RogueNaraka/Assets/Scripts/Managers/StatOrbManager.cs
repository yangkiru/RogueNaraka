using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatOrbManager : MonoBehaviour
{
    public static StatOrbManager instance;
    public GameObject orbPrefab;
    public GameObject endPrefab;
    public GameObject potPrefab;

    public GameObject potImg;

    public ObjectPool orbPool;
    public ObjectPool endPool;
    public ObjectPool potPool;
    public Transform[] stat;
    public Transform from;
    public Color[] statColor;

    public int orbPotAmount = 50;
    public float rndPower = 0.1f;

    [SerializeField]
    List<OrbPotParticle> potList = new List<OrbPotParticle>();

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            orbPool.EnqueueObjectPool(Instantiate(orbPrefab));
            endPool.EnqueueObjectPool(Instantiate(endPrefab));
        }
        for (int i = 0; i < orbPotAmount; i++)
        {
            potPool.EnqueueObjectPool(Instantiate(potPrefab));
        }
    }

    public IEnumerator ActivePot(bool value)
    {
        if(value)
        {
            potImg.gameObject.SetActive(true);
            for (int i = 0; i < orbPotAmount; i++)
            {
                OrbPotParticle particle = potPool.DequeueObjectPool().GetComponent<OrbPotParticle>();
                potList.Add(particle);
                particle.gameObject.SetActive(true);
                int rnd = Random.Range(1, 11);
                for(int j = 0; j < rnd; j++)
                    yield return null;
            }
        }
        else
        {
            for (int i = 0; i < potList.Count; i++)
            {
                potPool.EnqueueObjectPool(potList[i].gameObject);
            }
            potList.Clear();
            potImg.gameObject.SetActive(false);
        }
    }

    public void AddStat(STAT type, Stat stat = null, System.Action<STAT, Stat> onEnd = null)
    {
        GameObject obj = orbPool.DequeueObjectPool();
        StatOrb orb = obj.GetComponent<StatOrb>();
        orb.Play(type, stat, onEnd);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.Alpha0))
    //        AddStat(STAT.DMG);
    //    if (Input.GetKeyUp(KeyCode.Alpha1))
    //        AddStat(STAT.SPD);
    //    if (Input.GetKeyUp(KeyCode.Alpha2))
    //        AddStat(STAT.TEC);
    //    if (Input.GetKeyUp(KeyCode.Alpha3))
    //        AddStat(STAT.HP);
    //    if (Input.GetKeyUp(KeyCode.Alpha4))
    //        AddStat(STAT.HPREGEN);
    //    if (Input.GetKeyUp(KeyCode.Alpha5))
    //        AddStat(STAT.MP);
    //    if (Input.GetKeyUp(KeyCode.Alpha6))
    //        AddStat(STAT.MPREGEN);
    //}
}
