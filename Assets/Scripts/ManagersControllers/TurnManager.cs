using System;
using System.Net.Mime;
using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private int _teamAmount;
    public int currentTeamIndex { get; private set; }
    public int currentTurnIndex { get; private set; }
    
    public float turnTime { get; private set; }
    [SerializeField] private float _turnTimeMax = 60f;
    [SerializeField] private TextMeshProUGUI _turnTimerText;

    private void Awake()
    {
        currentTeamIndex = 0;
        currentTurnIndex = 1;
        Instance = this;
        EventManager.OnTurnChanged += OnTurnChanged;
    }

    public void Init(int teamAmount)
    {
        _teamAmount = teamAmount;
    }

    public void OnTurnChanged()
    {
        currentTeamIndex++;
        currentTeamIndex %= _teamAmount;
        currentTurnIndex++;
        turnTime = _turnTimeMax;
        EventManager.InvokeActiveCharacterChanged();
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            turnTime -= Time.deltaTime;
            TurnTimerOnGUI();
            if(turnTime <= 0)
            {
                EventManager.InvokeTurnChanged();
            }
        }
    }

    
    private void TurnTimerOnGUI()
    {
        _turnTimerText.SetText(turnTime.ToString("F0"));
    }

    
}
