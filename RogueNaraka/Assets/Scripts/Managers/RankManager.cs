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

    private void Start()
    {
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate(AuthenticateHandler);
    }

    string leaderBoardId = "CgkIvu-SvO4aEAIQAA";

    int highScore;
    public int GetHighScore(out bool isHighScoreUpdate)
    {
            int last = PlayerPrefs.GetInt("highScore");
            int current = BoardManager.instance.stage;
            int result = Mathf.Max(last, current);

            if (last == result) isHighScoreUpdate = true;
            else isHighScoreUpdate = false;

            return result;
    }
    public void SendPlayerRank()
    {
        bool isHighScore;
        highScore = GetHighScore(out isHighScore);
        if(isHighScore)
        {
            Social.ReportScore((long)highScore, leaderBoardId, (bool success) =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderBoardId);
                }
                else
                {
                    //upload highscore failed
                    Debug.Log("Failed");
                }
            });
        }
    }

    void AuthenticateHandler(bool isSuccess)
    {
        if (isSuccess)
        {
            Debug.Log("LeaderBoard Login Success");
        }
        else
        {
            Debug.Log("LeaderBoard Login Failed");
            //login failed
        }

    }

    public void ShowRank()
    {
        Social.ShowLeaderboardUI();
    }

    public void ShowAchievement()
    {

    }
}
