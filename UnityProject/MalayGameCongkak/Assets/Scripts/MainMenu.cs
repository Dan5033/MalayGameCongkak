using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

    [Header("Move")]
    public Rigidbody2D[] marbles;
    [SerializeField] private GameObject settings;

    [Header("Test Mode")]
    public static bool testMode = true;

    private void Awake()
    {
        SaveData.LoadGame();
    }

    void Start ()
    {
        foreach (Rigidbody2D i in marbles)
        {
            i.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            i.angularVelocity = Random.Range(-1, 1);
        }

        AdManager.InitializeAdMob();
        AdManager.RequestBanner();
    }

    private void Update()
    {
        settings.transform.Rotate(new Vector3(0, 0, 1));
    }

    public void CloseGame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        AdManager.bannerView.Destroy();
        Application.Quit();
    }

    public void GoToRoom(string sceneName)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(sceneName);
        AdManager.bannerView.Destroy();
    }
}
