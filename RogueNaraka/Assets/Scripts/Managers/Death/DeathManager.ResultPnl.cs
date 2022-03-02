using System.Collections;
using UnityEngine;

public partial class DeathManager : MonoBehaviour {
    [Header("ResultPnl")]
    public Transform resultPnl;
    public Transform btnPnl;
    private const float OPEN_SPEED = 1.5f;
    private const float CLOSE_SPEED = 1.5f;
    
    private IEnumerator OpenResultCorou(){
        // Open
        float openTime = 0;
        resultPnl.gameObject.SetActive(true);
        resultPnl.localScale = Vector3.zero;
        do {
            yield return null;
            openTime += Time.deltaTime * OPEN_SPEED;
            resultPnl.localScale = Vector3.Lerp(resultPnl.localScale, Vector3.one, openTime);
        } while(openTime < 1);
        resultPnl.localScale = Vector3.one;
    }

    public void CloseResultPnl(){
        StartCoroutine(CloseResultPnlCorou());
    }

    private IEnumerator CloseResultPnlCorou(){
        // Close
        float closeTime = 0;
        btnPnl.localScale = Vector3.zero;
        btnPnl.gameObject.SetActive(true);
        do {
            yield return null;
            closeTime += Time.deltaTime * CLOSE_SPEED;
            resultPnl.localScale = Vector3.Lerp(resultPnl.localScale, Vector3.zero, closeTime);
            btnPnl.localScale = Vector3.Lerp(btnPnl.localScale, Vector3.one, closeTime);
        } while(closeTime < 1);
        resultPnl.gameObject.SetActive(false);
    }
}