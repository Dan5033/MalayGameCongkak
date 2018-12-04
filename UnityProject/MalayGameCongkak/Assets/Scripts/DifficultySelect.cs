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

    [Header("Images")]
    [SerializeField] private Image diffImage;
    [SerializeField] private Sprite[] diffSprite;

    // Use this for initialization
    void Start ()
    {
        AIGame.marblePerSlot = 7;
        AIGame.startStyle = StartStyle.together;
        AIGame.afterStyle = AfterStyle.StartTogether;
        AIGame.roundsToWin = 2;
        AIGame.burntVillages = true;
        AIGame.timePerTurn = 5;

        ChangeDifficulty();
	}

    public void ChangeDifficulty()
    {
        switch ((int) diffSlider.value)
        {
            case 0:
                diffText.text = "Easy";
                AIGame.difficulty = Difficulty.easy;
                break;
            case 1:
                diffText.text = "Medium";
                AIGame.difficulty = Difficulty.medium;
                break;
            case 2:
                diffText.text = "Hard";
                AIGame.difficulty = Difficulty.hard;
                break;
        }
        diffImage.sprite = diffSprite[(int) diffSlider.value];
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
