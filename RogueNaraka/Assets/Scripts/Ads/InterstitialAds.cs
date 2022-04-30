using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAds : MonoBehaviour
{
    const string mySurfacingId = "interstitial";

    private Coroutine _coroutine;
    [SerializeField]
    private string InterstitialAdID;

    void Start()
    {
        Advertisement.Initialize("4694417");//Android
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
    }

    public void Show()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(ShowInterstitialWhenInitialized());
    }

    public IEnumerator ShowInterstitialWhenInitialized()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (!Advertisement.isInitialized || !Advertisement.IsReady(InterstitialAdID))
        {
            if(!Advertisement.isInitialized) {
                Advertisement.Initialize("4694417");//Android
            }
            yield return wait;
        }
        Advertisement.Show(InterstitialAdID);
    }
}