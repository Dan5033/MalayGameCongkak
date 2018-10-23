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

    [SerializeField] private string gameSceneName;

    // Use this for initialization
    void Start () {
        ChangeMode();
        ChangeMisc();
        ChangeStartStyle();
	}
	
	// Update is called once per frame
	void Update () {
		
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
                miscText.text = (1 + miscSlider.value * 2) + " Round(s)";
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
}
