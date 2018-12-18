using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModeSelectFreePlay : ModeSelect {

    [SerializeField] private Slider diffSlider;
    [SerializeField] private Text diffText;
    private List<Style> availableStyles = new List<Style>();

    private new void Start ()
    {
        base.Start();

        AIGame.master = Masters.Free;

        for (int i = 0; i < (int) Masters.TokSenah + 1; i++)
        {
            if (JSONSaveData.currentSave.defeated[i])
            {
                availableStyles.Add((Style)i);
            }
        }

        diffSlider.maxValue = availableStyles.Count - 1;
        if (availableStyles.Count > 1)
        {
            diffSlider.interactable = true;
        }

        ChangeAI(false);
    }

    public void ChangeAI(bool sound = true)
    {

        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        AIGame.style = availableStyles[(int) diffSlider.value];
        switch (AIGame.style)
        {
            case Style.Simple:
                diffText.text = "Simple Style AI";
                break;
            case Style.Speed:
                diffText.text = "Speed Style AI";
                break;
            case Style.Defense:
                diffText.text = "Defensive Style AI";
                break;
            case Style.Predictive:
                diffText.text = "Predictive Style AI";
                break;
            case Style.Balanced:
                diffText.text = "Balanced Style AI";
                break;
            case Style.Offensive:
                diffText.text = "Offensive Style AI";
                break;
            case Style.Complex:
                diffText.text = "Complex Style AI";
                break;
        }
        if (!diffSlider.interactable)
        {
            diffText.text = lockedString;
        }
    }

}
