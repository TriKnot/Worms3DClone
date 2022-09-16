using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Team 
{
    public List<PlayerCharacter> characters = new List<PlayerCharacter>();
    public string teamName;
    public Color teamColor;
    
    private void Awake()
    {
        characters = new List<PlayerCharacter>();
    }
}
