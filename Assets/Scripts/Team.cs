using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Team 
{
    public List<Character> characters = new List<Character>();
    public string teamName;
    
    private void Awake()
    {
        characters = new List<Character>();
    }
}
