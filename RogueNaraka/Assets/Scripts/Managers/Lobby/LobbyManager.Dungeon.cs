using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RogueNaraka.TitleScripts;

public partial class LobbyManager : MonoBehaviour
{
    [Header("Dungeon")]
    public GameObject DungeonPnl;
    public Image DungeonBtn;

    public void OpenDungeonPnl(){
        DungeonPnl.SetActive(true);
        StatPnl.SetActive(false);
        if (OnSelectButtonCorou != null)
            StopCoroutine(OnSelectButtonCorou);
        OnSelectButtonCorou = StartCoroutine(OnSelectButton(MenuButtons[2]));
    }

    public void OnEnter(){
        AudioManager.instance.PlayMusic("cave");
        GameManager.instance.Load();
        TitleManager.Instance.gameObject.SetActive(false);
        BoardManager.instance.tilemap.gameObject.SetActive(true);
        Content.SetActive(false);
    }
}
