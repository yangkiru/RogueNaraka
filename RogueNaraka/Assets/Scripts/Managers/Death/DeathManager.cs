using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RogueNaraka.TierScripts;
using RogueNaraka.TimeScripts;
//using RogueNaraka.TheBackendScripts;
using RogueNaraka.UnitScripts;

public partial class DeathManager : MonoBehaviour
{
    const float UP_PER_EXP_SPEED = 0.5f;
    const float CHANGE_SCALE_OF_TIER_EMBLEM_SPEED = 2.5f;
    const float MAX_SCALE_OF_TIER = 1.5f;

    public static DeathManager instance;

    public GameObject[] btnLayout;
    public GameObject deathPnl;
    public Image soulPnl;

    public Image youDied;

    public TextMeshProUGUI soulRefiningRateTxt;
    public TextMeshProUGUI unSoulTxt;
    public TextMeshProUGUI soulTxt;

    [Header("Tier Object")]
    public Image TierEmblem;
    public Image TierEffect_L;
    public Image TierEffect_R;
    public Sprite[] TierEffect;
    public TextMeshProUGUI TierName;
    public TextMeshProUGUI NextTierRequirement;
    public GameObject AdvanceBanner;
    public TextMeshProUGUI BannerText;

    [Header("Level And Exp Object")]
    public TextMeshProUGUI curLvTxt;
    public TextMeshProUGUI nextLvTxt;
    public TextMeshProUGUI expNumTxt;
    public Image ExpGauge;
    public Image LvUpEffect_L;
    public Image LvUpEffect_R;

    private List<int> huntedUnitNumList = new List<int>();
    public Canvas stageCanvas;

    private bool isClickableCloseBtn;

    public float tt { get; set; }

    void Awake()
    {
        instance = this;

        for(int i = 0; i < GameDatabase.instance.enemies.Count(); ++i) {
            this.huntedUnitNumList.Add(0);
        }
    }

    public void SetDeathPnl(bool value)
    {
        deathPnl.SetActive(value);
        //CameraShake.instance.Shake(0.2f, 0.2f, 0.01f);
        if (value)
        {
            StartCoroutine(PumpCorou(youDied.rectTransform, 3, 0.5f));
        }
    }

    public void OnDeath()
    {
        Debug.Log("OnDeath");
        if(BoardManager.instance.player)
            GameManager.instance.Save();

        PlayerPrefs.SetInt("isRun", 0);
        GameManager.instance.movePointer.SetActive(false);
        SetDeathPnl(true);
        BoardManager.instance.ClearFriendly();

        StartCoroutine(OpenBtnPnlCorou());

        GameManager.instance.SetSettingBtn(true);

        AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomDeathMusic());

