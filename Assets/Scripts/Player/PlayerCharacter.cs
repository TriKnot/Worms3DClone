using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    public int characterNumber;
    private int _health;
    [SerializeField] private int _maxHealth = 5;
    private UI_HealthBar _healthBar;
    private void Start()
    {
        _healthBar = GetComponent<UI_HealthBar>();
        _health = _maxHealth;
        _healthBar.UpdateHealthBar(_maxHealth, _health);
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if(_health <= 0)
        {
            _health = 0;
            Die();
        } 
        _healthBar.UpdateHealthBar(_maxHealth, _health);
    }
    
    private void Die()
    {
        GameManager.Instance.PlayerDied(this);
        Destroy(gameObject);
    }
    
}
