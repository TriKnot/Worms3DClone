using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private int CurrentTurnIndex { get; set; }
    
    private float TurnTime { get; set; }
    private float turnTimeMax = 60f;

    private void Awake()
    {
        CurrentTurnIndex = 1;
        turnTimeMax = SettingsManager.TurnTimerValue;
        TurnTime = turnTimeMax;
    }
    
    private void Update()
    {
        if (!GameManager.Instance.IsPaused)
        {
            TurnTime -= Time.deltaTime;
            UpdateTurnTimeOnGUI();
            if(TurnTime <= 0)
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
        CurrentTurnIndex++;
        TurnTime = turnTimeMax;
        EventManager.InvokeTurnChanged();
    }

    private void UpdateTurnTimeOnGUI()
    {
        GameManager.Instance.UIManager.TurnTimerOnGUI(TurnTime);
    }

    
}
