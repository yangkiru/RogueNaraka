using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.UnitScripts;
using TMPro;

public class BoardManager : MonoBehaviour {

    public static BoardManager instance = null;
    public GameObject unitPrefab;
    public GameObject bulletPrefab;
    public GameObject[] bossPrefabs;
    public GameObject effectPrefab;
    public GameObject soulPrefab;

    public Fade fade;

    public Unit player;
    public Unit boss;
    public Vector2 spawnPoint;//player spawn
    public Vector2 goalPoint;//next Stage
    public Vector2 bossPoint;
    public Vector2[] boardRange;//0:min, 1:max
    public ObjectPool unitPool;
    const int unitObjCount = 100;
    public ObjectPool bulletPool;
    const int bulletObjCount = 250;
    public ObjectPool effectPool;
    const int effectObjCount = 200;
    public ObjectPool soulPool;
    const int soulObjCount = 50;

    public List<Unit> enemies = new List<Unit>();
    public List<Unit> friendlies = new List<Unit>();

    public TextMeshProUGUI stageTxt;

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

    public GameObject SpawnObj(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, unitPool.transform);
        return obj;
    }

    public void InitBoard()
    {
        Debug.Log("InitBoard");
        int count = unitPool.GetCount();
        if (count < unitObjCount)//unit Pooling
        {
            for (int i = count; i < unitObjCount; i++)
            {
                GameObject obj = SpawnObj(unitPrefab);
                unitPool.EnqueueObjectPool(obj);
            }
        }
        count = bulletPool.GetCount();
        if (count < bulletObjCount)//bullet Pooling
        {
            for (int i = count; i < bulletObjCount; i++)
            {
                GameObject obj = SpawnObj(bulletPrefab);
                bulletPool.EnqueueObjectPool(obj);
            }
        }
        count = effectPool.GetCount();
        if (count < effectObjCount)//effect Pooling
        {
            for (int i = count; i < effectObjCount; i++)
            {
                GameObject obj = SpawnObj(effectPrefab);
                effectPool.EnqueueObjectPool(obj);
            }
        }
        count = soulPool.GetCount();
        if (count < soulObjCount)//soul Pooling
        {
            for (int i = count; i < soulObjCount; i++)
            {
                GameObject obj = SpawnObj(soulPrefab);
                soulPool.EnqueueObjectPool(obj);
            }
        }

        //GameManager.instance.SetPause(true);
        InitStage(_stage);
        player.Spawn(spawnPoint);
        isReady = true;
        fade.FadeIn();
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

    public void Save()
    {
        PlayerPrefs.SetInt("stage", stage);
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
        if(stage != 0 && stage % 30 == 0)
        {
            SpawnBoss(0);
            AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomBossMusic());
            
            StartCoroutine(StageTxtEffect(true));
            return;
        }
        else
        {
            if(AudioManager.instance.currentMusic.CompareTo(string.Empty)!=0)
                AudioManager.instance.PlayMusic(AudioManager.instance.currentMusic);
            else
                AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomMusic());
        }


        int cost = GameDatabase.instance.stageCosts[(stage - 1) % 30];
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
                if (GameDatabase.instance.enemies[temp].stage <= (_stage % 30))
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
                if (GameDatabase.instance.enemies[temp].stage <= _stage)
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
        player.cashedTransform.SetParent(unitPool.transform);
        player.cashedTransform.SetSiblingIndex(0);
    }

    public void SpawnEnemy(int id)
    {
        //Debug.Log(id + " Enemies Spawned");
        Unit enemy = unitPool.DequeueObjectPool().GetComponent<Unit>();
        enemy.Init(GameDatabase.instance.enemies[id]);
        enemy.stat.dmg *= RageManager.instance.enemiesDmg;
        enemy.stat.hp *= RageManager.instance.enemiesHp;
        enemy.stat.currentHp *= RageManager.instance.enemiesHp;
        enemy.Spawn(GetRandomSpawn());
    }

    public void SpawnBoss(int id)
    {
        //Debug.Log(id + " Boss Spawned");
        Unit boss = unitPool.DequeueObjectPool().GetComponent<Unit>();
        this.boss = boss;
        boss.Init(GameDatabase.instance.bosses[id]);
        boss.stat.dmg *= RageManager.instance.enemiesDmg;
        boss.stat.hp *= RageManager.instance.enemiesHp;
        boss.stat.currentHp *= RageManager.instance.enemiesHp;
        boss.Spawn(bossPoint);
        Fillable.bossHp.gameObject.SetActive(true);
    }

    public Vector2 GetRandomSpawn()
    {
        float radius = PolyNav.PolyNav2D.current.radiusOffset;
        float x = Random.Range(boardRange[0].x + radius, boardRange[1].x - radius);
        float y = Random.Range(boardRange[0].y + radius, boardRange[1].y - radius);
        return new Vector2(x, y);
    }

    private IEnumerator StageTxtEffect(bool isBoss = false)
    {
        Debug.Log("stageTxt");
        stageTxt.gameObject.SetActive(true);
        string text = isBoss ? "BOSS STAGE" : string.Format("STAGE {0}", _stage);
        stageTxt.text = string.Empty;
        Color color = Color.white;
        stageTxt.color = color;
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
