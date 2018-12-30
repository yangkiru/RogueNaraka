using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatOrbManager : MonoBehaviour
{
    public static StatOrbManager instance;
    public GameObject orbPrefab;
    public GameObject endPrefab;
    public ObjectPool orbPool;
    public ObjectPool endPool;
    public Transform[] stat;
    public Transform from;
    public Color[] statColor;

    public float rndPower = 0.1f;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < 50; i++)
        {
            orbPool.EnqueueObjectPool(Instantiate(orbPrefab));
            endPool.EnqueueObjectPool(Instantiate(endPrefab));
        }
    }

    public void AddStat(STAT stat)
    {
        GameObject obj = orbPool.DequeueObjectPool();
        StatOrb orb = obj.GetComponent<StatOrb>();
        orb.Play(stat);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0))
            AddStat(STAT.DMG);
        if (Input.GetKeyUp(KeyCode.Alpha1))
            AddStat(STAT.SPD);
        if (Input.GetKeyUp(KeyCode.Alpha2))
            AddStat(STAT.TEC);
        if (Input.GetKeyUp(KeyCode.Alpha3))
            AddStat(STAT.HP);
        if (Input.GetKeyUp(KeyCode.Alpha4))
            AddStat(STAT.HPREGEN);
        if (Input.GetKeyUp(KeyCode.Alpha5))
            AddStat(STAT.MP);
        if (Input.GetKeyUp(KeyCode.Alpha6))
            AddStat(STAT.MPREGEN);
    }
}
