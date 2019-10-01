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
    [SerializeField] Slider dispSlider;
    [SerializeField] Text dispText;
    [SerializeField] GameObject design;
    [SerializeField] Slider designSlider;
    [SerializeField] Text designText;
    [SerializeField] Image designDisplay;
    [SerializeField] Text warning;

    [SerializeField] string mainMenuName;

    private int resetCounter = 7;
    private float timer = 0;

    private List<MarbleDesign> available = new List<MarbleDesign>();

    private void Start()
    {
        LoadFromSavedata();
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

        designDisplay.transform.eulerAngles += new Vector3(0, 0, 1);
    }

    public void ChangeBGM(bool sound = false)
    {
        JSONSaveData.currentSave.BGMVol = bgmSlider.value/100;
        bgmText.text = "BGM: " + Mathf.RoundToInt(bgmSlider.value);
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        AudioController.instance.ResetVolume();
    }

    public void ChangeSFX(bool sound = false)
    {
        JSONSaveData.currentSave.SFXVol = sfxSlider.value/100;
        sfxText.text = "SFX: " + Mathf.RoundToInt(sfxSlider.value);
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        AudioController.instance.ResetVolume();
    }

    public void ChangeDisplayType(bool sound = false)
    {
        switch ((int) dispSlider.value)
        {
            case 0:
                dispText.text = "Meeple Only";
                break;
            case 1:
                dispText.text = "Meeple and Text";
                break;
            case 2:
                dispText.text = "Text Only";
                break;
        }
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }

        //Save
        JSONSaveData.currentSave.displayType = (int) dispSlider.value;
    }

    public void ChangeMarbleDesign(bool sound = false)
    {
        MarbleDesign des = available[(int)designSlider.value];
        designText.text = Marble.names[(int) des];
        designDisplay.sprite = Marble.sprites[(int)des];
        if (sound)
        {
            AudioController.instance.PlaySoundEffect(Context.SliderChange);
        }
        JSONSaveData.currentSave.selectedDesign = available[(int) designSlider.value];
    }

    public void ReturnToMainMenu()
    {
        GPGSHandler.instance.SaveGame();
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
            JSONSaveData.currentSave = new JSONSaveData();
            GPGSHandler.instance.SaveGame();
            LoadFromSavedata();
        }
    }

    private void LoadFromSavedata()
    {
        bgmSlider.value = Mathf.RoundToInt(JSONSaveData.currentSave.BGMVol * 100);
        sfxSlider.value = Mathf.RoundToInt(JSONSaveData.currentSave.SFXVol * 100);
        dispSlider.value = JSONSaveData.currentSave.displayType;

        ChangeBGM(false);
        ChangeSFX(false);
        ChangeDisplayType(false);


        available = new List<MarbleDesign>();
        for (int i = 0; i < (int)MarbleDesign.Golden + 1; i++)
        {
            if (JSONSaveData.currentSave.marbleUnlocked[i])
            {
                available.Add((MarbleDesign)i);
            }
        }

        if (available.Count < 2)
        {
            design.SetActive(false);
        }
        else
        {
            design.SetActive(true);
            int index = 0;
            for (int i = 0; i < available.Count; i++)
            {
                if (available[i] == JSONSaveData.currentSave.selectedDesign)
                {
                    index = i;
                    break;
                }
            }
            designSlider.maxValue = available.Count - 1;
            designSlider.value = index;
            ChangeMarbleDesign(false);
        }
    }
}
