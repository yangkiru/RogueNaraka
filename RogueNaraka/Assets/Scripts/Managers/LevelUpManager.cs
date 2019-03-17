using UnityEngine;
using System.Collections;
using RogueNaraka.UnitScripts;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager instance = null;

    public Fade fade;

    float time;
    bool isLevelUp;

    public RollManager rollManager;
    public StatManager statManager;

    IEnumerator endStageCorou;

    public Unit player { get { return BoardManager.instance.player; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        isLevelUp = PlayerPrefs.GetInt("isLevelUp") == 1;
    }

    public void RequestEndStageCorou()
    {
        if (endStageCorou != null)
            return;

        Debug.Log("StartEndStageCoroutine!!!");
        endStageCorou = EndStageCorou();
        StartCoroutine(EndStageCorou());
    }

    IEnumerator EndStageCorou()
    {
        do
        {
            yield return null;
            if (player.deathable.isDeath)
            {
                endStageCorou = null;
                yield break;
            }
        } while (BoardManager.instance.enemies.Count != 0 || SoulParticle.soulCount != 0);
        Debug.Log("EndStage!!");
        OnEndStage();
        float t = 2;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);
        //AdMobManager.instance.RequestBanner();
        endStageCorou = null;
    }

    public void OnEndStage()
    {
        player.autoMoveable.enabled = false;
        player.moveable.agent.Stop();

        fade.FadeOut();
        isLevelUp = true;
        BoardManager.instance.ClearStage();
    }

    public void LevelUp()
    {
        Debug.Log("LevelUp");
        GameManager.instance.SetPause(true);
        PlayerPrefs.SetInt("isLevelUp", 1);
        statManager.SyncStatUpgradeTxt();
        //SkillManager.instance.SetIsDragable(false);
        if (statManager.isLeftStatChanged)
        {
            rollManager.SetRollPnl(true, true, false);
            statManager.SetStatPnl(true);
            //fade.FadeIn();
        }
        else
        {
            rollManager.SetRollPnl(true);
        }
        GameManager.instance.Save();
        time = 0;
        player.moveable.agent.Stop();
        player.autoMoveable.enabled = true;
    }
}
