using System.Collections;
using UnityEngine;

public partial class DeathManager : MonoBehaviour {
    [Header("BtnPnl")]
    public Transform btnPnl;
    public InterstitialAds interstitialAds;
    private const float OPEN_SPEED = 1.5f;
    private const float CLOSE_SPEED = 1.5f;

    private IEnumerator OpenBtnPnlCorou(){
        // Close
        float closeTime = 0;
        btnPnl.localScale = Vector3.zero;
        btnPnl.gameObject.SetActive(true);
        do {
            yield return null;
            closeTime += Time.deltaTime * CLOSE_SPEED;
            btnPnl.localScale = Vector3.Lerp(btnPnl.localScale, Vector3.one, closeTime);
        } while(closeTime < 1);
        interstitialAds.Show();
    }
}