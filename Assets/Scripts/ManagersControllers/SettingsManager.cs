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
    
    //Controls Settings
    
    //Other Settings


}
