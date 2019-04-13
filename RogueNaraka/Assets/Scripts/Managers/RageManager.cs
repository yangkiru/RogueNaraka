﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RageManager : MonoBehaviour
{
    public static RageManager instance;

    public int rageLevel;
    public float enemiesDmg = 1;
    public float enemiesHp = 1;
    public float soul = 1;
    public bool isRage;

    public Button rageBtn;
    public GameObject ragePnl;
    public ParticleSystem[] fireParticles;
    
    public TextMeshProUGUI contentTxt;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void ResetSave()
    {
        PlayerPrefs.SetInt("rageLevel", 0);
    }

    public void Rage()
    {
        Debug.Log("Rage");
        rageLevel = PlayerPrefs.GetInt("rageLevel") + 1;
        PlayerPrefs.SetInt("rageLevel", rageLevel);
        //ragePnl.SetActive(true);

        //PlayerPrefs.SetInt("isRun", 0);

        ResetRage();
        Rage(rageLevel);
        if (rageLevel > 0)
            SetActiveSmallRageBtn(true);
        else
            rageBtn.gameObject.SetActive(false);
        TxtUpdate();
        //StartCoroutine(EndCorou());
    }

    public void SetActiveSmallRageBtn(bool value)
    {
        rageBtn.gameObject.SetActive(value);
        if (value)
            StartCoroutine("EndCorou");
        else
            StopCoroutine("EndCorou");
    }

    IEnumerator EndCorou()
    {
        float t = 2.5f;
        do
        {
            yield return null;
            t -= Time.deltaTime;
        } while (t > 0);

        t = 2.5f;
        float tt = t;
        Color color = rageBtn.targetGraphic.color;
        var main0 = fireParticles[0].main;
        var main1 = fireParticles[1].main;
        Color fireColor = main0.startColor.color;
        float fireAlpha = 0.3921569f;

        fireColor.a = fireAlpha;

        color.a = 1;
        rageBtn.targetGraphic.color = color;
        main0.startColor = fireColor;
        main1.startColor = fireColor;

        do
        {
            yield return null;
            t -= Time.deltaTime;
            float amount = Time.deltaTime / tt;
            color.a -= amount;
            rageBtn.targetGraphic.color = color;
            fireColor.a -= amount * fireAlpha;
            main0.startColor = fireColor;
            main1.startColor = fireColor;
        } while (t > 0);
        rageBtn.gameObject.SetActive(false);
        rageBtn.GetComponent<OnMouseButton>().onUp.Invoke();
    }

    public void CheckRage()
    {
        rageLevel = PlayerPrefs.GetInt("rageLevel");
        ResetRage();
        Rage(rageLevel);
        if (rageLevel > 0)
            SetActiveSmallRageBtn(true);
        TxtUpdate();
    }

    public void ResetRage()
    {
        enemiesDmg = 1;
        enemiesHp = 1;
        soul = 1;
        contentTxt.text = string.Empty;
    }

    public void TxtUpdate()
    {
        contentTxt.text = string.Empty;
        if(enemiesDmg != 0)
        {
            string str = string.Empty;
            switch (GameManager.language)
            {
                case Language.English:
                    str = string.Format("Enemies' Dmg {0}% Up \n", (enemiesDmg-1) * 100);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 공격력 {0}% 증가\n", (enemiesDmg-1) * 100);
                    break;
            }
            contentTxt.text += str;
        }
        if(enemiesHp != 0)
        {
            string str = string.Empty;
            switch (GameManager.language)
            {
                case Language.English:
                    str = string.Format("Enemies' Hp {0}% Up\n", (enemiesHp - 1) * 100);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 체력 {0}% 증가\n", (enemiesHp - 1) * 100);
                    break;
            }
            contentTxt.text += str;
        }

        if(soul != 0)
        {
            string str = string.Empty;
            switch (GameManager.language)
            {
                case Language.English:
                    str = string.Format("Soul Get {0}% Up\n", (soul-1)*100);
                    break;
                case Language.Korean:
                    str = string.Format("얻는 영혼 {0}% 증가\n", (soul-1)*100);
                    break;
            }
            contentTxt.text += str;
        }
    }

    public void Rage(int level)
    {
        switch(level)
        {
            case 0:
                fireParticles[0].gameObject.SetActive(true);
                fireParticles[1].gameObject.SetActive(true);
                return;
            //case 1:
            //    DmgUp(5f, level);
            //    HpUp(5f, level);
            //    SoulUp(0.5f, level);
            //    break;
            //case 2:
            //    DmgUp(2f, level);
            //    HpUp(2f, level);
            //    break;
            default:
                DmgUp(5f, level);
                HpUp(5f, level);
                SoulUp(0.5f, level);
                break;

        }
        Rage(level - 1);
        return;
    }

    void DmgUp(float amount, int level)
    {
        Debug.Log("DmgUp" + amount);
        enemiesDmg += amount;
    }

    void HpUp(float amount, int level)
    {
        Debug.Log("HpUp" + amount);
        enemiesHp += amount;
    }

    void SoulUp(float amount, int level)
    {
        Debug.Log("SoulUp" + amount);
        soul += amount;
    }
}
