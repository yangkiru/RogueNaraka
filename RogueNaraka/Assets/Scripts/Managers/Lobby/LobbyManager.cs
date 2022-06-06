using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public GameObject Content;
    public Button[] MenuButtons;
    public Sprite LockBtnSpr;
    public Image AlertPnl;
    public TextMeshProUGUI AlertTxt;

    private Coroutine onSelectButtonCorou;
    private Coroutine alertCorou;

    private void Awake(){
        Instance = this;
        OpenDungeonPnl();
    }

    private IEnumerator OnSelectButton(Button btn){
        Image img = btn.image;
        Color c = img.color;
        float a;
        for(int i = 0; i < MenuButtons.Length; i++){
            MenuButtons[i].transform.localPosition = Vector3.zero;
            MenuButtons[i].image.color = Color.white;
            MenuButtons[i].interactable = true;
        }
        btn.interactable = false;
        float t = 0;
        while(t < 1) {
            t += Time.unscaledDeltaTime * 2;
            a = Mathf.Lerp(1, 0.5f, t);
            c.a = a;
            img.color = c;
            img.transform.localPosition = Vector3.Lerp(img.transform.localPosition, new Vector3(0, -8, 0), t);
            yield return null;
        }
        onSelectButtonCorou = null;
    }

    private void DeactivateButton(Button btn) {
        btn.interactable = false;
        btn.image.transform.localPosition = new Vector3(0, -8, 0);
        Color c = btn.image.color;
        c.a = 0.5f;
        btn.image.color = c;
    }

    private void ActivateButton(Button btn) {
        btn.interactable = true;
        btn.image.transform.localPosition = Vector3.zero;
        Color c = btn.image.color;
        c.a = 1;
        btn.image.color = c;
    }

    public void SetAlert(string text) {
        AlertTxt.text = text;
        AlertPnl.gameObject.SetActive(true);
        if (alertCorou != null) StopCoroutine(alertCorou);
        alertCorou = StartCoroutine(AlertCoroutine());
    }

    private IEnumerator AlertCoroutine(){
        Color PnlColor = AlertPnl.color;
        Color TxtColor = AlertTxt.color;
        PnlColor.a = 1;
        TxtColor.a = 1;
        float t = 1;
        AlertPnl.color = PnlColor;
        while(t > 0){
            t -= Time.unscaledDeltaTime*0.05f;
            PnlColor.a = Mathf.Lerp(0, PnlColor.a, t);
            TxtColor.a = Mathf.Lerp(0, TxtColor.a, t);
            AlertPnl.color = PnlColor;
            AlertTxt.color = TxtColor;
            yield return null;
        }
        AlertPnl.color = PnlColor;
        AlertPnl.gameObject.SetActive(false);
        alertCorou = null;
    }
}
