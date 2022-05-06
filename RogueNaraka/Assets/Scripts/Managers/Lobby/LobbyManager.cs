using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    public GameObject Content;
    private void Awake(){
        Instance = this;
        OpenDungeonPnl();
    }
}
