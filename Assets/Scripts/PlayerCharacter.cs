using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    private int _health;
    [SerializeField] private int _maxHealth = 5;
    private UI_HealthBar _healthBar;
    private void Start()
    {
        _healthBar = GetComponent<UI_HealthBar>();
        _health = _maxHealth;
        _healthBar.UpdateHealthBar(_maxHealth, _health);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Damage(1);
        }
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
        Debug.Log("Player died");
    }
}
