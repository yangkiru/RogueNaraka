using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using TMPro;

public partial class DeathManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {
    [Header("RewardAd")]
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "RewardCoin_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms
    [SerializeField] Transform _resultPnl;
    [SerializeField] Transform _rewardBackPnl;
    [SerializeField] Transform _rewardPnl;
    [SerializeField] TextMeshProUGUI _resultCoinTxt;
    [SerializeField] TextMeshProUGUI _stageTxt;
    [SerializeField] TextMeshProUGUI _rewardCoinTxt;
    [SerializeField] TextMeshProUGUI _rewardBtnTxt;


    private int _rewardCoin;

    private IEnumerator OpenResultPnlCorou(){
        // Close
        float closeTime = 0;
        LoadAd();
        _resultPnl.localScale = Vector3.zero;
        _resultPnl.gameObject.SetActive(true);

        if (GameManager.language == Language.Korean) {
            _stageTxt.text = "스테이지 " + BoardManager.instance.stage.ToString();
            _resultCoinTxt.text = "코인 : " + MoneyManager.instance.TempCoin.ToString();
            _rewardBtnTxt.text = "광고 시청 후 2배";
            
        } else {
            _stageTxt.text = "Stage" + BoardManager.instance.stage.ToString();
            _resultCoinTxt.text = "Coins : " + MoneyManager.instance.TempCoin.ToString();
            _rewardBtnTxt.text = "Ad for x2";
        }
        
        
        do {
            yield return null;
            closeTime += Time.unscaledDeltaTime * CLOSE_SPEED;
            _resultPnl.localScale = Vector3.Lerp(_resultPnl.localScale, Vector3.one, closeTime);
        } while(closeTime < 1);
    }

    private IEnumerator OpenRewardPnlCorou(){
        // Close
        float closeTime = 0;
        _rewardBackPnl.gameObject.SetActive(true);
        _rewardPnl.localScale = Vector3.zero;
        _rewardPnl.gameObject.SetActive(true);
        do {
            yield return null;
            closeTime += Time.unscaledDeltaTime * CLOSE_SPEED;
            _rewardPnl.localScale = Vector3.Lerp(_rewardPnl.localScale, Vector3.one, closeTime);
        } while(closeTime < 1);
    }

    public void CloseResultPnl(){
        if (MoneyManager.instance.TempCoin > 1) {
            MoneyManager.instance.AddSoul(MoneyManager.instance.TempCoin);
        }
        _rewardPnl.gameObject.SetActive(false);
        _rewardBackPnl.gameObject.SetActive(false);
        _resultPnl.gameObject.SetActive(false);
        OpenLobby();
    }

    private void RewardAdAwake(){
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
        //Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;
    }
// Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }
 
    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
 
        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }
 
    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }
 
    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            int resultCoin = MoneyManager.instance.ExchangeTempCoin(2);
            if (GameManager.language == Language.Korean) {
                _rewardCoinTxt.text = resultCoin.ToString() + " 코인을 획득했습니다";
            }
            else{
                _rewardCoinTxt.text = "You got " + resultCoin.ToString() + " coins";
            }
            
            StartCoroutine(OpenRewardPnlCorou());

            // Load another ad:
            Advertisement.Load(_adUnitId, this);
        }
    }
 
    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 
    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
 
    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}