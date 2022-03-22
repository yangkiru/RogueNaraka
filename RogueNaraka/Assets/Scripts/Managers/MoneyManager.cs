using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

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
    public float RemainSoul { get { return remainSoul; } set { remainSoul = value; } }
    private float remainSoul = 0;

    public float refiningRate { get { return PlayerPrefs.GetFloat("refiningRate"); } set { PlayerPrefs.SetFloat("refiningRate", value); } }

    public static MoneyManager instance = null;

    private bool loaded = false;
    public bool Loaded { get {return this.loaded; } }

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
        MoneyUpdate();
    }

    public void AddUnrefinedSoul(int value)
    {
        if (_unrefinedSoul + value >= 0)
            _unrefinedSoul += value;
        else
            _unrefinedSoul = 0;
        MoneyUpdate();
        if(value != 0)
            PointTxtManager.instance.TxtOnSoul(value, unrefinedSoulTxt.transform, soulSpawnPosition);
    }

    public void SetSoul(int value)
    {
        _soul = value;
        MoneyUpdate();
    }

    public void AddSoul(int value, bool isSave = true)
    {
        if (_soul + value >= 0)
            _soul += value;
        else
            _soul = 0;
        MoneyUpdate();
        if(value != 0)
            PointTxtManager.instance.TxtOnSoul(value, soulTxt.transform, Vector2.zero);
        if(isSave)
            Save(false);
    }

    public bool UseSoul(int amount)
    {
        if (IsUseable(amount))
        {
            _soul -= amount;
            MoneyUpdate();
            PointTxtManager.instance.TxtOnSoul(-amount, soulTxt.transform, Vector2.zero);
            Save(false);
            return true;
        }
        else
            return false;
    }

    public bool IsUseable(int amount)
    {
        return _soul - amount >= 0;
    }

    public void RefineSoul(float rate = 1)
    {
        if(_unrefinedSoul > 0)
            AddSoul((int)(_unrefinedSoul * rate));
        SetUnrefinedSoul(0);
        Save();
        Debug.Log("RefineSoul");
    }

    public float GetRandomRefiningRate()
    {
        float rate = Random.Range(refiningRate, 1f);
        return rate;
    }

    public void Save(bool isUnrefined = true)
    {
        if(isUnrefined)
            PlayerPrefs.SetInt("unrefinedSoul", _unrefinedSoul);
        PlayerPrefs.SetInt("soul", _soul);
    }

    public void Load()
    {
        SetUnrefinedSoul(PlayerPrefs.GetInt("unrefinedSoul"));
        SetSoul(PlayerPrefs.GetInt("soul"));
        this.loaded = true;
    }

    public void ResetData()
    {
        PlayerPrefs.SetInt("unrefinedSoul", 0);
        PlayerPrefs.SetInt("soul", 0);
    }

    public void MoneyUpdate()
    {
        //Debug.Log("MoneyUpdate");
        soulTxt.text = string.Format("Coin {0}", _soul.ToString());
    }
}
