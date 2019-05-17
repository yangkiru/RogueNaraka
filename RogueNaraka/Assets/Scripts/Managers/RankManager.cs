using UnityEngine;
using System.Collections;
using GooglePlayGames;
using RogueNaraka.PopUpScripts;
using RogueNaraka.TheBackendScripts;

public class RankManager : MonoBehaviour
{
    public static RankManager instance;

    public int highScore { get { return PlayerPrefs.GetInt("highScore"); } set { PlayerPrefs.SetInt("highScore", value); } }

    private void Awake()
    {
        instance = this;
    }

    string leaderBoardId = "CgkIvu-SvO4aEAIQAA";

    /*
    public void Login()
    {
        if (Social.localUser.authenticated)
            return;

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
                Debug.Log("Logged In");
            else
                Debug.Log("Login Failed");
        });
    }*/

    public void SendPlayerRank()
    {
        Debug.Log("SendPlayerRank:" + (long)BoardManager.instance.stage);
        highScore = Mathf.Max(highScore, BoardManager.instance.stage);
#if UNITY_ANDROID && !UNITY_EDITOR
        //Login();
        if(TheBackendManager.Instance.IsLoginSuccess) {
            Social.ReportScore((long)BoardManager.instance.stage, leaderBoardId, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Score Updated");
                }
                else
                {
                    //upload highscore failed
                    Debug.Log("Failed Score Updating");
                }
            });
        }
#endif
    }

    //void AuthenticateHandler(bool isSuccess)
    //{
    //    if (isSuccess)
    //    {
    //        Debug.Log("LeaderBoard Login Success");
    //    }
    //    else
    //    {
    //        Debug.Log("LeaderBoard Login Failed");
    //        //login failed
    //    }

    //}

    public void ShowRank()
    {
        //Login();
        if(TheBackendManager.Instance.IsLoginSuccess) {
            Social.ShowLeaderboardUI();
        } else {
            string popUpContext = "";
            switch(GameManager.language) {
                case Language.English:
                    popUpContext = "Not available while logging out.";
                break;
                case Language.Korean:
                    popUpContext = "로그아웃 상태에서는 이용하실 수 없습니다.";
                break;
            } 
            PopUpManager.Instance.ActivateOneButtonPopUp(
                popUpContext,
                (OneButtonPopUpController _popUp) => { 
                    _popUp.DeactivatePopUp(); 
                    PopUpManager.Instance.DeactivateBackPanel();
                    GameManager.instance.SetPause(false);
                });
        }
    }

    //public void SetLocalRank()
    //{

    //}

    //public void ShowAchievement()
    //{

    //}
}
