using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon, IMeleeWeapon
{
    
    private CapsuleCollider _collider;
    private Animator _animator;
    private float _swingCharge = 0f;
    [SerializeField] private float swingChargeModifier = 1f;
    [SerializeField] private float swingUpModifier = 1f;
    
    private List<Collider> _hitColliders = new List<Collider>();
    
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _collider.enabled = false;
        _animator = GetComponent<Animator>();
        EventManager.OnTurnChanged += OnTurnChanged;
    }
    
    public void Shoot()
    {
        Shoot(0);
    }

    public void Shoot(float charge)
    {
        _hitColliders = new List<Collider>();
        _swingCharge = charge;
        _animator.SetTrigger("Attack");
        _collider.enabled = true;
    }

    public void SetChargeAnimation(bool active)
    {
        _animator.SetBool("Charge", active);
    }
    
    public GameObject GetWeaponObject()
    {
        return gameObject;
    }

    public void SetCollider(bool state)
    {
        _collider.enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerMovement player))
        {   
            if(_hitColliders.Contains(other)) return;
            _hitColliders.Add(other);
            player.AddExplosionForce(_swingCharge * swingChargeModifier, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), swingUpModifier);
        }
    }
    
    public void OnPickup(CharacterManager player)
    {
        
    }
    
    public bool CanShoot()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).IsName("Bat_Idle");
    }
    
    
    public int GetAmmoCount()
    {
        return 0;
    }

    public void AddAmmo(int amount)
    {
        //Ignores ammo
    }

    public void OnTurnChanged()
    {
        //Don't have ammo
    }
}
