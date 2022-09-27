using System;
using System.Net.Mime;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int currentTurnIndex { get; private set; }
    
    public float turnTime { get; private set; }
    [SerializeField] private float _turnTimeMax = 60f;
    [SerializeField] private TextMeshProUGUI _turnTimerText;

    private void Awake()
    {
        currentTurnIndex = 1;
        Instance = this;
        turnTime = _turnTimeMax;
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            turnTime -= Time.deltaTime;
            TurnTimerOnGUI();
            if(turnTime <= 0)
            {
                ChangeTurn();
            }
        }
    }
    
    public void EndTurn()
    {
        ChangeTurn();
    }
    
    private void ChangeTurn()
    {
        currentTurnIndex++;
        turnTime = _turnTimeMax;
        EventManager.InvokeTurnChanged();
        print("Turn changed to " + currentTurnIndex);
        print("Team nummer is: " + GameManager.Instance.CurrentTeamIndex );
    }

    private void TurnTimerOnGUI()
    {
        _turnTimerText.SetText(turnTime.ToString("F0"));
    }

    
}
