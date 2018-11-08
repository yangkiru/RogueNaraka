using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour {

    public TextMeshProUGUI soulTxt;
    public TextMeshProUGUI unrefinedSoulTxt;

    private Vector2 soulSpawnPosition;

    public int unrefinedSoul
    {
        get { return _unrefinedSoul; }
    }
    [SerializeField][ReadOnly]
    private int _unrefinedSoul;
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

    public void SetUnrefinedSoul(int value)
    {
        _unrefinedSoul = value;
    }

    public void AddUnrefinedSoul(int value)
    {
        if (_unrefinedSoul + value >= 0)
            _unrefinedSoul += value;
        else
            _unrefinedSoul = 0;
        MoneyUpdate();
        PointTxtManager.instance.TxtOnSoul(value, unrefinedSoulTxt.transform, soulSpawnPosition);
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
        PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform, soulSpawnPosition);
    }

    public bool UseSoul(int amount)
    {
        if (_soul - amount >= 0)
        {
            _soul -= amount;
            MoneyUpdate();
            PointTxtManager.instance.TxtOnSoul(-amount, soulTxt.transform, soulSpawnPosition);
            return true;
        }
        else
            return false;
    }

    public void RefineSoul()
    {
        if(_unrefinedSoul > 0)
            AddSoul(_unrefinedSoul);
        SetUnrefinedSoul(0);
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("unrefinedSoul", _unrefinedSoul);
        PlayerPrefs.SetInt("soul", _soul);
    }

    public void Load()
    {
        SetUnrefinedSoul(PlayerPrefs.GetInt("unrefinedSoul"));
        SetSoul(PlayerPrefs.GetInt("soul"));
    }

    public void Reset()
    {
        PlayerPrefs.SetInt("unrefinedSoul", 0);
        PlayerPrefs.SetInt("soul", 0);
    }

    [ContextMenu("SoulUp")]
    public void SoulUp()
    {
        Debug.Log("soulUp");
        AddSoul(10);
    }

    public void MoneyUpdate()
    {
        soulTxt.text = _soul.ToString();
        unrefinedSoulTxt.text = _unrefinedSoul.ToString();
    }
}
