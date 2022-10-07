using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Team 
{
    public List<CharacterManager> PlayerCharacters { get; private set; }
    public string TeamName { get; private set; }
    public int TeamNumber { get; private set; }
    public Color TeamColor {get; private set; }
    
    public Team(string teamName, int teamNumber, Color teamColor)
    {
        TeamName = teamName;
        TeamNumber = teamNumber;
        TeamColor = teamColor;
        PlayerCharacters = new List<CharacterManager>();
    }
    
    public void AddCharacter(CharacterManager character)
    {
        PlayerCharacters.Add(character);
    }

}
