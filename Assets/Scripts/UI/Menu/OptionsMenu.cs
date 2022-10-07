using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterAudioMixer;
    
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Control Options")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle invertYAxisToggle;
    
    private const string MASTER_VOLUME_KEY = "Master_Volume";
    private const string MUSIC_VOLUME_KEY = "Music_Volume";
    private const string SFX_VOLUME_KEY = "SFX_Volume";
    private const string MOUSE_SENSITIVITY_KEY = "Mouse_Sensitivity";
    private const string INVERT_MOUSE_Y_KEY = "Invert_Mouse_Y";
    
    private float _currentMasterVolume;
    private float _currentMusicVolume;
    private float _currentSFXVolume;
    private float _currentMouseSensitivity;
    
    
    private void OnEnable()
    {
        LoadSettings();
    }


    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat(MASTER_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentMasterVolume = volume;      
    }    
    
    public void SetMusicVolume(float volume)
    {
        masterAudioMixer.SetFloat(MUSIC_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentMusicVolume = volume;      
    }    
    
    public void SetSFXVolume(float volume)
    {
        masterAudioMixer.SetFloat(SFX_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentSFXVolume = volume;      
    }
    
    public void SetMouseSensitivity(float sensitivity)
    {
        _currentMouseSensitivity = sensitivity;
    }
    
    public void SaveSettings()
    {

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _currentMasterVolume); 
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _currentMusicVolume); 
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _currentSFXVolume);
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, _currentMouseSensitivity);
        SettingsManager.MouseSensitivity = _currentMouseSensitivity;
        PlayerPrefs.SetInt(INVERT_MOUSE_Y_KEY, invertYAxisToggle.isOn ? 1 : 0);
        SettingsManager.InvertMouseY = invertYAxisToggle.isOn;
    }
    
    public void LoadSettings()
    {
       
        if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
        {
            masterAudioMixer.SetFloat(MASTER_VOLUME_KEY, PlayerPrefs.GetFloat(MASTER_VOLUME_KEY));
            masterVolumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
        }        
        else
        {
            masterAudioMixer.SetFloat(MASTER_VOLUME_KEY, .5F);
            masterVolumeSlider.value = .5F;
        }
        
        if(PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
        {
            masterAudioMixer.SetFloat(MUSIC_VOLUME_KEY, PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY));
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        }        
        else
        {
            masterAudioMixer.SetFloat(MUSIC_VOLUME_KEY, .5F);
            musicVolumeSlider.value = .5F;
        }
        
        if(PlayerPrefs.HasKey(SFX_VOLUME_KEY))
        {
            masterAudioMixer.SetFloat(SFX_VOLUME_KEY, PlayerPrefs.GetFloat(SFX_VOLUME_KEY));
            sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
        }        
        else
        {
            masterAudioMixer.SetFloat(SFX_VOLUME_KEY, .5F);
            sfxVolumeSlider.value = .5F;
        }
        
        if(PlayerPrefs.HasKey(MOUSE_SENSITIVITY_KEY))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY);
        }        
        else
        {
            mouseSensitivitySlider.value = 1F;
        }
        
        if(PlayerPrefs.HasKey(INVERT_MOUSE_Y_KEY))
        {
            invertYAxisToggle.isOn = PlayerPrefs.GetInt(INVERT_MOUSE_Y_KEY) == 1;
        }        
        else
        {
            invertYAxisToggle.isOn = false;
        }
        
    }
    
    public void ExitOptions()
    {
        SaveSettings();
        gameObject.SetActive(false);
    }
}
