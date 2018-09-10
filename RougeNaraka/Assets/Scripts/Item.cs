using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [SerializeField][ReadOnly]
    private ItemData data;

    public SpriteRenderer spriteRenderer;

    public void SyncData(ItemData dt)
    {
        data.name = dt.name;
        data.id = dt.id;
        data.spriteId = dt.spriteId;
        data.values = (ValueData[])dt.values.Clone();
        data.isKnown = dt.isKnown;
    }

    public void SyncData(int id)
    {
        SyncData(GameDatabase.instance.items[id]);
    }

    public void SyncSprite()
    {
        spriteRenderer.sprite = GameDatabase.instance.itemSprites[data.spriteId].spr;
    } 

    public void SpawnItem(ItemData dt)
    {
        SyncData(dt);
        SyncSprite();
    }

    public void SpawnRandomItem()
    {
        int rnd = Random.Range(0, GameDatabase.instance.items.Length + 1);
    }
}
