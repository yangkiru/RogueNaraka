using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RogueNaraka.TitleScripts;

public partial class LobbyManager : MonoBehaviour
{
    [Header("IAP")]
    public GameObject IAPPnl;
    public TextMeshProUGUI IAPCoinTxt;
    public Button RemoveAdsBtn;
    public string[] IAPAlertStrs;

    public void OpenIAPPnl(){
        MoneyManager.instance.Load();
        IAPCoinTxt.text = "Coin : " + MoneyManager.instance.soul;
        StatPnl.SetActive(false);
        DungeonPnl.SetActive(false);
        IAPPnl.SetActive(true);

        if(PlayerPrefs.GetInt("isRemoveAds") == 1) RemoveAdsBtn.interactable = false;

        if (onSelectButtonCorou != null)
            StopCoroutine(onSelectButtonCorou);
        onSelectButtonCorou = StartCoroutine(OnSelectButton(MenuButtons[0]));
    }
    public void PurchaseRemoveAds() {
        PlayerPrefs.SetInt("isRemoveAds", 1);
        RemoveAdsBtn.interactable = false;
        SetAlert("Successful Purchase");
        Debug.Log("RemoveAds");
    }

    public void UnPurchaseRemoveAds() {
        PlayerPrefs.SetInt("isRemoveAds", 0);
        RemoveAdsBtn.interactable = true;
        SetAlert("Purchase Failed");
        Debug.Log("Un RemoveAds");
    }
}
