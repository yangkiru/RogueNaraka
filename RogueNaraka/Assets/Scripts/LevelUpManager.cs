using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour {

    public Text[] upgradeTxt;

    public GameObject selectPnl;
    public GameObject statPnl;

    public Button cancelBtn;


    private Player player
    { get { return Player.instance; } }

    public static LevelUpManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    /// <summary>
    /// Player가 Enemy를 모두 처치하고 화면 상단에 도착하면 호출
    /// </summary>
    /// 
    public void LevelUp()
    {
        GameManager.instance.SetPause(true);
        SyncStatUpgradeTxt();
        SetSelectPnl(true);
        GameManager.instance.Save();
    }

    public void SetSelectPnl(bool value)
    {
        selectPnl.SetActive(value);
    }

    public void StatUp(int type)
    {

        if (player.AddStat((STAT)type, 1))
        {
            Debug.Log("Stat Upgraded");
            cancelBtn.interactable = false;
            statPnl.SetActive(false);
            StartCoroutine(WaitForStatUpClose());
            GameManager.instance.StatTextUpdate();
        }
        else
        {
            Debug.Log("Stat Maxed");
        }
    }

    private IEnumerator WaitForStatUpClose()
    {
        yield return new WaitForSecondsRealtime(1);
        cancelBtn.interactable = true;
        SetSelectPnl(false);
        BoardManager.instance.StageUp();
        GameManager.instance.Save();
    }

    public void SyncStatUpgradeTxt()
    {
        upgradeTxt[0].text = string.Format("{0}/{1}", player.stat.dmg.ToString(), player.maxStat.dmg);
        upgradeTxt[1].text = string.Format("{0}/{1}", player.stat.spd.ToString(), player.maxStat.spd);
        upgradeTxt[2].text = string.Format("{0}/{1}", player.stat.tec.ToString(), player.maxStat.tec);
        upgradeTxt[3].text = string.Format("{0}/{1}", player.stat.hp.ToString(), player.maxStat.hp);
        upgradeTxt[4].text = string.Format("{0}/{1}", player.stat.mp.ToString(), player.maxStat.mp);
        upgradeTxt[5].text = string.Format("{0}/{1}", player.stat.hpRegen.ToString(), player.maxStat.hpRegen);
        upgradeTxt[6].text = string.Format("{0}/{1}", player.stat.mpRegen.ToString(), player.maxStat.mpRegen);
    }
}
