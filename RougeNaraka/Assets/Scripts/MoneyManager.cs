using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    public Text goldTxt;
    public Text soulTxt;
    public int gold
    {
        get { return _gold; }
    }
    [SerializeField][ReadOnly]
    private int _gold;

    public int collectedSoul
    {
        get { return _collectedSoul; }
    }
    [SerializeField][ReadOnly]
    private int _collectedSoul;
    public int soul
    {
        get { return _soul; }
    }
    [SerializeField][ReadOnly]
    private int _soul;

    public static MoneyManager instance = null;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void SetGold(int value)
    {
        _gold = value;
        MoneyUpdate();
    }

    public void AddGold(int value)
    {
        if (_gold + value >= 0)
            _gold += value;
        else
            _gold = 0;
        MoneyUpdate();
        PointTxtManager.instance.TxtOnGold(value, goldTxt.transform);
    }

    public bool UseGold(int amount)
    {
        if (_gold - amount >= 0)
        {
            _gold -= amount;
            MoneyUpdate();
            PointTxtManager.instance.TxtOnGold(-amount, goldTxt.transform);
            return true;
        }
        else
            return false;
    }

    public void SetCollectedSoul(int value)
    {
        _collectedSoul = value;
    }

    public void AddCollectedSoul(int value)
    {
        if (_collectedSoul + value >= 0)
            _collectedSoul += value;
        else
            _collectedSoul = 0;
        MoneyUpdate();
        PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform);
    }

    public void SetSoul(int value)
    {
        _soul = value;
        MoneyUpdate();
    }

    public void AddSoul(int value)
    {
        if (_soul + value >= 0)
            _soul += value;
        else
            _soul = 0;
        MoneyUpdate();
        PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform);
    }

    public bool UseSoul(int amount)
    {
        if (_soul - amount >= 0)
        {
            _soul -= amount;
            MoneyUpdate();
            PointTxtManager.instance.TxtOnSoul(-amount, soulTxt.transform);
            return true;
        }
        else
            return false;
    }

    public void CollectedSoulToSoul()
    {
        if(_collectedSoul > 0)
            AddSoul(_collectedSoul);
        SetCollectedSoul(0);
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("gold", _gold);
        PlayerPrefs.SetInt("collectedSoul", _collectedSoul);
        PlayerPrefs.SetInt("soul", _soul);
    }

    public void Load()
    {
        SetGold(PlayerPrefs.GetInt("gold"));
        SetCollectedSoul(PlayerPrefs.GetInt("collectedSoul"));
        SetSoul(PlayerPrefs.GetInt("soul"));
    }

    [ContextMenu("SoulUp")]
    public void SoulUp()
    {
        Debug.Log("soulUp");
        AddSoul(10);
    }

    public void MoneyUpdate()
    {
        goldTxt.text = _gold.ToString();
        soulTxt.text = _soul.ToString() + "(" + collectedSoul + ")";
    }
}
