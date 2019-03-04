using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RageManager : MonoBehaviour
{
    public static RageManager instance;

    public int rageLevel;
    public float enemiesDmg = 1;
    public float enemiesHp = 1;

    public GameObject ragePnl;
    public TextMeshProUGUI contentTxt;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void Rage()
    {
        rageLevel = PlayerPrefs.GetInt("rageLevel") + 1;
        PlayerPrefs.SetInt("rageLevel", rageLevel);
        ragePnl.SetActive(true);

        PlayerPrefs.SetInt("isRun", 0);

        ResetRage();
        Rage(rageLevel);
        StartCoroutine(EndCorou());
    }

    IEnumerator EndCorou()
    {
        float t = 3;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        ragePnl.SetActive(false);
        BoardManager.instance.player.Kill();
    }

    public void CheckRage()
    {
        rageLevel = PlayerPrefs.GetInt("rageLevel");
        Rage(rageLevel);
    }

    public void ResetRage()
    {
        enemiesDmg = 1;
        enemiesHp = 1;
    }

    public void Rage(int level)
    {
        switch(level)
        {
            case 0:
                return;
            case 1:
                DmgUp(0.5f, level);
                break;
            case 2:
                HpUp(0.5f, level);
                break;
        }
        Rage(level - 1);
        return;
    }

    void DmgUp(float amount, int level)
    {
        enemiesDmg += amount;
        if (level == rageLevel)
        {
            string str = string.Empty;
            switch (GameManager.language)
            {
                case Language.English:
                    str = string.Format("Enemies' Dmg {0}% Up", amount * 100);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 공격력 {0}% 증가", amount * 100);
                    break;
            }
            contentTxt.text = str;
        }
    }

    void HpUp(float amount, int level)
    {
        enemiesDmg += amount;
        if (level == rageLevel)
        {
            string str = string.Empty;
            switch (GameManager.language)
            {
                case Language.English:
                    str = string.Format("Enemies' Hp {0}% Up", amount * 100);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 체력 {0}% 증가", amount * 100);
                    break;
            }
            contentTxt.text = str;
        }
    }
}
