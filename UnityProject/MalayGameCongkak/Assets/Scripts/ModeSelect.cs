using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelect : MonoBehaviour {

    [Header("UI Elements")]
    [SerializeField] protected Text marbleNumText;
    [SerializeField] protected Slider marbleNumSlider;
    [SerializeField] protected Text startText;
    [SerializeField] protected Slider startSlider;
    [SerializeField] protected Text afterText;
    [SerializeField] protected Slider afterSlider;
    [SerializeField] protected Text roundText;
    [SerializeField] protected Slider roundSlider;
    [SerializeField] protected Text burnText;
    [SerializeField] protected Slider burnSlider;
    [SerializeField] protected Text timeText;
    [SerializeField] protected Slider timeSlider;

    protected const string lockedString = "Locked";

    // Use this for initialization
    protected void Start () {
        //Lee
        if (JSONSaveData.currentSave.defeated[1])
        {
            timeSlider.interactable = true;
        }
        //Murugam
        if (JSONSaveData.currentSave.defeated[2])
        {
            roundSlider.interactable = true;
        }
        //Kamal
        if (JSONSaveData.currentSave.defeated[3])
        {
            burnSlider.interactable = true;
        }
        //Eric
        if (JSONSaveData.currentSave.defeated[4])
        {
            afterSlider.interactable = true;
            startSlider.interactable = true;
        }
        //Esther
        if (JSONSaveData.currentSave.defeated[5])
        {
            marbleNumSlider.interactable = true;

        }

        ChangeTimePerTurn(false);
        ChangeRoundToWin(false);
        ChangeBurningRule(false);
        ChangeStartStyle(false);
        ChangeAfterStyle(false);
        ChangeMarbleNumber(false);
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
        if (!marbleNumSlider.interactable)
        {
            marbleNumText.text = lockedString;
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
        if (!startSlider.interactable)
        {
            startText.text = lockedString;
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
        if (!afterSlider.interactable)
        {
            afterText.text = lockedString;
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
        if (!roundSlider.interactable)
        {
            roundText.text = lockedString;
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
        if (!burnSlider.interactable)
        {
            burnText.text = lockedString;
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
        if (!timeSlider.interactable)
        {
            timeText.text = lockedString;
        }
    }

    public void RoomGoTo(string room)
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(room);
    }
}
