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
    public void OpenIAPPnl(){
        MoneyManager.instance.Load();
        IAPCoinTxt.text = "Coin : " + MoneyManager.instance.soul;
        StatPnl.SetActive(false);
        DungeonPnl.SetActive(false);
        IAPPnl.SetActive(true);

        if(PlayerPrefs.GetInt("RemoveAds") == 1) RemoveAdsBtn.interactable = false;

        if (OnSelectButtonCorou != null)
            StopCoroutine(OnSelectButtonCorou);
        OnSelectButtonCorou = StartCoroutine(OnSelectButton(MenuButtons[0]));

        if (PlayerPrefs.GetInt("isRun") == 1) {
            // 추후 플레이중인 던전 정보도 저장
            DeactivateButton(MenuButtons[3]); // 스탯 버튼 비활성화
        }else {
            ActivateButton(MenuButtons[3]); // 스탯 버튼 활성화
        }
    }
    public void PurchaseRemoveAds() {
        PlayerPrefs.SetInt("RemoveAds", 1);
        RemoveAdsBtn.interactable = false;
        Debug.Log("RemoveAds");
    }
}
