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
        float amount = need / Player.instance.GetStat(STAT.MP);
        if (amount > 1) amount = 1;
        needMana.fillAmount = amount;
        needMana.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        needMana.gameObject.SetActive(false);
    }

    public IEnumerator NoMana()
    {
        noMana.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        noMana.gameObject.SetActive(false);
    }
}
