using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour {

    [Header("UI Elements")]
    [SerializeField] private Text modeText;
    [SerializeField] private Slider modeSlider;
    [SerializeField] private Text miscText;
    [SerializeField] private Slider miscSlider;
    [SerializeField] private Text startText;
    [SerializeField] private Slider startSlider;
    [SerializeField] private Text numText;
    [SerializeField] private Slider numSlider;

    [SerializeField] private string gameSceneName;
    [SerializeField] private string mainMenuSceneName;

    // Use this for initialization
    void Start () {
        ChangeMode();
        ChangeMisc();
        ChangeStartStyle();
        ChangeMarbleNum();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeMarbleNum()
    {
        Game.marblePerSlot = (int) numSlider.value;
        numText.text = ((int)numSlider.value + 1) + " Marbles per Village";
    }

    public void ChangeMode()
    {
        Game.gameMode = (GameMode)modeSlider.value;

        switch ((int) modeSlider.value)
        {
            case 0:
                modeText.text = "Best of";
                miscSlider.maxValue = 4;
                break;
            case 1:
                modeText.text = "Burning Villages";
                miscSlider.maxValue = 6;
                break;
            case 2:
                modeText.text = "Speed Congkak";
                miscSlider.maxValue = 9;
                break;
        }
        miscSlider.value = 0;

        ChangeMisc();
    }

    public void ChangeMisc()
    {
        Game.miscNum = (int) miscSlider.value;

        switch ((int) modeSlider.value)
        {
            case 0:
                int rounds = (int) (1 + miscSlider.value * 2);
                if (rounds == 1)
                {
                    miscText.text = 1 + " Round";
                } else
                {
                    miscText.text = rounds + " Rounds";
                }
                break;
            case 1:
                miscText.text = (1 + miscSlider.value) + " Burnt Villages max";
                break;
            case 2:
                miscText.text = (1 + miscSlider.value) + " Seconds per turn";
                break;
        }
    }

    public void ChangeStartStyle()
    {
        Game.startStyle = (StartStyle) startSlider.value;

        switch ((int) startSlider.value)
        {
            case 0:
                startText.text = "P1 Starts First";
                break;
            case 1:
                startText.text = "P2 Starts First";
                break;
            case 2:
                startText.text = "Start Together";
                break;
        }
    }

    public void Startgame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
