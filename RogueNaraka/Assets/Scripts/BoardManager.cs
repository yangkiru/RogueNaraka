using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    public static BoardManager instance = null;
    public GameObject enemyPrefab;
    public GameObject bulletPrefab;
    public GameObject[] bossPrefabs;
    public List<Enemy> enemies = new List<Enemy>();
    public Player player;
    public Vector2 spawnPoint;//player spawn
    public Vector2 goalPoint;//next Stage
    public Vector2 bossPoint;
    public Vector2[] boardRange;//0:min, 1:max
    public ObjectPool enemyPool;//basic 100 counts
    public ObjectPool bulletPool;//basic 200 counts
    [ReadOnly]
    public Enemy boss;

    public Text stageTxt;

    public int stage
    { get { return _stage; } }
    [SerializeField][ReadOnly]
    private int _stage;
    public bool isReady;

    private void Awake()
    {
        if (instance == null)
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

    public GameObject SpawnEnemyObj()
    {
        GameObject obj = Instantiate(enemyPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), enemyPool.transform);
        return obj;
    }

    public GameObject SpawnBulletObj()
    {
        GameObject obj = Instantiate(bulletPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), bulletPool.transform);
        return obj;
    }

    public void InitBoard()
    {
        //Debug.Log("InitBoard");
        int count = enemyPool.GetCount();
        if (count < 100)//enemy Pooling
        {
            for (int i = count; i < 100; i++)
            {
                GameObject obj = SpawnEnemyObj();
                enemyPool.EnqueueObjectPool(obj);
            }
        }
        count = bulletPool.GetCount();
        if (count < 200)//bullet Pooling
        {
            for (int i = count; i < 200; i++)
            {
                GameObject obj = SpawnBulletObj();
                bulletPool.EnqueueObjectPool(obj);
            }
        }

        //GameManager.instance.SetPause(true);
        InitStage(_stage);
        player.Respawn();
        StartCoroutine(WaitForLoad());
    }

    private IEnumerator WaitForLoad()
    {
        yield return new WaitForSecondsRealtime(1);
        GameManager.instance.SetPause(false);
        isReady = true;
    }
    /// <summary>
    /// stage = value;
    /// </summary>
    /// <param name="value"></param>
    public void SetStage(int value)
    {
        _stage = value;
    }

    public void StageUp()
    {
        _stage++;
    }

    private void InitStage(int stage)
    {
        isReady = false;
        for (int i = 0; i < bulletPool.transform.childCount; i++)
        {
            GameObject obj = bulletPool.transform.GetChild(i).gameObject;
            if (obj.activeSelf)
            {
                obj.GetComponent<Bullet>().RevolveFunction();
                bulletPool.EnqueueObjectPool(obj);
            }
        }
        //Debug.Log("SetStage(" + stage + ")");
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

    public void SpawnEnemy(int id)
    {
        //Debug.Log(id + " Enemies Spawned");
        Enemy spawn = enemyPool.DequeueObjectPool().GetComponent<Enemy>();
        spawn.transform.position = GetRandomSpawn();
        spawn.SyncData(GameDatabase.instance.enemies[id]);
        spawn.gameObject.SetActive(true);
        enemies.Add(spawn);
    }

    public void SpawnBoss(int id)
    {
        Debug.Log(id + " Boss Spawned");
        boss = Instantiate(bossPrefabs[id], bossPoint, new Quaternion(0, 0, 0, 0)).GetComponent<Enemy>();
        boss.SyncData(GameDatabase.instance.bosses[id]);
        boss.gameObject.SetActive(true);
        enemies.Add(boss);
    }

    public Vector2 GetRandomSpawn()
    {
        float radius = PolyNav.PolyNav2D.current.radiusOffset;
        float x = Random.Range(boardRange[0].x + radius, boardRange[1].x - radius);
        float y = Random.Range(boardRange[0].y + radius, boardRange[1].y - radius);
        return new Vector2(x, y);
    }

    private IEnumerator StageTxtEffect()
    {
        float appearTime = 0.95f, disappearTime = 0.07f;
        string text = "STAGE " + _stage.ToString();
        stageTxt.text = string.Empty;
        //Appear
        for (int i = 0; i < text.Length; i++)
        {
            stageTxt.text += text[i];
            yield return new WaitForSecondsRealtime(appearTime / text.Length);
        }
        int alpha = 255;
        while (alpha >= 0)
        {
            alpha -= 10;
            stageTxt.color = new Color(stageTxt.color.r, stageTxt.color.g, stageTxt.color.b, alpha / 255f);
            yield return new WaitForSecondsRealtime(disappearTime);
        }
    }

    public void ClearStage()
    {
        enemies.Clear();
        enemyPool.Clear();
    }
    public static Vector3 GetMousePosition()
    {
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(mp.x, mp.y, 0);
    }

    public static bool IsMouseInBoard()
    {
        Vector3 mp = GetMousePosition();
        Vector3 min = BoardManager.instance.boardRange[0];
        Vector3 max = BoardManager.instance.boardRange[1];
        return mp.x > min.x && mp.y > min.y && mp.x < max.x && mp.y < max.y;
    }
}
