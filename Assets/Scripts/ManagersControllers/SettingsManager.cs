using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager 
{

    //Sound Settings
    public static float MusicVolume { get; set; }
    public static float SFXVolume { get; set; }
    
    //Graphic Settings
    
    //Gameplay Settings
    public static int TeamSize { get; set; }
    public static int MaxTeamSize { get; } = 5;
    public static int MinTeamSize { get; } = 1;
    public static int MaxPlayers { get; } = 4;
    public static int MinPlayers { get; } = 2;
    public static int TeamAmount { get; set; }
    public static List<string> TeamNames { get; set; }
    public static List<Color> TeamColors { get; set; }
    public static bool UseStamina { get; set; }
    public static int MinTurnTimerValue { get; } = 30;
    public static int MaxTurnTimerValue { get; } = 300;
    public static int TurnTimerValue { get; set; }
    private static bool invertMouseY = false;

    public static bool InvertMouseY
    {
        get
        {
            return invertMouseY;
        }
        set
        {
            invertMouseY = value;
            OnSettingsChanged?.Invoke();
        }
    }
    
    private static float _mouseSensitivity = 1f;

    public static float MouseSensitivity
    {
        get
        {
            return _mouseSensitivity;
        }
        set
        {
            _mouseSensitivity = Mathf.Clamp(value, 0.1f, 1f);
            OnSettingsChanged?.Invoke();
        }
    }

    //Controls Settings

    //Other Settings
    
    
    public delegate void SettingsChanged();
    public static event SettingsChanged OnSettingsChanged;

}
