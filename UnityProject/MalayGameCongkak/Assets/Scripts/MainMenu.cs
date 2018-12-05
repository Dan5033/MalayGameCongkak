using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

    [Header("Scene Names")]
    [SerializeField] private string modeSelectName = "";
    [SerializeField] private string diffSelectName = "";
    [SerializeField] private string settingsName = "Settings";

    [Header("Move")]
    public Rigidbody2D[] marbles;
    [SerializeField] private GameObject settings;

    [Header("Test Mode")]
    public static bool testMode = true;

    void Start ()
    {

        foreach (Rigidbody2D i in marbles)
        {
            i.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            i.angularVelocity = Random.Range(-1, 1);
        }
    }

    private void Update()
    {
        settings.transform.Rotate(new Vector3(0, 0, 1));
    }

    public void GoToModeSelection()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(modeSelectName);
        AdManager.bannerView.Destroy();
    }

    public void GoToDifficultySelection()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(diffSelectName);
        AdManager.bannerView.Destroy();
    }

    public void CloseGame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        Application.Quit();
    }

    public void GoToSettings()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(settingsName);
        AdManager.bannerView.Destroy();
    }
}
