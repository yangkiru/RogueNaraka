using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    [SerializeField][ReadOnly]
    private ItemData data;

    public Image img;
    public int[] sprIds;
    public bool[] isKnown;

    private void Awake()
    {
        Load();
    }

    public void ResetSave()
    {
        PlayerPrefs.SetInt("isItemFirst", 0);
        PlayerPrefs.SetString("item", string.Empty);
        PlayerPrefs.SetString("itemSpr", string.Empty);
        PlayerPrefs.SetString("itemIsKnow", string.Empty);
    }

    public void Save()
    {
        //현재 데이터 저장
        if (data.id == -1)
            PlayerPrefs.SetString("item", string.Empty);
        else
            PlayerPrefs.SetString("item", JsonUtility.ToJson(data));
        PlayerPrefs.SetString("itemSpr", JsonHelper.ToJson<int>(sprIds));
        PlayerPrefs.SetString("itemIsKnow", JsonHelper.ToJson<bool>(isKnown));
    }

    public void Load()
    {
        if (PlayerPrefs.GetInt("isItemFirst") == 0)//처음
        {
            SetRandomSprite();
            isKnown = new bool[GameDatabase.instance.items.Length];
            data.id = -1;
            img.color = Color.clear;
            PlayerPrefs.SetInt("isItemFirst", 1);
        }
        else
        {
            string itemData = PlayerPrefs.GetString("item");
            string sprData = PlayerPrefs.GetString("itemSpr");
            string isKnownData = PlayerPrefs.GetString("itemIsKnown");
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
            if (itemData != string.Empty)
            {
                SyncData(JsonUtility.FromJson<ItemData>(itemData));
                SyncSprite();
            }
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
        data.name = dt.name;
        data.id = dt.id;
        data.value = dt.value;
        data.amount = dt.amount;
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

    public void SpawnRandomItem()
    {
        int rnd = Random.Range(0, GameDatabase.instance.items.Length);
        SyncData(rnd);
        SyncSprite();
    }
}
