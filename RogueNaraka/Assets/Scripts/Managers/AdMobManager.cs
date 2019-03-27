using UnityEngine;
using System;
using GoogleMobileAds.Api;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AdMobManager : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;
    private float deltaTime = 0.0f;
    private static string outputMessage = string.Empty;

    public GameObject adPnl;
    public TextMeshProUGUI adNameTxt;
    public TextMeshProUGUI adDescTxt;

    AdEvent onReward;
    AdEvent onStart;

    public Button removeAdsBtn;
    public Image testerPnl;
    public TMPro.TextMeshProUGUI testerTxt;

    bool isInit;

    public static AdMobManager instance;

    public static string OutputMessage
    {
        set { outputMessage = value; }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;
#if UNITY_ANDROID
        string appId = "ca-app-pub-8089259583608162~1486526628";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-3940256099942544~1458002511";
#else
        string appId = "unexpected_platform";
#endif
        if (isInit)
        {
            RequestRewardBasedVideo();
            return;
        }
        MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        //광고 요청이 성공적으로 로드되면 호출
        this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
        //광고 요청을 로드하지 못했을 때 호출
        this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
        //광고가 표시될 때 호출
        this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
        //광고가 재생되기 시작하면 호출
        this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
        //사용자가 비디오 시청을 통해 보상을 받을 때 호출
        this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
        //광고가 닫힐 때 호출
        this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
        //광고 클릭으로 인해 사용자가 애플리케이션을 종료한 경우 호출
        this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

        RequestRewardBasedVideo();

        isInit = true;
    }

    public void SetIsTester(bool value)
    {
        PlayerPrefs.SetInt("isTester", value ? 1 : 0);
        if (value)
            testerTxt.text = "Now you are Ads Tester";
        else
            testerTxt.text = "Now you are not Ads Tester";
        testerPnl.gameObject.SetActive(true);
        StartCoroutine(CloseTesterPnl());
    }

    public void ToggleIsTester()
    {
        bool value = PlayerPrefs.GetInt("isTester") == 0;
        SetIsTester(value);
    }

    int requestAmount;
    public void RequestToggleIsTester()
    {
        if(++requestAmount >= 10)
        {
            ToggleIsTester();
            requestAmount = 0;
        }
    }

    IEnumerator CloseTesterPnl()
    {
        float t = 2;
        do
        {
            yield return null;
            t -= Time.unscaledDeltaTime;
        } while (t > 0);

        testerPnl.gameObject.SetActive(false);
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("isRemoveAds", 1);
        if(this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
        removeAdsBtn.interactable = false;
    }

    // Returns an ad request with custom ad targeting.
    private AdRequest CreateAdRequest()
    {
        if (PlayerPrefs.GetInt("isTester") == 1)
        {
            return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .Build();
        }
        else
        {
            return new AdRequest.Builder()
            .Build();
        }
            //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            //.AddKeyword("game")
            //.SetGender(Gender.Male)
            //.SetBirthday(new DateTime(1985, 1, 1))
            //.TagForChildDirectedTreatment(false)
            //.AddExtra("color_bg", "9B30FF")
            //.Build();
    }

    public void RequestBanner()
    {
        if (PlayerPrefs.GetInt("isRemoveAds") == 1)
        {
            removeAdsBtn.interactable = false;
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(RequestBannerCorou());
            return;
        }

            // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
            string adUnitId = "ca-app-pub-8089259583608162/9668268975";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-8089259583608162/9668268975";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Register for ad events.
        this.bannerView.OnAdLoaded += this.HandleAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        this.bannerView.OnAdOpening += this.HandleAdOpened;
        this.bannerView.OnAdClosed += this.HandleAdClosed;
        this.bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        // Load a banner ad.
        this.bannerView.LoadAd(this.CreateAdRequest());
    }

    private void RequestInterstitial()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unexpected_platform";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        // Load an interstitial ad.
        this.interstitial.LoadAd(this.CreateAdRequest());
    }

    public void RequestRewardBasedVideo()
    {
#if UNITY_EDITOR
        string adUnitId = "ca-app-pub-8089259583608162/3877063853";
#elif UNITY_ANDROID
        string adUnitId = "ca-app-pub-8089259583608162/3877063853";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        string adUnitId = "unexpected_platform";
#endif

        this.rewardBasedVideo.LoadAd(this.CreateAdRequest(), adUnitId);
    }

    private void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            MonoBehaviour.print("Interstitial is not ready yet");
        }
    }

    public void ShowRewardBasedVideo()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            switch (GameManager.language)
            {
                case Language.English:
                    adNameTxt.text = "Network Connection Error";
                    adDescTxt.text = "Please try again later";
                    break;
                case Language.Korean:
                    adNameTxt.text = "네트워크 연결 에러";
                    adDescTxt.text = "나중에 다시 시도해주세요";
                    break;
            }
            adPnl.SetActive(true);
            return;
        }
        if (this.rewardBasedVideo.IsLoaded())
        {
            this.rewardBasedVideo.Show();
        }
        else
        {
            MonoBehaviour.print("Reward based video ad is not ready yet");
            //adPnl.SetActive(true);
            //switch (GameManager.language)
            //{
            //    case Language.English:
            //        adNameTxt.text = "Ad is not ready yet";
            //        adDescTxt.text = "Please try again later";
            //        break;
            //    case Language.Korean:
            //        adNameTxt.text = "광고가 준비되지 않았습니다";
            //        adDescTxt.text = "나중에 다시 시도해주세요";
            //        break;
            //}
            RequestRewardBasedVideo();
        }
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    }

    public void HandleAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeftApplication event received");
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLoaded event received");
    }

    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleInterstitialFailedToLoad event received with message: " + args.Message);
    }

    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialOpened event received");
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialClosed event received");
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleInterstitialLeftApplication event received");
    }

    #endregion

    #region RewardBasedVideo callback handlers

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
        //adPnl.SetActive(true);
        //switch (GameManager.language)
        //{
        //    case Language.English:
        //        adNameTxt.text = "Ad load failed";
        //        adDescTxt.text = "Please try again later";
        //        break;
        //    case Language.Korean:
        //        adNameTxt.text = "광고를 불러오지 못했습니다";
        //        adDescTxt.text = "나중에 다시 시도해주세요";
        //        break;
        //}
        RequestRewardBasedVideo();
        //StartCoroutine(RequestRewardVideoCorou());
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        if (onStart != null)
            onStart.Invoke();
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        //adPnl.SetActive(true);
        //switch (GameManager.language)
        //{
        //    case Language.English:
        //        adNameTxt.text = "Ad closed";
        //        adDescTxt.text = "You can't be rewarded";
        //        break;
        //    case Language.Korean:
        //        adNameTxt.text = "광고가 취소되었습니다";
        //        adDescTxt.text = "광고를 끝까지 보지 못하여 보상을 받을 수 없습니다";
        //        break;
        //}
        RequestRewardBasedVideo();
        //StartCoroutine(RequestRewardVideoCorou());
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        if (onReward != null)
            onReward.Invoke();
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
        RequestRewardBasedVideo();
        //StartCoroutine(RequestRewardVideoCorou());
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }

    #endregion

    public void SetOnReward(AdEventScript adEventScript)
    {
        //AdEventScript adEventScript = GetComponent<AdEventScript>();
        onReward = adEventScript.onReward;
    }
    public void SetOnStart(AdEventScript adEventScript)
    {
        onStart = adEventScript.onStart;
    }

    IEnumerator RequestBannerCorou()
    {
        while(true)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Start();
                RequestBanner();
                yield break;
            }
            float t = 10;
            do
            {
                yield return null;
                t -= Time.unscaledDeltaTime;
            } while (t > 0);
        }
    }


}