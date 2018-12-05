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
        AudioController.instance.PlaySoundEffect(Context.SliderChange);
        switch ((int) diffSlider.value)
        {
            case 0:
                diffText.text = "Easy";
                AIGame.difficulty = Difficulty.easy;
                AIGame.timePerTurn = 0;
                break;
            case 1:
                diffText.text = "Medium";
                AIGame.difficulty = Difficulty.medium;
                AIGame.timePerTurn = 10;
                break;
            case 2:
                diffText.text = "Hard";
                AIGame.difficulty = Difficulty.hard;
                AIGame.timePerTurn = 5;
                break;
        }
        diffImage.sprite = diffSprite[(int) diffSlider.value];
    }

    public void StartGame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(aiGameName);
    }

    public void ReturnToTheMainManu()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(mainMenuName);
    }
}
