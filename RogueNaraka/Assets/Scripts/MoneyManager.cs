using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    public TMPro.TextMeshProUGUI soulTxt;

    private TxtHolder soulHolder = new TxtHolder();

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
        PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform, soulHolder);
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
        PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform, soulHolder);
    }

    public bool UseSoul(int amount)
    {
        if (_soul - amount >= 0)
        {
            _soul -= amount;
            MoneyUpdate();
            PointTxtManager.instance.TxtOnSoul(-amount, soulTxt.transform, soulHolder);
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
        PlayerPrefs.SetInt("collectedSoul", _collectedSoul);
        PlayerPrefs.SetInt("soul", _soul);
    }

    public void Load()
    {
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
        soulTxt.text = _soul.ToString() + "(" + collectedSoul + ")";
    }
}
