using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EventManager
{
    public delegate void ActiveCharacterChanged();
    public static event ActiveCharacterChanged OnActiveCharacterChanged;
    
    public delegate void TogglePLayerControl(bool toggle);
    public static event TogglePLayerControl OnTogglePlayerControl;

    public delegate void TurnChanged();
    public static event TurnChanged OnTurnChanged;

    public delegate void AmmoChanged(int ammoLeft);
    public static event AmmoChanged OnAmmoChanged;
    
    public delegate void PlayerDied(PlayerCharacter character);
    public static event PlayerDied OnPlayerDied;


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
    
    public static void InvokePlayerDied(PlayerCharacter character)
    {
        OnPlayerDied?.Invoke(character);
    }
}