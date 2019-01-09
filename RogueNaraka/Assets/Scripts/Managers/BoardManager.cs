using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;

public class BoardManager : MonoBehaviour {

    public static BoardManager instance = null;
    public GameObject unitPrefab;
    public GameObject bulletPrefab;
    public GameObject[] bossPrefabs;
    public GameObject effectPrefab;

    public Unit player;
    public Vector2 spawnPoint;//player spawn
    public Vector2 goalPoint;//next Stage
    public Vector2 bossPoint;
    public Vector2[] boardRange;//0:min, 1:max
    public ObjectPool unitPool;//basic 100 counts
    public ObjectPool bulletPool;//basic 500 counts
    public ObjectPool effectPool;//basic 200 counts

    public List<Unit> enemies = new List<Unit>();
    public List<Unit> friendlies = new List<Unit>();

    public Text stageTxt;

    public int stage
    { get { return _stage; } }
    [SerializeField]
    private int _stage;
    public bool isReady;

    private void Awake()
    {
        instance = this;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPoint, 0.1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(goalPoint, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(bossPoint, 0.1f);
        if (boardRange.Length > 1)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(boardRange[0], 0.1f);
            Gizmos.DrawWireSphere(boardRange[1], 0.1f);
        }
    }

    public GameObject SpawnUnitObj()
    {
        GameObject obj = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity, unitPool.transform);
        return obj;
    }

    public GameObject SpawnBulletObj()
    {
        GameObject obj = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity, bulletPool.transform);
        return obj;
    }

    public GameObject SpawnEffectObj()
    {
        GameObject obj = Instantiate(effectPrefab, Vector3.zero, Quaternion.identity, effectPool.transform);
        return obj;
    }

    public void InitBoard()
    {
        Debug.Log("InitBoard");
        int count = unitPool.GetCount();
        if (count < 100)//unit Pooling
        {
            for (int i = count; i < 100; i++)
            {
                GameObject obj = SpawnUnitObj();
                unitPool.EnqueueObjectPool(obj);
            }
        }
        count = bulletPool.GetCount();
        if (count < 500)//bullet Pooling
        {
            for (int i = count; i < 500; i++)
            {
                GameObject obj = SpawnBulletObj();
                bulletPool.EnqueueObjectPool(obj);
            }
        }
        count = effectPool.GetCount();
        if (count < 200)//effect Pooling
        {
            for (int i = count; i < 200; i++)
            {
                GameObject obj = SpawnEffectObj();
                effectPool.EnqueueObjectPool(obj);
            }
        }
        
        //GameManager.instance.SetPause(true);
        InitStage(_stage);
        player.Spawn(spawnPoint);
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return null;
        GameManager.instance.SetPause(false);
        isReady = true;
    }
    /// <summary>
    /// stage = value;
    /// </summary>
    /// <param name="value"></param>
    public void SetStage(int value)
    {
        ClearStage();
        _stage = value;
    }

    public void StageUp()
    {
        _stage++;
    }

    //void RandomEnemy(int leftCost)
    //{
    //    List<UnitData> list = new List<UnitData>();
    //    int max = GameDatabase.instance.unitCosts[Unitcost]
    //    int cost = Random.Range(1, leftCost)
    //}

    private void InitStage(int stage)
    {
        isReady = false;
        for (int i = 0; i < bulletPool.transform.childCount; i++)
        {
            GameObject obj = bulletPool.transform.GetChild(i).gameObject;
            if (obj.activeSelf)
            {
                bulletPool.EnqueueObjectPool(obj);
            }
        }
        //Debug.Log("SetStage(" + stage + ")");
        if(stage > GameDatabase.instance.stageCosts.Length)
        {
            Debug.Log("NoMoreStage");
            GameManager.instance.StartCoroutine(GameManager.instance.OnEnd());
            return;
        }
        int cost = GameDatabase.instance.stageCosts[stage - 1];
        UnitCost[] unitCosts = GameDatabase.instance.unitCosts;
        List<int> able = new List<int>();
        for(int i = 0; i < unitCosts.Length; i++)
        {
            if (unitCosts[i].cost <= cost)
                able.Add(unitCosts[i].cost);
        }
        for (int i = able.Count-1; i >= 1; i--)
        {
            int amount = Random.Range(0, (cost / able[i]) + 1);
            List<int> ids = new List<int>();

            for(int j = 0; j < unitCosts[i].unitId.Length; j++)//stage 제한
            {
                int temp = unitCosts[i].unitId[j];
                if (GameDatabase.instance.enemies[temp].stage < _stage)
                    ids.Add(temp);
                
            }
            if (ids.Count > 0)
            {
                for (int j = 0; j < amount; j++)
                {
                    int index = Random.Range(0, ids.Count);
                    SpawnEnemy(ids[index]);
                    cost -= unitCosts[i].cost;
                }
            }
            if (cost <= 1)
                break;
        }
        for (int i = 0; i < cost; i++)
        {
            List<int> ids = new List<int>();
            for (int j = 0; j < unitCosts[0].unitId.Length; j++)//stage 제한
            {
                int temp = unitCosts[0].unitId[j];
                if (GameDatabase.instance.enemies[temp].stage < _stage)
                    ids.Add(temp);
            }
            if (ids.Count > 0)
            {
                int index = Random.Range(0, ids.Count);
                //Debug.Log("index:" + index + " ids.Count:" + ids.Count + " ids[index]:" + ids[index]);
                SpawnEnemy(ids[index]);
            }
        }

        StartCoroutine(StageTxtEffect());
    }

    public void SpawnPlayer(UnitData data)
    {
        if(player == null)
            player = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
        player.Init(data);
        player.Spawn(spawnPoint);
    }

    public void SpawnEnemy(int id)
    {
        //Debug.Log(id + " Enemies Spawned");
        Unit enemy = unitPool.DequeueObjectPool().GetComponent<Unit>();
        enemy.Init(GameDatabase.instance.enemies[id]);
        enemy.Spawn(GetRandomSpawn());
    }

    //public void SpawnBoss(int id)
    //{
    //    Debug.Log(id + " Boss Spawned");
    //    boss = Instantiate(bossPrefabs[id], bossPoint, new Quaternion(0, 0, 0, 0)).GetComponent<Enemy>();
    //    boss.SyncData(GameDatabase.instance.bosses[id]);
    //    boss.gameObject.SetActive(true);
    //    enemies.Add(boss);
    //}

    public Vector2 GetRandomSpawn()
    {
        float radius = PolyNav.PolyNav2D.current.radiusOffset;
        float x = Random.Range(boardRange[0].x + radius, boardRange[1].x - radius);
        float y = Random.Range(boardRange[0].y + radius, boardRange[1].y - radius);
        return new Vector2(x, y);
    }

    private IEnumerator StageTxtEffect()
    {
        Debug.Log("stageTxt");
        stageTxt.gameObject.SetActive(true);
        string text = string.Format("STAGE {0}", _stage);
        stageTxt.text = string.Empty;
        stageTxt.color = Color.white;
        yield return null;
        //Appear
        for (int i = 0; i < text.Length; i++)
        {
            float t = 0.1f;
            while (t > 0)
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            }
            stageTxt.text = string.Format("{0}{1}", stageTxt.text, text[i]);
        }
        float alpha = 1;
        float amount = 5 / 255f;
        while (alpha >= 0)
        {
            alpha -= amount;
            stageTxt.color = new Color(stageTxt.color.r, stageTxt.color.g, stageTxt.color.b, alpha);
            yield return null;
        }
        stageTxt.gameObject.SetActive(false);
    }

    public void ClearStage()
    {
        Debug.Log("Clear stage " + enemies.Count + " enemies");
        for(int i = enemies.Count - 1; i >= 0; i--)
            unitPool.EnqueueObjectPool(enemies[i].gameObject);

        for (int i = friendlies.Count - 1; i >= 0; i--)
        {
            if (friendlies[i].Equals(player))
                continue;
            unitPool.EnqueueObjectPool(friendlies[i].gameObject);
        }
    }
    public static Vector3 GetMousePosition()
    {
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mp.x, mp.y, 0);
    }

    public static bool IsMouseInBoard()
    {
        Vector3 mp = GetMousePosition() + new Vector3(0, Pointer.instance.offset, 0);
        Vector3 min = BoardManager.instance.boardRange[0];
        Vector3 max = BoardManager.instance.boardRange[1];
        return mp.x > min.x && mp.y > min.y && mp.x < max.x && mp.y < max.y;
    }
}
