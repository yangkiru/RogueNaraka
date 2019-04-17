using RogueNaraka.UnitScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.TierScripts;
using RogueNaraka.TimeScripts;

public class DeathManager : MonoBehaviour
{
    //초당 ?�르?? 게임?? ?��? 경험치퍼?�트?�니??.
    const float UP_PER_EXP_SPEED = 0.5f;

    public static DeathManager instance;

    public GameObject[] btnLayout;
    public Image deathPnl;
    public Image soulPnl;

    public Image youDied;

    public TextMeshProUGUI soulRefiningRateTxt;
    public TextMeshProUGUI unSoulTxt;
    public TextMeshProUGUI soulTxt;

    public TextMeshProUGUI curLvTxt;
    public TextMeshProUGUI nextLvTxt;
    public TextMeshProUGUI expNumTxt;
    public Image ExpGauge;

    private List<int> huntedUnitNumList = new List<int>();
    public Canvas stageCanvas;

    void Awake()
    {
        instance = this;
        
        for(int i = 0; i < GameDatabase.instance.enemies.Count(); ++i) {
            this.huntedUnitNumList.Add(0);
        }
    }

    public void SetDeathPnl(bool value)
    {
        deathPnl.gameObject.SetActive(value);
        //CameraShake.instance.Shake(0.2f, 0.2f, 0.01f);
        if (value)
        {
            btnLayout[0].SetActive(false);
            btnLayout[1].SetActive(false);
            StartCoroutine(PumpCorou(youDied.rectTransform, 3, 0.5f));
        }
    }

    public void OnDeath()
    {
        //RankManager.instance.SendPlayerRank();
        if(BoardManager.instance.player)
            GameManager.instance.Save();

        RankManager.instance.SendPlayerRank();

        SetDeathPnl(true);

        StartCoroutine(SoulPnlCorou(1));

        GameManager.instance.SetSettingBtn(true);

        AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomDeathMusic());

