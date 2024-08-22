using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ToggleSettings : MonoBehaviour
{
    private AudioSource[] _audioSources;

    private int prefsForAudio;
    private int prefsForVibrate;

    public const string soundStr = "sound";
    public const string vibrationStr = "vibration";

    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;
    [SerializeField] private GameObject vibrateOn;
    [SerializeField] private GameObject vibrateOff;

    void Start()
    {
        _audioSources = FindObjectsOfType<AudioSource>();
        
        SetVibration();
        SetSound();
    }

    private void SetVibration()
    {
        var vibration = PlayerPrefs.GetInt(vibrationStr);
        switch (vibration)
        {
            case 0:
                Vibration.enable = false;
                vibrateOn.SetActive(false);
                vibrateOff.SetActive(true);
                break;
            case 1:
                Vibration.enable = true;
                vibrateOn.SetActive(true);
                vibrateOff.SetActive(false);
                break;
        }
    }

    private void SetSound()
    {
        if (!PlayerPrefs.HasKey(soundStr)) return;
        var allowSound = PlayerPrefs.GetInt(soundStr);
        
        switch (allowSound)
        {
            case 0:
                MuteAllSounds();
                soundOff.SetActive(true);
                soundOn.SetActive(false);
                break;
            case 1:
                UnMuteAllSounds();
                soundOff.SetActive(false);
                soundOn.SetActive(true);
                break;
        }
    }

    public void MuteAllSounds()
    {
        foreach (var aSource in _audioSources)
        {
            aSource.mute = true;
        }

        
        SetPrefs(soundStr, 0);
    }

    public void UnMuteAllSounds()
    {
        foreach (var aSource in _audioSources)
        {
            aSource.mute = false;
        }
        SetPrefs(soundStr, 1);
    }

    public void SetVibration(bool val)
    {
        Vibration.enable = val;
        
        switch (val)
        {
            case false:
                SetPrefs(vibrationStr, 0);
                break;
            case true:
                SetPrefs(vibrationStr, 1);
                break;
        }
    }

    private void SetPrefs(string str, int val)
    {
        PlayerPrefs.SetInt(str, val);
    }
}
