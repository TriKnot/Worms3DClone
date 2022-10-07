public static class EventManager
{
    public delegate void ActiveCharacterChanged();
    public static event ActiveCharacterChanged OnActiveCharacterChanged;
    
    public delegate void TogglePLayerControl(bool toggle);
    public static event TogglePLayerControl OnTogglePlayerControl;

    public delegate void TurnChanged();
    public static event TurnChanged OnTurnChanged;
    
    public delegate void PlayerDied(CharacterManager characterManager);
    public static event PlayerDied OnPlayerDied;
    
    public delegate void GamePaused(bool isPaused);
    public static event GamePaused OnGamePaused;
    
    public delegate void GameEnded(Team winningTeam);
    public static event GameEnded OnGameEnded;
    
    public delegate void RestartGame();
    public static event RestartGame OnRestartGame;

    public delegate void BackToMainMenu();
    public static event BackToMainMenu OnBackToMainMenu;
    
    public delegate void QuitToDesktop();
    public static event QuitToDesktop OnQuitToDesktop;
    
    public delegate void PlayerHasFiredAShot();
    public static event PlayerHasFiredAShot OnPlayerHasFiredAShot;


    public static void InvokeActiveCharacterChanged()
    {
        OnActiveCharacterChanged?.Invoke();
    }
    
    public static void InvokeTogglePlayerControl(bool toggle)
    {
        OnTogglePlayerControl?.Invoke(toggle);
    }

    public static void InvokeTurnChanged()
    {
        OnTurnChanged?.Invoke();
    }

    public static void InvokePlayerDied(CharacterManager characterManager)
    {
        OnPlayerDied?.Invoke(characterManager);
    }

    public static void InvokeGamePaused(bool isPaused)
    {
        OnGamePaused?.Invoke(isPaused);
    }
    
    public static void InvokeGameEnded(Team winningTeam)
    {
        OnGameEnded?.Invoke(winningTeam);
    }
    
    public static void InvokeRestartGame()
    {
        OnRestartGame?.Invoke();
    }
    
    public static void InvokeBackToMainMenu()
    {
        OnBackToMainMenu?.Invoke();
    }
    
    public static void InvokeQuitToDesktop()
    {
        OnQuitToDesktop?.Invoke();
    }
    
    public static void InvokePlayerHasFiredAShot()
    {
        OnPlayerHasFiredAShot?.Invoke();
    }
}
