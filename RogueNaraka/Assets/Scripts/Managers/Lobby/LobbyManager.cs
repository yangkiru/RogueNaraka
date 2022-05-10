using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public GameObject Content;
    public Image[] MenuButtons;
    private Coroutine OnSelectButtonCorou;
    private void Awake(){
        Instance = this;
        OpenDungeonPnl();
    }

    private IEnumerator OnSelectButton(Image img){
        Color c = img.color;
        float a;
        for(int i = 0; i < MenuButtons.Length; i++){
            MenuButtons[i].transform.localPosition = Vector3.zero;
            MenuButtons[i].color = Color.white;
        }
        float t = 0;
        while(t < 1) {
            t += Time.unscaledDeltaTime * 2;
            a = Mathf.Lerp(1, 0.5f, t);
            c.a = a;
            img.color = c;
            img.transform.localPosition = Vector3.Lerp(img.transform.localPosition, new Vector3(0, -8, 0), t);
            yield return null;
        }
        OnSelectButtonCorou = null;
    }
}
