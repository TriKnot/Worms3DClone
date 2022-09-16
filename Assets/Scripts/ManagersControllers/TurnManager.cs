using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TurnManager 
{
    public static TurnManager Instance;
    private int _teamAmount;
    public int currentTeamIndex { get; private set; }
    public int currentTurnIndex { get; private set; }


    public TurnManager(int teamAmount)
    {
        _teamAmount = teamAmount;
        currentTeamIndex = 1;
        currentTurnIndex = 1;
        Instance = this;
    }

    public void NextTurn()
    {
        currentTeamIndex++;
        currentTeamIndex %= _teamAmount;
        currentTurnIndex++;
    }
    
}
