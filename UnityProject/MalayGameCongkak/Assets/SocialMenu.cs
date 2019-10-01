using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocialMenu : MonoBehaviour {

    [SerializeField] WarningNotification warning;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowAchivements()
    {
        if (Social.localUser.authenticated)
        {
            GPGSHandler.instance.ShowAchievements();
        }
        else
        {
            warning.Show();
        }
    }

    public void ShowLeaderboards()
    {
        if (Social.localUser.authenticated)
        {
            GPGSHandler.instance.ShowLeaderboards();
        }
        else
        {
            warning.Show();
        }
    }

    public void ShowPaidVersion()
    {
        Application.OpenURL("market://details?id=com.GammaResources.MG.CongkakPaid");
    }
}
