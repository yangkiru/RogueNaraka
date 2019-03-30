using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaScript : MonoBehaviour {

    public Image needMana;
    public Image noMana;

    public static ManaScript instance = null;
    int needCount;
    int noCount;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetNeedMana(bool value, float need = 0)
    {
        if(!value)
        {
            needMana.gameObject.SetActive(false);
            return;
        }

        float amount = need / BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);
        if (amount > 1) amount = 1;

        needMana.fillAmount = amount;
        needMana.gameObject.SetActive(true);
    }

    public IEnumerator NeedMana(float need)
    {
        float amount = need / BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);
        if (amount > 1) amount = 1;
        needMana.fillAmount = amount;
        needMana.gameObject.SetActive(true);
        needCount++;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if(--needCount <= 0)
            needMana.gameObject.SetActive(false);
    }

    public IEnumerator NoMana()
    {
        noMana.gameObject.SetActive(true);
        noCount++;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        if (--noCount <= 0)
            noMana.gameObject.SetActive(false);
    }
}