        stageCanvas.sortingLayerName = "UI";
    }

    IEnumerator SoulPnlCorou(float t)
    {
        //lv, 경험�? ?�팅
        int playerOriginLv = TierManager.Instance.PlayerLevel;
        double curExp = TierManager.Instance.CurrentExp;
        double maxExp = GameDatabase.instance.requiredExpTable[playerOriginLv - 1];
        this.curLvTxt.text = playerOriginLv.ToString();
        this.nextLvTxt.text = (playerOriginLv + 1).ToString();
        this.expNumTxt.text = string.Format("{0}  /  {1}", curExp, maxExp);
        this.ExpGauge.fillAmount = (float)(curExp / maxExp);
        //

        TierManager.Instance.SaveExp();
        yield return new WaitForSecondsRealtime(1.5f);

        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        if (PlayerPrefs.GetFloat("lastRefiningRate") != -1)
        {
            SetSoulPnl(true);
        }
        else
        {
            btnLayout[0].SetActive(true);
            btnLayout[1].SetActive(true);
        }

        StartCoroutine(StartGainExpAnimation(playerOriginLv, curExp));
    }

    public void SetSoulPnl(bool value)
    {
        if(value)
        {
            float lastRefiningRate = PlayerPrefs.GetFloat("lastRefiningRate");
            float refiningRate = lastRefiningRate == 0 ? MoneyManager.instance.GetRandomRefiningRate() : lastRefiningRate;
            soulTxt.text = "0";
            unSoulTxt.text = "0";
            PlayerPrefs.SetFloat("lastRefiningRate", refiningRate);
            soulPnl.gameObject.SetActive(true);
            rate = refiningRate;
            StartCoroutine(SoulRefiningRateTxtCorou(refiningRate));
            StartCoroutine(PumpCorou(soulPnl.rectTransform, 0f, 0.25f));
            soulCorou = SoulCorou();
            StartCoroutine(soulCorou);
            AdMobManager.instance.RequestRewardBasedVideo();
        }
        else
        {
            soulPnl.gameObject.SetActive(false);
            btnLayout[0].SetActive(true);
            btnLayout[1].SetActive(true);
            PlayerPrefs.SetFloat("lastRefiningRate", -1);
        }
    }

    IEnumerator SoulRefiningRateTxtCorou(float rate)
    {
        int intRate = (int)(rate * 100);
        float delay = 1f / intRate;
        for (int i = 0; i <= intRate; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);

            if (Input.anyKey)
                i = intRate;

            soulRefiningRateTxt.text = string.Format("{0}%", i);
        }

        int unSoul = MoneyManager.instance.unrefinedSoul;
        delay = 0.5f / intRate;
        for (int i = 0; i <= unSoul; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);

            if (Input.anyKey)
                i = unSoul;

            unSoulTxt.text = string.Format("{0}<size=12><sprite=0></size>", i);
        }

        int soul = (int)(unSoul * rate);
        delay = 0.5f / intRate;
        for (int i = 0; i <= soul; i++)
        {
            float t = delay;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);
            if (Input.anyKey)
                i = soul;

            soulTxt.text = string.Format("{0}<size=12><sprite=0></size>", i);
        }
    }

    bool isADReward;
    //-    public bool isClose { get; set; }
    //-
    //-    IEnumerator SoulAutoCloseCorou(float rate)
    //-    {
    //-        float t = 6;
    //-        do
    //-        {
    //-            yield return null;
    //-            t -= Time.unscaledDeltaTime* (isADActive? 0 : 1);
    //-            if(isADReward)
    //-            {
    //-                MoneyManager.instance.RefineSoul(rate* 2);
    //-                SetSoulPnl(false);
    //-                yield break;
    //-            }
    //-            if(isClose)
    //-            {
    //-                MoneyManager.instance.RefineSoul(rate);
    //-                SetSoulPnl(false);
    //-                isClose = false;
    //-                yield break;
    //-            }
    //-        } while (t > 0);
    //-
    //-        if (soulPnl.gameObject.activeSelf)
    //-        {
    //-            MoneyManager.instance.RefineSoul(rate);
    //-            SetSoulPnl(false);
    //-        }
    //-    }

    IEnumerator soulCorou;
    IEnumerator SoulCorou()
    {
        while(true)
        {
            yield return null;
            if(isADReward)
            {
                yield return null;
                isADReward = false;
                MoneyManager.instance.RefineSoul(rate * 2);
                SetSoulPnl(false);
                break;
            }
        }
        soulCorou = null;
    }

    float rate;

    public void OnSoulRefiningRatePnlClose()
    {
        StopCoroutine(soulCorou);
        MoneyManager.instance.RefineSoul(rate);
        SetSoulPnl(false);
    }

    public void OnADReward()
    {
        isADReward = true;
    }

    public void ReGame()
    {
        SetDeathPnl(false);

        RageManager.instance.SetActiveSmallRageBtn(false);
        //AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomMainMusic());
        PlayerPrefs.SetInt("isRun", 0);
        SkillManager.instance.ResetSave();
        Item.instance.ResetSave();
        Item.instance.InitItem();

        BoardManager.instance.ClearStage();
        GameManager.instance.Load();
        PlayerPrefs.SetFloat("lastRefiningRate", 0);

        BoardManager.instance.player.hpable.SetFullHp();
        BoardManager.instance.player.mpable.SetFullMp();

        GameManager.instance.SetSettingBtn(true);

        stageCanvas.sortingLayerName = "Wall";
    }

    public void OpenSoulShop()
    {
        SoulShopManager.instance.SetSoulShop(true);
    }

    IEnumerator PumpCorou(RectTransform rect, float size, float t)
    {
        RectTransform imgRect = rect;
        Vector3 origin = imgRect.localScale;
        imgRect.localScale = new Vector3(size, size, 0);
        float tt = 0;
        while (tt < t)
        {
            yield return null;
            tt += Time.unscaledDeltaTime;
            imgRect.localScale = Vector3.Lerp(imgRect.localScale, origin, tt / t);
        }
        imgRect.localScale = origin;
    }

    public List<int> GetHuntedUnitNumList() {
        return this.huntedUnitNumList.ToList();
    }

    public int GetHuntedUnitNum(int _unitId) {
        if(_unitId < 0 || _unitId >= this.huntedUnitNumList.Count) {
            throw new System.ArgumentException(string.Format("Unit Id is Incorrect! Id : {0}", _unitId));
        }
        return this.huntedUnitNumList[_unitId];
    }

    public void ReceiveHuntedUnit(int _unitId) {
        this.huntedUnitNumList[_unitId] += 1;
    }

    public void ClearHuntedUnitList() {
        for(int i = 0; i < this.huntedUnitNumList.Count; ++i) {
            this.huntedUnitNumList[i] = 0;
        }
    }

    private IEnumerator StartGainExpAnimation(int _originLv, double _originExp) {
        double upExpPerSecond = TierManager.Instance.TotalGainExpInGame * UP_PER_EXP_SPEED;
        double min_upExpPerSecond = upExpPerSecond * 0.08d;
        double maxExp = GameDatabase.instance.requiredExpTable[_originLv - 1];
        double remainUpExp = TierManager.Instance.TotalGainExpInGame;
        while(_originLv < TierManager.Instance.PlayerLevel 
            || _originExp < TierManager.Instance.CurrentExp) {
            yield return new WaitForFixedUpdate();
            if(remainUpExp <= TierManager.Instance.TotalGainExpInGame * 0.2d) {
                upExpPerSecond *= 0.95d;
                if(upExpPerSecond < min_upExpPerSecond) {
                    upExpPerSecond = min_upExpPerSecond;
                }
            }
            double upExp = upExpPerSecond * TimeManager.Instance.FixedDeltaTime;
            _originExp += upExp;
            remainUpExp -= upExp;
            if(_originExp >= maxExp) {
                _originLv++;
                _originExp = 0.0d;
                maxExp = GameDatabase.instance.requiredExpTable[_originLv - 1];
                this.curLvTxt.text = _originLv.ToString();
                this.nextLvTxt.text = (_originLv + 1).ToString();
            }
            this.expNumTxt.text = string.Format("{0}  /  {1}", (int)_originExp, (int)maxExp);
            this.ExpGauge.fillAmount = (float)(_originExp / maxExp);
        }
        this.expNumTxt.text = string.Format("{0}  /  {1}", (int)TierManager.Instance.CurrentExp
            , (int)GameDatabase.instance.requiredExpTable[TierManager.Instance.PlayerLevel - 1]);
        this.ExpGauge.fillAmount = (float)(TierManager.Instance.CurrentExp / GameDatabase.instance.requiredExpTable[TierManager.Instance.PlayerLevel - 1]);
    }
}