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
    public Button LeftDungeonBtn;
    public Button RightDungeonBtn;
    public TextMeshProUGUI DungeonResultTxt;
    public TextMeshProUGUI DungeonNameTxt;
    public Image DungeonImg;

    public void OpenDungeonPnl(){
        DungeonPnl.SetActive(true);
        StatPnl.SetActive(false);
        IAPPnl.SetActive(false);
        if (onSelectButtonCorou != null)
            StopCoroutine(onSelectButtonCorou);
        onSelectButtonCorou = StartCoroutine(OnSelectButton(MenuButtons[2]));
        InitDungeonInfo(0);
        if (PlayerPrefs.GetInt("isRun") == 1) {
            // 추후 플레이중인 던전 정보도 저장
            LockStatBtn();
            LeftDungeonBtn.interactable = false;
            RightDungeonBtn.interactable = false;
        }else {
            ActivateButton(MenuButtons[3]); // 스탯 버튼 활성화
        }
    }

    private void InitDungeonInfo(int stage){
        // 임시로 강제 지정, 추후 정글 열리면 DB 의존해서 변경
        DungeonNameTxt.text = "Naraka";
        DungeonResultTxt.text = "";
        // 임시로 강제 잠금
        LeftDungeonBtn.interactable = false;
        RightDungeonBtn.interactable = false;
    }

    public void OnEnter(){
        AudioManager.instance.PlayMusic("cave");
        GameManager.instance.Load();
        TitleManager.Instance.gameObject.SetActive(false);
        BoardManager.instance.tilemap.gameObject.SetActive(true);
        Content.SetActive(false);
    }
}
