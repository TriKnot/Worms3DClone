using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    [HideInInspector] public int characterNumber;
    private int _health;
    [SerializeField] private int _maxHealth = 5;
    private UI_PlayerBars _playerBars;

    [SerializeField] private Transform _weaponHolder;
    
    public Inventory Inventory { get; private set; }

    private void Awake()
    {
        Inventory = new Inventory();
        Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
    }

    private void Start()
    {
        _playerBars = GetComponent<UI_PlayerBars>();
        _health = _maxHealth;
        _playerBars.UpdateHealthBar(_maxHealth, _health);
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if(_health <= 0)
        {
            _health = 0;
            Die();
        } 
        _playerBars.UpdateHealthBar(_maxHealth, _health);
    }
    
    private void Die()
    {
        GameManager.Instance.PlayerDied(this);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IWeapon weapon))
        {
            Inventory.AddWeapon(collision.gameObject);
            var weaponTransform = weapon.GetWeaponObject().transform;
            weaponTransform.SetParent(_weaponHolder);
            weaponTransform.localPosition = new Vector3(0,0,0);
            weaponTransform.localRotation = Quaternion.Euler(Vector3.zero);
            weaponTransform.localScale = Vector3.one;
        }
    }
}
