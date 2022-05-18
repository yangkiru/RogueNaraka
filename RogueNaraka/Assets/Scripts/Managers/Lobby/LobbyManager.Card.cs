using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RogueNaraka.TitleScripts;

public partial class LobbyManager : MonoBehaviour
{
    [Header("Card")]
    public string[] CardAlertStrs;
    public void OpenCardPnl(){
        SetAlert(CardAlertStrs[(int)GameManager.language]);
    }
}
