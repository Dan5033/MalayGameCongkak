using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MainMenu : MonoBehaviour {

    [Header("Scene Names")]
    [SerializeField] private string modeSelectName = "";
    [SerializeField] private string diffSelectName = "";

    [Header("Move")]
    public Rigidbody2D[] marbles;

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
}
