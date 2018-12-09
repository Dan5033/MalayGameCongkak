﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModeSelectFreePlay : ModeSelect {

    [SerializeField] private Slider diffSlider;
    [SerializeField] private Text diffText;

    private new void Start ()
    {
        base.Start();

        AIGame.master = Masters.Free;

        //Safiya
	    if (SaveData.currentSave.defeated[0])
        {
            diffSlider.maxValue = 0;
        }
        //Lee
        if (SaveData.currentSave.defeated[1])
        {
            diffSlider.maxValue = 1;
            diffSlider.interactable = true;
        }
        //Murugam
        if (SaveData.currentSave.defeated[2])
        {
            diffSlider.maxValue = 1;
            diffSlider.interactable = true;
        }
        //Kamal
        if (SaveData.currentSave.defeated[3])
        {
            diffSlider.maxValue = 1;
            diffSlider.interactable = true;

        }
        //Eric
        if (SaveData.currentSave.defeated[4])
        {
            diffSlider.maxValue = 1;
            diffSlider.interactable = true;

        }
        //Esther
        if (SaveData.currentSave.defeated[5])
        {
            diffSlider.maxValue = 1;
            diffSlider.interactable = true;

        }
        //Tok Senah
        if (SaveData.currentSave.defeated[6])
        {
            diffSlider.maxValue = 2;
            diffSlider.interactable = true;

        }
    }

    public void ChangeAI(bool sound = true)
    {

        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        AIGame.difficulty = (Difficulty) diffSlider.value;
        diffText.text = "AI Difficulty: ";
        switch (AIGame.difficulty)
        {
            case Difficulty.easy:
                diffText.text += "Easy";
                break;
            case Difficulty.medium:
                diffText.text += "Medium";
                break;
            case Difficulty.hard:
                diffText.text += "Hard";
                break;
        }
    }

}
