using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCheck : MonoBehaviour
{
    public GameObject pnl;
    public TMPro.TextMeshProUGUI txt;
    private void Start()
    {
        CheckNetwork();
    }

    public void CheckNetwork()
    {
        if (networkCheckCorou == null)
        {
            networkCheckCorou = NetworkCheckCorou();
            StartCoroutine(networkCheckCorou);
        }
    }

    IEnumerator networkCheckCorou;
    IEnumerator NetworkCheckCorou()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
            pnl.SetActive(true);
        else
            yield break;

        switch(GameManager.language)
        {
            case Language.English:
                txt.text = "Must be connected to the network.";
                break;
            case Language.Korean:
                txt.text = "네트워크에 연결되어\n있어야 합니다.";
                break;
        }

        do
        {
            yield return null;
        } while (Application.internetReachability == NetworkReachability.NotReachable);
        networkCheckCorou = null;
        pnl.SetActive(false);

        AdMobManager.instance.Start();
    }
}
