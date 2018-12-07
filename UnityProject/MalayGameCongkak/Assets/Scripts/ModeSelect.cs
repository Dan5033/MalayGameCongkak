using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour {

    [Header("UI Elements")]
    [SerializeField] private Text marbleNumText;
    [SerializeField] private Slider marbleNumSlider;
    [SerializeField] private Text startText;
    [SerializeField] private Slider startSlider;
    [SerializeField] private Text afterText;
    [SerializeField] private Slider afterSlider;
    [SerializeField] private Text roundText;
    [SerializeField] private Slider roundSlider;
    [SerializeField] private Text burnText;
    [SerializeField] private Slider burnSlider;
    [SerializeField] private Text timeText;
    [SerializeField] private Slider timeSlider;

    [SerializeField] private string gameSceneName;
    [SerializeField] private string mainMenuSceneName;
    
    // Use this for initialization
    void Start () {
        ChangeMarbleNumber(false);
        ChangeStartStyle(false);
        ChangeAfterStyle(false);
        ChangeRoundToWin(false);
        ChangeBurningRule(false);
        ChangeTimePerTurn(false);
	}

    public void ChangeMarbleNumber(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        Game.marblePerSlot = (int)marbleNumSlider.value;
        marbleNumText.text = Game.marblePerSlot.ToString();
        if (Game.marblePerSlot == 1)
        {
            marbleNumText.text += " villager per village";
        } else if (Game.marblePerSlot > 1)
        {
            marbleNumText.text += " villagers per village";
        }
    }

    public void ChangeStartStyle(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        Game.startStyle = (StartStyle)startSlider.value;
        switch (Game.startStyle)
        {
            case StartStyle.P1:
                startText.text = "P1 starts first";
                break;
            case StartStyle.P2:
                startText.text = "P2 starts first";
                break;
            case StartStyle.together:
                startText.text = "Start together";
                break;
        }
    }

    public void ChangeAfterStyle(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        Game.afterStyle = (AfterStyle)afterSlider.value;
        switch (Game.afterStyle)
        {
            case AfterStyle.P1Start:
                afterText.text = "P1 starts new match";
                break;
            case AfterStyle.P2Start:
                afterText.text = "P2 starts new match";
                break;
            case AfterStyle.RoundLoser:
                afterText.text = "Loser starts new match";
                break;
            case AfterStyle.RoundWinner:
                afterText.text = "Winner starts new match";
                break;
            case AfterStyle.StartTogether:
                afterText.text = "Start new match together";
                break;
        }
    }

    public void ChangeRoundToWin(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        Game.roundsToWin = (int)roundSlider.value;
        if (Game.roundsToWin == 0)
        {
            roundText.text = "Endless game";
        } else if (Game.roundsToWin == 1)
        {
            roundText.text = "1 round to win";
        } else
        {
            roundText.text = Game.roundsToWin + " rounds to win";
        }
    }

    public void ChangeBurningRule(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        if (burnSlider.value == 0)
        {
            Game.burntVillages = false;
            burnText.text = "Empty villages will not be burned";
        } else if (burnSlider.value == 1)
        {
            Game.burntVillages = true;
            burnText.text = "Empty villages will be burned";
        }
    }

    public void ChangeTimePerTurn(bool sound = true)
    {
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        Game.timePerTurn = timeSlider.value;
        if (Game.timePerTurn == 0)
        {
            timeText.text = "Endless turn";
        } else if (Game.timePerTurn == 1)
        {
            timeText.text = "1 Second per turn";
        } else
        {
            timeText.text = Game.timePerTurn + " Seconds per turn";
        }
    }

    public void Startgame()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToMainMenu()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
