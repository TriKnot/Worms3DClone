using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
//https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterAudioMixer;
    
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Graphics DropDowns")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown textureDropdown;
    [SerializeField] private TMP_Dropdown aaDropdown;


    private const string RESOLUTION_KEY = "ResolutionSettingPreference";
    private const string QUALITY_KEY = "QualitySettingPreference";
    private const string TEXTURE_KEY = "TextureSettingPreference";
    private const string AA_KEY = "AntiAliasingSettingPreference";
    private const string MASTER_VOLUME_KEY = "Master_Volume";
    private const string MUSIC_VOLUME_KEY = "Music_Volume";
    private const string SFX_VOLUME_KEY = "SFX_Volume";
    
    
    
    
    private Resolution[] _resolutions;
    private float _currentMasterVolume;
    private float _currentMusicVolume;
    private float _currentSFXVolume;
    private int _currentResolutionIndex;
    
    private bool _hasSaved = false;


    private void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        _resolutions = Screen.resolutions;
        _currentResolutionIndex = 0;        
        
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + 
                            _resolutions[i].height;
            options.Add(option);
            if (_resolutions[i].width == Screen.currentResolution.width 
                && _resolutions[i].height == Screen.currentResolution.height)
                _currentResolutionIndex = i;
        }
        
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }

    private void OnEnable()
    {
        LoadSettings(_currentResolutionIndex);
    }


    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat(MASTER_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentMasterVolume = volume;      
        _hasSaved = false;
    }    
    
    public void SetMusicVolume(float volume)
    {
        masterAudioMixer.SetFloat(MUSIC_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentMusicVolume = volume;      
        _hasSaved = false;
    }    
    
    public void SetSFXVolume(float volume)
    {
        masterAudioMixer.SetFloat(SFX_VOLUME_KEY, Mathf.Log10(volume) * 20);
        _currentSFXVolume = volume;      
        _hasSaved = false;
    }
    
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = Screen.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);      
        _hasSaved = false;
    }
    
    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.masterTextureLimit = textureIndex;
        qualityDropdown.value = 6;      
        _hasSaved = false;
    }
    
    public void SetAntiAliasing(int antiAliasingIndex)
    {
        QualitySettings.antiAliasing = antiAliasingIndex;
        qualityDropdown.value = 6;       
        _hasSaved = false;
    }
    
    public void SetQuality(int qualityIndex)
    {
        if(qualityIndex != 6)
            QualitySettings.SetQualityLevel(qualityIndex);

        switch (qualityIndex)
        {
            case 0: // quality level - very low
                textureDropdown.value = 3;
                aaDropdown.value = 0;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 0;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;        
        }
        qualityDropdown.value = qualityIndex;
        _hasSaved = false;
    }
    
    public void SaveSettings()
    {
        PlayerPrefs.SetInt(QUALITY_KEY, qualityDropdown.value);
        PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionDropdown.value);
        PlayerPrefs.SetInt(TEXTURE_KEY, textureDropdown.value);
        PlayerPrefs.SetInt(AA_KEY, aaDropdown.value);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _currentMasterVolume); 
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _currentMusicVolume); 
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _currentSFXVolume);
        _hasSaved = true;
    }
    
    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey(QUALITY_KEY))
        {
            qualityDropdown.value = PlayerPrefs.GetInt(QUALITY_KEY);
        }        
        else
        {
            qualityDropdown.value = 3;
        }    
        
        if (PlayerPrefs.HasKey(RESOLUTION_KEY))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt(RESOLUTION_KEY);
        }        
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }   
        
        if (PlayerPrefs.HasKey(TEXTURE_KEY))
        {
            textureDropdown.value = PlayerPrefs.GetInt(TEXTURE_KEY);
        }        
        else
        {
            textureDropdown.value = 0;
        }        
        
        if (PlayerPrefs.HasKey(AA_KEY))
        {
            aaDropdown.value = PlayerPrefs.GetInt(AA_KEY);
        }        
        else
        {
            aaDropdown.value = 1;
        }
        
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
        
    }
    
    public void ExitOptions()
    {
        if(!_hasSaved)
        {
            Debug.Log("Save your settings first!");
        }    
        gameObject.SetActive(false);
    }
}
