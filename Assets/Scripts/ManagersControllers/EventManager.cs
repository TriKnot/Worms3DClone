using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager 
{
    public delegate void ActiveCharacterChanged(PlayerCharacter character);
    public static event ActiveCharacterChanged OnActiveCharacterChanged;
    
    public delegate void StaminaChanged(float maxStamina,float stamina);
    public static event StaminaChanged OnStaminaChanged;
    
    public delegate void HealthChanged(int maxHealth,int health);
    public static event HealthChanged OnHealthChanged;
    
    public delegate void ChargeChanged(float maxCharge,float charge);
    public static event ChargeChanged OnChargeChanged;
    
    
    public static void InvokeActiveCharacterChanged(PlayerCharacter character)
    {
        OnActiveCharacterChanged?.Invoke(character);
    }
    
    public static void InvokeStaminaChanged(float maxStamina, float stamina)
    {
        OnStaminaChanged?.Invoke(maxStamina, stamina);
    }
    
    public static void InvokeHealthChanged(int maxHealth, int health)
    {
        OnHealthChanged?.Invoke(maxHealth, health);
    }
    
    public static void InvokeChargeChanged(float maxCharge, float charge)
    {
        OnChargeChanged?.Invoke(maxCharge, charge);
    }
    
}
