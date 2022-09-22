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
    
    PlayerCharacter _playerCharacter;
    
    public HealthSystem(int healthMax, PlayerCharacter player)
    {
        _maxHealth = healthMax;
        _health = healthMax;
        _playerCharacter = player;
    }
    
    public float GetHealthPercent()
    {
        return (float)_health / _maxHealth;
    }
       
    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        _health = _health < 0 ? 0 : _health;
        if(_health == 0)
        {
            _playerCharacter.Die();
        }
        OnHealthChanged?.Invoke();
    }
    
    public void Heal(int healAmount)
    {
        _health += healAmount;
        _health = _health > _maxHealth ? _maxHealth : _health;
        OnHealthChanged?.Invoke();
    }
}
