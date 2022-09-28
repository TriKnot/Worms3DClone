using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
//https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/
    [SerializeField] private AudioMixer masterAudioMixer;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioMixer effectsAudioMixer;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown textureDropdown;
    [SerializeField] private Dropdown aaDropdown;
    
    private Resolution[] _resolutions;
    private float _currentMasterVolume;
    private float _currentMusicVolume;
    private float _currentEffectsVolume;
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
        LoadSettings(_currentResolutionIndex);
    }
    
    

    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("volume", volume);
        _currentMasterVolume = volume;      
        _hasSaved = false;
    }    
    
    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("volume", volume);
        _currentMusicVolume = volume;      
        _hasSaved = false;
    }    
    
    public void SetEffectsVolume(float volume)
    {
        effectsAudioMixer.SetFloat("volume", volume);
        _currentEffectsVolume = volume;      
        _hasSaved = false;
    }
    
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;      
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
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MasterVolumePreference", _currentMasterVolume); 
        PlayerPrefs.SetFloat("MusicVolumePreference", _currentMusicVolume); 
        PlayerPrefs.SetFloat("EffectsVolumePreference", _currentEffectsVolume);
        _hasSaved = true;
    }
    
    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        }        
        else
        {
            qualityDropdown.value = 3;
        }    
        
        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        }        
        else
        {
            resolutionDropdown.value = currentResolutionIndex;
        }   
        
        if (PlayerPrefs.HasKey("TextureQualityPreference"))
        {
            textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
        }        
        else
        {
            textureDropdown.value = 0;
        }        
        
        if (PlayerPrefs.HasKey("AntiAliasingPreference"))
        {
            aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
        }        
        else
        {
            aaDropdown.value = 1;
        }        
        
        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        }        
        else
        {
            Screen.fullScreen = true;
        }        

    }
    
    private void ExitOptions()
    {
        if(_hasSaved)
            gameObject.SetActive(false);
        else
        {
            LoadSettings(_currentResolutionIndex);
            Debug.Log("Save your settings first!");
        }    
    }
}
