using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon, IMeleeWeapon
{
    
    private CapsuleCollider _collider;
    private Animator _animator;
    private float _swingCharge = 0f;
    
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
        _swingCharge = 0f;
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
        if(other.TryGetComponent(out PlayerCharacter player))
        {   
            if(_hitColliders.Contains(other)) return;
            _hitColliders.Add(other);
            var direction = Vector3.ClampMagnitude(player.gameObject.transform.position - transform.position, 1f);
            direction += Vector3.up;
            player.gameObject.GetComponent<Rigidbody>().AddForce(direction * 10f * _swingCharge, ForceMode.Impulse);
        }
    }
    
    public void OnPickup(PlayerCharacter player)
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
