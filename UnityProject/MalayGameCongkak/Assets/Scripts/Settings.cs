using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour {

    [Header("Sliders")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Text bgmText;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Text sfxText;
    [SerializeField] Text warning;

    [SerializeField] string mainMenuName;

    private int resetCounter = 7;
    private float timer = 0;

    private void Start()
    {
        bgmSlider.value = Mathf.RoundToInt(AudioController.BGMVol * 100);
        sfxSlider.value = Mathf.RoundToInt(AudioController.SFXVol * 100);
    }

    private void Update()
    {
        if (resetCounter < 7)
        {
            timer += Time.deltaTime;
            if (timer > 3)
            {
                timer = 0;
                resetCounter = 7;
            }
        }

        if (resetCounter < 7 && resetCounter > 0)
        {
            warning.text = "Tap " + resetCounter + " more time to reset data \n This cannot be reverted.";
        } else if (resetCounter <= 0)
        {
            warning.text = "Data Reset";
        } else
        {
            warning.text = "";
        }

    }

    public void ChangeBGM()
    {
        AudioController.BGMVol = bgmSlider.value/100;
        bgmText.text = "BGM: " + Mathf.RoundToInt(bgmSlider.value);
        AudioController.instance.PlaySoundEffect(Context.SliderChange);
        AudioController.instance.ResetVolume();

        //Save
        SaveData.currentSave.BGMVol = AudioController.BGMVol;
    }

    public void ChangeSFX()
    {
        AudioController.SFXVol = sfxSlider.value/100;
        sfxText.text = "SFX: " + Mathf.RoundToInt(sfxSlider.value);
        AudioController.instance.PlaySoundEffect(Context.SliderChange);
        AudioController.instance.ResetVolume();

        //Save
        SaveData.currentSave.SFXVol = AudioController.SFXVol;
    }

    public void ReturnToMainMenu()
    {
        SaveData.SaveGame();
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        SceneManager.LoadScene(mainMenuName);
    }

    public void ResetData()
    {
        AudioController.instance.PlaySoundEffect(Context.ButtonPress);
        if (resetCounter > 0)
        {
            resetCounter--;
            timer = 0;
        }
        if (resetCounter == 0)
        {
            SaveData.currentSave = new SaveData();
            SaveData.SaveGame();
        }
    }
}