        stageCanvas.sortingLayerName = "UI";
    }

    IEnumerator SoulPnlCorou(float t)
    {
        this.isClickableCloseBtn = false;

        tt = 1.5f;
        do
        {
            yield return null;
            tt -= TimeManager.Instance.UnscaledDeltaTime;
        } while (tt > 0);

        if (PlayerPrefs.GetFloat("lastRefiningRate") != -1)
        {
            SetSoulPnl(true);
        }

        this.isClickableCloseBtn = true;
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
            //AdMobManager.instance.RequestRewardBasedVideo();
        }
        else
        {
            soulPnl.gameObject.SetActive(false);
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
        if(!this.isClickableCloseBtn) {
            return;
        }
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
        EndGame();
        PlayerPrefs.SetInt("isRun", 0);
        GameManager.instance.Load();
    }

    public void OpenLobby(){
        EndGame();
        LobbyManager.Instance.Content.SetActive(true);
        LobbyManager.Instance.OpenDungeonPnl();
    }

    public void EndGame()
    {
        SetDeathPnl(false);
        RollManager.instance.IsFirstRoll = true;

        RageManager.instance.SetActiveSmallRageBtn(false);
        //AudioManager.instance.PlayMusic(AudioManager.instance.GetRandomMainMusic());

        SkillManager.instance.ResetSave();
        //Item.instance.ResetSave();
        //Item.instance.InitItem();

        BoardManager.instance.SetStage(0);
        BoardManager.instance.ClearStage();

        PlayerPrefs.SetFloat("lastRefiningRate", 0);

        Fillable.playerHp.img.fillAmount = 1;
        Fillable.playerMp.img.fillAmount = 1;

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
        double upExpPerSecond = (TierManager.Instance.TotalGainExpInGame) * UP_PER_EXP_SPEED;
        double min_upExpPerSecond = upExpPerSecond * 0.08d;
        double maxExp = GameDatabase.instance.requiredExpTable[_originLv - 1];
        double remainUpExp = TierManager.Instance.TotalGainExpInGame;

        WaitForFixedUpdate waitFixedFrame = new WaitForFixedUpdate();

        while(_originLv < TierManager.Instance.PlayerLevel
            || _originExp < TierManager.Instance.CurrentExp) {
            yield return waitFixedFrame;
            if(remainUpExp <= TierManager.Instance.TotalGainExpInGame * 0.2d) {
                if(upExpPerSecond <= min_upExpPerSecond) {
                    upExpPerSecond = min_upExpPerSecond;
                } else {
                    upExpPerSecond *= 0.95d;
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
                this.LvUpEffect_L.gameObject.SetActive(true);
                this.LvUpEffect_R.gameObject.SetActive(true);
            }
            this.expNumTxt.text = string.Format("{0}  /  {1}", (int)_originExp, (int)maxExp);
            this.ExpGauge.fillAmount = (float)(_originExp / maxExp);
        }
        this.expNumTxt.text = string.Format("{0}  /  {1}", (int)TierManager.Instance.CurrentExp
            , (int)GameDatabase.instance.requiredExpTable[TierManager.Instance.PlayerLevel - 1]);
        this.ExpGauge.fillAmount = (float)(TierManager.Instance.CurrentExp / GameDatabase.instance.requiredExpTable[TierManager.Instance.PlayerLevel - 1]);
    }

    private IEnumerator StartChangeTierAnimation() {
        string bannerTextFormat = "";
        this.TierEffect_L.gameObject.SetActive(true);
        this.TierEffect_R.gameObject.SetActive(true);
        if(TierManager.Instance.IsAdvanced) {
            switch(GameManager.language) {
                case Language.English:
                    bannerTextFormat = "Promoted to {0} {1}!";
                break;
                case Language.Korean:
                    bannerTextFormat = "{0} {1}로 승급했습니다!";
                break;
            }
            this.TierEffect_L.sprite = this.TierEffect[0];
            this.TierEffect_R.sprite = this.TierEffect[1];
        } else {
            switch(GameManager.language) {
                case Language.English:
                    bannerTextFormat = "Downgraded to {0} {1}.";
                break;
                case Language.Korean:
                    bannerTextFormat = "{0} {1}로 강등했습니다.";
                break;
            }
            this.TierEffect_L.sprite = this.TierEffect[2];
            this.TierEffect_R.sprite = this.TierEffect[3];
        }
        this.BannerText.text = string.Format(bannerTextFormat
            , TierManager.Instance.CurrentTier.type
            , TierManager.Instance.CurrentTier.tier_num != 0 ? TierManager.Instance.CurrentTier.tier_num.ToString() : "");

        WaitForFixedUpdate waitFixedFrame = new WaitForFixedUpdate();
        //Pause
        yield return new WaitForSecondsRealtime(0.3f);
        //Banner
        this.AdvanceBanner.SetActive(true);
        //Change
            this.TierEmblem.transform.localScale = new Vector2(MAX_SCALE_OF_TIER, MAX_SCALE_OF_TIER);
            this.TierEmblem.sprite = TierManager.Instance.CurrentTier.emblem;
            this.TierName.text = string.Format("- {0} {1} Tier -"
            , TierManager.Instance.CurrentTier.type
            , TierManager.Instance.CurrentTier.tier_num != 0 ? TierManager.Instance.CurrentTier.tier_num.ToString() : "");
            string textFormat = "";
            switch(GameManager.language) {
                case Language.English:
                    textFormat = "Current Top {0}%\nNext Tier : Top {1}%";
                break;
                case Language.Korean:
                    textFormat = "현재 상위 {0}%\n다음 티어 : 상위 {1}%";
                break;
            }
            /*this.NextTierRequirement.text = string.Format(textFormat
                , Mathf.Floor(TheBackendManager.Instance.TopPercentToClearStageForRank * 100) * 0.01f
                , TierManager.Instance.NextTier.requiredRankingPercent);*/

        //Smaller
        while(this.TierEmblem.transform.localScale.x >= 1.0f) {
            this.TierEmblem.transform.localScale -= new Vector3(
                CHANGE_SCALE_OF_TIER_EMBLEM_SPEED
                , CHANGE_SCALE_OF_TIER_EMBLEM_SPEED
                , 0.0f) * TimeManager.Instance.FixedDeltaTime;
            yield return waitFixedFrame;
        }
        this.TierEmblem.transform.localScale = new Vector2(1.0f, 1.0f);
    }
}
