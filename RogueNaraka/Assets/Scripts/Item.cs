using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static Item instance = null;
    [SerializeField][ReadOnly]
    private ItemData data;

    public CircleRenderer circle;
    public LineRenderer line;

    public Image img;
    public int[] sprIds;
    public bool[] isKnown;

    public RectTransform[] points;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void InitItem()
    {
        data.id = -1;
        img.color = Color.clear;
    }

    public void ResetSave()
    {
        PlayerPrefs.SetInt("isItemFirst", 0);
        PlayerPrefs.SetInt("item", -1);
        PlayerPrefs.SetString("itemSpr", string.Empty);
        PlayerPrefs.SetString("itemIsKnow", string.Empty);
    }

    public void Save()
    {
        //현재 데이터 저장
        PlayerPrefs.SetInt("item", data.id);
        PlayerPrefs.SetString("itemSpr", JsonHelper.ToJson<int>(sprIds));
        PlayerPrefs.SetString("itemIsKnown", JsonHelper.ToJson<bool>(isKnown));
    }

    public void Load()
    {
        if (PlayerPrefs.GetInt("isItemFirst") == 0)//처음
        {
            SetRandomSprite();
            isKnown = new bool[GameDatabase.instance.items.Length];
            InitItem();
            Save();
            PlayerPrefs.SetInt("isItemFirst", 1);
        }
        else
        {
            int itemData = PlayerPrefs.GetInt("item");
            string sprData = PlayerPrefs.GetString("itemSpr");
            string isKnownData = PlayerPrefs.GetString("itemIsKnown");
            //Debug.Log(itemData);
            if (sprData != string.Empty)
            {
                sprIds = JsonHelper.FromJson<int>(sprData);
                if (GameDatabase.instance.itemSprites.Length != sprIds.Length)//DB와 크기 불일치
                    UpdateRandomSprite();//크기 맞추기, 감소했을 경우 오류 위험
            }
            else//초기화가 안되어 있는데 데이터가 없을 경우는 오류가 아닐까
                SetRandomSprite();
            if (isKnownData != string.Empty)
            {
                isKnown = JsonHelper.FromJson<bool>(isKnownData);
                if(isKnown.Length != GameDatabase.instance.items.Length)//DB와 크기 불일치
                {
                    List<bool> temp = new List<bool>();
                    for(int i = 0; i < GameDatabase.instance.items.Length; i++)
                    {
                        if (i < isKnown.Length)
                            temp.Add(isKnown[i]);//작으면 그대로 삽입
                        else
                            temp.Add(false);//크면 false로 초기화
                    }
                    isKnown = temp.ToArray();
                }
            }
            else//isKnown은 항상 값이 있어야하는데 아마도 이건 오류가 아닐까
            {
                isKnown = new bool[GameDatabase.instance.items.Length];
            }
            if (itemData != -1)
            {
                SyncData(GameDatabase.instance.items[itemData]);
                SyncSprite();
            }
            else
                data.id = -1;
        }
    }

    private void SetRandomSprite()
    {
        sprIds = new int[GameDatabase.instance.itemSprites.Length];
        List<int> temp = new List<int>();
        for (int i = 0; i < sprIds.Length; i++)
            temp.Add(i);
        int leng = temp.Count;
        for (int i = 0; i < leng; i++)
        {
            int rnd = Random.Range(0, temp.Count);
            sprIds[i] = temp[rnd];
            temp.RemoveAt(rnd);
        }
    }

    private void UpdateRandomSprite()
    {
        int update = GameDatabase.instance.itemSprites.Length;
        int last = sprIds.Length;
        int dif = update - last;
        List<int> newSpriteIds = new List<int>();
        if(dif < 0)//Sprite 감소
        {
            for(int i = 0; i < sprIds.Length; i++)
                if (sprIds[i] < update)
                    newSpriteIds.Add(sprIds[i]);
            sprIds = newSpriteIds.ToArray();
        }
        else//Sprite 증가
        {
            for(int i = 0; i < sprIds.Length;i++)
                newSpriteIds.Add(sprIds[i]);
            List<int> temp = new List<int>();
            for(int i = 0; i < dif; i++)
                temp.Add(i);
            for(int i = 0; i < dif; i++)
            {
                int rnd = Random.Range(0, temp.Count);
                newSpriteIds.Add(temp[rnd]);
                temp.RemoveAt(rnd);
            }
            sprIds = newSpriteIds.ToArray();
        }
    }

    public ItemData GetData(int id)
    {
        return GameDatabase.instance.items[id];
    }

    public void SyncData(ItemData dt)
    {
        data = dt;
    }

    public void SyncData(int id)
    {
        SyncData(GetData(id));
    }

    public void SyncSprite()
    {
        img.sprite = GameDatabase.instance.itemSprites[sprIds[data.id]].spr;
        img.color = Color.white;
    }

    [ContextMenu("Spawn")]
    public void SpawnRandomItem()
    {
        int rnd = Random.Range(0, GameDatabase.instance.items.Length);
        SyncData(rnd);
        SyncSprite();
    }

    public void UseItem()
    {
        circle.SetCircle(data.size);
        circle.MoveCircleToMouse();
        isKnown[data.id] = true;
        Collider2D[] hits = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(
            Input.mousePosition), data.size, GameDatabase.instance.unitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            Unit unit = hits[i].GetComponent<Unit>();
            switch (data.id)
            {
                case 0://HealPotion
                    Debug.Log("Heal");
                    unit.HealHealth(data.value);
                    break;
                case 1:
                    Debug.Log("FragGrenade");
                    unit.GetDamage(data.value);
                    unit.KnockBack(unit.transform.position - BoardManager.GetMousePosition(), (int)(data.value/2));
                    break;
                case 2:
                    Debug.Log("HighExplosive");
                    unit.GetDamage(data.value / 2);
                    unit.KnockBack(unit.transform.position - BoardManager.GetMousePosition(), (int)data.value);
                    break;
            }
        }
        InitItem();
    }

    private void DrawLine()
    {
        points[0].position = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        points[2].position = new Vector3(mp.x, mp.y, 0);
        float mid = (BoardManager.instance.boardRange[0].x + BoardManager.instance.boardRange[1].x) / 2;
        points[1].position = new Vector3((mid + mp.x) / 2, (points[0].position.y + points[2].position.y) / 2, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data.id != -1)
        {
            circle.MoveCircleToMouse();
            circle.SetCircle(data.size);
            circle.SetEnable(true);
            line.enabled = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (data.id != -1)
        {
            circle.MoveCircleToMouse();
            DrawLine();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        line.enabled = false;
        circle.SetEnable(false);
        if (data.id != -1)
        {
            if(BoardManager.IsMouseInBoard())
            {
                UseItem();
            }
        }
    }
}
