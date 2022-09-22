using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem
{
    
    public delegate void StaminaChanged();
    public event StaminaChanged OnStaminaChanged;
    
    private float _stamina;
    private float _maxStamina;
    
    public float Stamina { get { return _stamina; } }
    public float MaxStamina { get { return _maxStamina; } }
    
    public StaminaSystem(float staminaMax)
    {
        _maxStamina = staminaMax;
        _stamina = staminaMax;
    }
    
    public float GetStaminaPercent()
    {
        return _stamina / _maxStamina;
    }
       
    public void DecreaseStamina(float amount)
    {
        _stamina -= amount;
        _stamina = _stamina < 0 ? 0 : _stamina;
        OnStaminaChanged?.Invoke();
    }
    
    public void IncreaseStamina(float amount)
    {
        _stamina += amount;
        _stamina = _stamina > _maxStamina ? _maxStamina : _stamina;
        OnStaminaChanged?.Invoke();
    }
}
