using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DifficultySelect : MonoBehaviour {

    [Header("UI")]
    [SerializeField] private Slider diffSlider;
    [SerializeField] private Text diffText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuName;
    [SerializeField] private string aiGameName;

    // Use this for initialization
    void Start ()
    {
        ChangeDifficulty();
	}

    public void ChangeDifficulty()
    {
        switch ((int) diffSlider.value)
        {
            case 0:
                diffText.text = "Easy";
                break;
            case 1:
                diffText.text = "Medium";
                break;
            case 2:
                diffText.text = "Hard";
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(aiGameName);
    }

    public void ReturnToTheMainManu()
    {
        SceneManager.LoadScene(mainMenuName);
    }
}
