public static class EventManager
{
    public delegate void ActiveCharacterChanged();
    public static event ActiveCharacterChanged OnActiveCharacterChanged;
    
    public delegate void TogglePLayerControl(bool toggle);
    public static event TogglePLayerControl OnTogglePlayerControl;

    public delegate void TurnChanged();
    public static event TurnChanged OnTurnChanged;

    public delegate void AmmoChanged(int ammoLeft);
    public static event AmmoChanged OnAmmoChanged;
    
    public delegate void PlayerDied(CharacterManager characterManager);
    public static event PlayerDied OnPlayerDied;
    
    public delegate void GamePaused(bool isPaused);
    public static event GamePaused OnGamePaused;


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

    public static void InvokeAmmoChanged(int ammoLeft)
    {
        OnAmmoChanged?.Invoke(ammoLeft);
    }
    
    public static void InvokePlayerDied(CharacterManager characterManager)
    {
        OnPlayerDied?.Invoke(characterManager);
    }

    public static void InvokeGamePaused(bool isPaused)
    {
        OnGamePaused?.Invoke(isPaused);
    }
}
