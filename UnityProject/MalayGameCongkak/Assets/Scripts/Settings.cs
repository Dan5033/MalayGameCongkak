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

    [SerializeField] string mainMenuName;

    private void Start()
    {
        bgmSlider.value = Mathf.RoundToInt(AudioController.BGMVol * 100);
        sfxSlider.value = Mathf.RoundToInt(AudioController.SFXVol * 100);
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
}
