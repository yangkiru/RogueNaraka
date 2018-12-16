using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaScript : MonoBehaviour {

    public Image needMana;
    public Image noMana;

    public static ManaScript instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public IEnumerator NeedMana(float need)
    {
        float amount = need / BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);
        if (amount > 1) amount = 1;
        needMana.fillAmount = amount;
        needMana.gameObject.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        needMana.gameObject.SetActive(false);
    }

    public IEnumerator NoMana()
    {
        noMana.gameObject.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        noMana.gameObject.SetActive(false);
    }
}
