using UnityEngine;
using System.Collections;
using GooglePlayGames;

public class RankManager : MonoBehaviour
{
    public static RankManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
#if UNITY_ANDROID
        if (Application.internetReachability == NetworkReachability.NotReachable)
            return;

        PlayGamesPlatform.Activate();
        Login();
#endif
    }

    string leaderBoardId = "CgkIvu-SvO4aEAIQAA";

    //int highScore;
    //public int GetHighScore(out bool isHighScoreUpdate)
    //{
    //    int last = PlayerPrefs.GetInt("highScore");
    //    int current = BoardManager.instance.stage;
    //    int result = Mathf.Max(last, current);

    //    if (last == result) isHighScoreUpdate = true;
    //    else isHighScoreUpdate = false;
    //    if (isHighScoreUpdate)
    //        Debug.Log("It's HighScore:" + result);
    //    else
    //        Debug.Log("It's not HighScore:" + result);
    //    return result;
    //}

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
    }

    public void SendPlayerRank()
    {
        Debug.Log("SendPlayerRank:" + (long)BoardManager.instance.stage);
#if UNITY_ANDROID
        Login();
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
        Login();
        Social.ShowLeaderboardUI();
    }

    //public void SetLocalRank()
    //{

    //}

    //public void ShowAchievement()
    //{

    //}
}
