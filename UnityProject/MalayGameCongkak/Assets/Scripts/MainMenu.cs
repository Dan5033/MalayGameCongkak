using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

    [Header("Scene Names")]
    [SerializeField] private string modeSelectName = "";
    [SerializeField] private string diffSelectName = "";

    [Header("Adevertising")]
    public string bannerPlacement = "mainMenuBanner";
    public bool testMode = false;

    #if UNITY_IOS
        public const string gameID = "2894540";
    #elif UNITY_ANDROID
        public const string gameID = "2864542";
    #elif UNITY_EDITOR
        public const string gameID = "1111111";
    #endif

    void Start ()
    {
        Advertisement.Initialize(gameID, testMode);
        StartCoroutine(ShowBannerWhenReady());
    }
	
	void Update () {
		
	}

    public void GoToModeSelection()
    {
        SceneManager.LoadScene(modeSelectName);
    }

    public void GoToDifficultySelection()
    {
        SceneManager.LoadScene(diffSelectName);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(bannerPlacement))
        {
            Debug.Log(Advertisement.IsReady(bannerPlacement));
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Show(bannerPlacement);
    }
}
