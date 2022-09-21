using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public delegate void ActiveCharacterChanged(PlayerCharacter character);

    public static event ActiveCharacterChanged OnActiveCharacterChanged;

    public delegate void ChargeChanged(float maxCharge, float swingCharge);

    public static event ChargeChanged OnChargeChanged;

    public delegate void TurnChanged();

    public static event TurnChanged OnTurnChanged;

    public delegate void AmmoChanged(int ammoLeft);

    public static event AmmoChanged OnAmmoChanged;

    public delegate void TogglePlayerControl(bool toggle);

    public static event TogglePlayerControl OnTogglePlayerControl;


    public static void InvokeActiveCharacterChanged(PlayerCharacter character)
    {
        OnActiveCharacterChanged?.Invoke(character);
    }

    public static void InvokeChargeChanged(float maxCharge, float swingCharge)
    {
        OnChargeChanged?.Invoke(maxCharge, swingCharge);
    }

    public static void InvokeTurnChanged()
    {
        OnTurnChanged?.Invoke();
    }

    public static void InvokeTogglePlayerControl(bool toggle)
    {
        OnTogglePlayerControl?.Invoke(toggle);
    }

    public static void InvokeAmmoChanged(int ammoLeft)
    {
        OnAmmoChanged?.Invoke(ammoLeft);
    }
}
