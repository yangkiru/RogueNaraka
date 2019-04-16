using System.Collections;
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
    public GameObject rageSmallPnl;
    public Image rageBigPnl;
    public ParticleSystem[] fireParticles;

    public TextMeshProUGUI rageLevelTxt;
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
        isRage = true;

        if (rageLevel > 0)
            SetActiveSmallRageBtn(true);
        else
            rageBtn.gameObject.SetActive(false);
        TxtUpdate();
        //StartCoroutine(EndCorou());
    }

    public void SetActiveSmallRageBtn(bool value)
    {
        if (value)
        {
            StopCoroutine("SmallBtnCorou");
            rageBtn.interactable = true;
            rageBtn.gameObject.SetActive(true);
            rageBtn.targetGraphic.color = Color.white;
            fireParticles[0].gameObject.SetActive(true);
            fireParticles[1].gameObject.SetActive(true);
        }
        else
            StartCoroutine("SmallBtnCorou");

        if (rageLevel > 1)
        {
            rageLevelTxt.text = string.Format("x{0}", rageLevel);
            rageLevelTxt.gameObject.SetActive(true);
        }
        else
            rageLevelTxt.gameObject.SetActive(false);
    }

    public void BigPnlOpen()
    {
        StartCoroutine(BigPnlCorou());
    }

    IEnumerator BigPnlCorou()
    {
        rageBigPnl.gameObject.SetActive(true);
        rageBigPnl.color = Color.black;

        rageBigPnl.rectTransform.localScale = Vector3.zero;
        float value = 0;
        Vector3 vec = new Vector3(0, 0, 1);

        do
        {
            yield return null;
            value += Time.unscaledDeltaTime * 5;
            vec.x = value;
            vec.y = value;
            rageBigPnl.rectTransform.localScale = vec;
        } while (value < 1.2f);

        value = 1.2f;
        vec.x = value;
        vec.y = value;
        rageBigPnl.rectTransform.localScale = vec;

        do
        {
            yield return null;
            value -= Time.unscaledDeltaTime * 2;
            vec.x = value;
            vec.y = value;
            rageBigPnl.rectTransform.localScale = vec;
        } while (value > 1f);
        value = 1;
        vec.x = value;
        vec.y = value;
        rageBigPnl.rectTransform.localScale = vec;

        float t = 1.5f;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        t = 2;
        float tt = t;
        Color color = rageBigPnl.color;

        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
            float amount = Time.unscaledDeltaTime / tt;
            color.a -= amount;
            rageBigPnl.color = color;
        } while (t > 0);

        rageBigPnl.gameObject.SetActive(false);
        isRage = false;
    }

    IEnumerator SmallBtnCorou()
    {
        float t = 3f;
        float tt = t;
        Color color = Color.white;

        rageBtn.interactable = false;
        //var main0 = fireParticles[0].main;
        //var main1 = fireParticles[1].main;
        //Color fireColor = main0.startColor.color;
        //float fireAlpha = 0.3921569f;

        do
        {
            yield return null;
            t -= Time.deltaTime;
            float amount = Time.unscaledDeltaTime / tt;
            color.a -= amount;
            rageBtn.targetGraphic.color = color;
            //fireColor.a -= amount * fireAlpha;
            //main0.startColor = fireColor;
            //main1.startColor = fireColor;
        } while (t > 0);

        rageBtn.gameObject.SetActive(false);
        color.a = 1;
        rageBtn.targetGraphic.color = color;
        //rageBtn.gameObject.SetActive(false);
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
                    str = string.Format("Enemies' Dmg {0} times Up \n", enemiesDmg-1);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 공격력 {0}배 증가\n", enemiesDmg-1);
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
                    str = string.Format("Enemies' Hp {0} times Up\n", enemiesHp - 1);
                    break;
                case Language.Korean:
                    str = string.Format("적들의 체력 {0}배 증가\n", enemiesHp - 1);
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
                    str = string.Format("Soul Get {0} times Up\n", soul-1);
                    break;
                case Language.Korean:
                    str = string.Format("얻는 영혼 {0}배 증가\n", soul-1);
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
