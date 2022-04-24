using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaScript : MonoBehaviour {

    public Image needMana;
    public Image needManaMask;
    public Image noMana;

    public static ManaScript instance = null;
    int needCount;
    private float need;
    
    public Coroutine NoManaCoroutine;

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
        this.need = need;
        float currentMp = BoardManager.instance.player.data.stat.currentMp;
        float maxMp = BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);
        float amount = (need + maxMp - currentMp) / maxMp;
        if (amount > 1) amount = 1;

        needManaMask.fillAmount = currentMp / maxMp;
        needMana.fillAmount = amount;
        noMana.fillAmount = need / maxMp;
        noMana.gameObject.SetActive(true);
        needMana.gameObject.SetActive(true);
    }

    public IEnumerator NoMana()
    {
        noMana.gameObject.SetActive(true);
        needMana.gameObject.SetActive(true);

        float t = 0;
        for (int i = 0; i < 10; i++){
            if (i % 2 == 0) {
                float currentMp = BoardManager.instance.player.data.stat.currentMp;
                float maxMp = BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);
                needMana.gameObject.SetActive(!needMana.gameObject.activeSelf);
                needManaMask.fillAmount = currentMp / maxMp;
                float amount = (need + maxMp - currentMp) / maxMp;
                needMana.fillAmount = amount;
                noMana.fillAmount = need / maxMp;
            }
            t += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        needMana.gameObject.SetActive(false);
        noMana.gameObject.SetActive(false);
        NoManaCoroutine = null;
    }

    public IEnumerator NoMaxMana()
    {
        needMana.gameObject.SetActive(true);
        noMana.gameObject.SetActive(true);

        float t = 0;
        for (int i = 0; i < 10; i++){
            if (i % 2 == 0) {
                float currentMp = BoardManager.instance.player.data.stat.currentMp;
                float maxMp = BoardManager.instance.player.data.stat.GetCurrent(STAT.MP);

                needMana.gameObject.SetActive(!needMana.gameObject.activeSelf);
                noMana.gameObject.SetActive(!noMana.gameObject.activeSelf);
                needManaMask.fillAmount = currentMp / maxMp;
                float amount = (need + maxMp - currentMp) / maxMp;
                needMana.fillAmount = amount;
                noMana.fillAmount = need / maxMp;
            }
            t += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        needMana.gameObject.SetActive(false);
        noMana.gameObject.SetActive(false);
        NoManaCoroutine = null;
    }
}
