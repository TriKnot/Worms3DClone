using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class HealthSystem
{
    
    public delegate void HealthChanged();
    public event HealthChanged OnHealthChanged;
    
    private int _health;
    private int _maxHealth;
    
    public int Health { get { return _health; } }
    public int MaxHealth { get { return _maxHealth; } }
    
    public HealthSystem(int healthMax)
    {
        _maxHealth = healthMax;
        _health = healthMax;
    }
    
    public float GetHealthPercent()
    {
        return (float)_health / _maxHealth;
    }
       
    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        Debug.Log("health after damage: " + _health);
        _health = _health < 0 ? 0 : _health;
        OnHealthChanged?.Invoke();
    }
    
    public void Heal(int healAmount)
    {
        _health += healAmount;
        _health = _health > _maxHealth ? _maxHealth : _health;
        OnHealthChanged?.Invoke();
    }
}
