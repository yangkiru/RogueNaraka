using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    [SerializeField][ReadOnly]
    private ItemData data;

    public Image img;
    public int[] spriteIds;

    private void Awake()
    {
        SpawnRandomItem();
    }

    private void Init()
    {
        //int lastLength = 로드 할 스프라이트의 길이
        int currentLength = GameDatabase.instance.items.Length;
        spriteIds = new int[GameDatabase.instance.items.Length];
    }

    private void SetRandomSprite()
    {
        List<int> temp = new List<int>();
        for(int i = 0; i < spriteIds.Length; i++)
            temp.Add(i);
        for(int i = 0; i < temp.Count; i++)
        {
            int rnd = Random.Range(0, temp.Count);
        }
    }


    public ItemData GetData(int id)
    {
        return GameDatabase.instance.items[id];
    }

    public void SyncData(ItemData dt)
    {
        data.name = dt.name;
        data.id = dt.id;
        data.spriteId = dt.spriteId;
        data.value = dt.value;
        data.isKnown = dt.isKnown;
        data.amount = dt.amount;
    }

    public void SyncData(int id)
    {
        SyncData(GetData(id));
    }

    public void SyncSprite()
    {
        img.sprite = GameDatabase.instance.itemSprites[data.spriteId].spr;
    }

    public void SpawnRandomItem()
    {
        int rnd = Random.Range(0, GameDatabase.instance.items.Length + 1);
        SyncData(rnd);
    }
}
