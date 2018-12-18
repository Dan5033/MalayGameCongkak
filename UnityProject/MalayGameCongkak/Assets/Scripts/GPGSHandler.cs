using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSHandler : MonoBehaviour {

    public static GPGSHandler instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        //Singleton Pattern
        if (instance == null)
        {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
    }

    void Start ()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
	}

    private void SignIn()
    {
        Social.localUser.Authenticate(success => { });
    }

    public void ShowAchievements()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboards()
    {
        Social.ShowLeaderboardUI();
    }

    public void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, success => { });
    }

    public void ReportMaster()
    {
        System.DateTime now = System.DateTime.Now;
        int score = now.Year * 10000 + now.Month * 100 + now.Day;
        Social.ReportScore(score, GPGSIds.leaderboard_master_of_masters, success => { });
    }

    public void IncrementEvent(string id)
    {
        PlayGamesPlatform.Instance.Events.IncrementEvent(id, 1);
    }
}
