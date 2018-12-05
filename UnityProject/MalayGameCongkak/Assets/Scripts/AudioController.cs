﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Context
{
    ButtonPress,
    MarblePlace,
    HousePickup,
    HouseBomb,
    SliderChange
}

public class AudioController : MonoBehaviour {

    [Header("Singleton")]
    public static AudioController instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource asBGM;
    [SerializeField] AudioSource asSFX;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] musicPlaylist;
    [SerializeField] protected AudioClip acPop;
    [SerializeField] protected AudioClip acTake;
    [SerializeField] protected AudioClip acMoney;
    [SerializeField] protected AudioClip acClick;

    [Header("Static")]
    public static float BGMVol = 1;
    public static float SFXVol = 1;
    public const string bgmString = "BGMVolume";
    public const string sfxString = "SFXVolume";

    void Start ()
    {
        DontDestroyOnLoad(this);

        //Singleton Pattern
        if (instance == null)
        {
            instance = this;
            BGMVol = PlayerPrefs.GetFloat(bgmString,0.5f);
            SFXVol = PlayerPrefs.GetFloat(sfxString,1);
            ResetVolume();
        } else if (instance != this)
        {
            Destroy(this);
        }
	}
	
	void Update ()
    {
		if (!asBGM.isPlaying)
        {
            asBGM.clip = RandomSong();
            asBGM.Play();
        }
	}

    private AudioClip RandomSong()
    {
        return musicPlaylist[Mathf.RoundToInt(Random.value * (musicPlaylist.Length - 1))];
    }

    public void PlaySoundEffect(AudioClip sfx, float pitchVariation)
    {
        asSFX.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        asSFX.PlayOneShot(sfx);
    }

    public void PlaySoundEffect(Context context)
    {
        switch (context) {
            case Context.ButtonPress:
                PlaySoundEffect(acClick, 0);
                break;
            case Context.HousePickup:
                PlaySoundEffect(acTake, 0.2f);
                break;
            case Context.HouseBomb:
                PlaySoundEffect(acMoney, 0);
                break;
            case Context.MarblePlace:
                PlaySoundEffect(acPop, 0.2f);
                break;
            case Context.SliderChange:
                PlaySoundEffect(acClick, 0.2f);
                break;
        }
    }

    public void ResetVolume()
    {
        asBGM.volume = BGMVol;
        asSFX.volume = SFXVol;
    }
}