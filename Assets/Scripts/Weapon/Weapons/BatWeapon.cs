using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWeapon : MonoBehaviour, IWeapon, IChargeableWeapon, IMeleeWeapon
{
    
    private CapsuleCollider _collider;
    private Animator _animator;
    private bool _isCharging = false;
    private float swingCharge = 0f;
    [SerializeField] private float maxCharge = 1f;

    private bool _canAttack = true;
    
    private List<Collider> _hitColliders = new List<Collider>();
    
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _collider.enabled = false;
        _animator = GetComponent<Animator>();
        EventManager.OnTurnChanged += OnTurnChanged;
    }
    
    private void Update()
    {
        if (_isCharging)
        {
            ChargeWeaponUp();
        }
    }

    public void Shoot()
    {
        _animator.SetTrigger("Attack");
        _collider.enabled = true;
        _isCharging = false;
        StartCoroutine(ResetSwing());
    }

    IEnumerator ResetSwing()
    {
        _canAttack = false;
        yield return new WaitForSeconds(2f);
        _canAttack = true;
        swingCharge = 0f;
        EventManager.InvokeChargeChanged(maxCharge, swingCharge);
        _hitColliders = new List<Collider>();
    }

    public GameObject GetWeaponObject()
    {
        return gameObject;
    }

    public void SetCollider(bool state)
    {
        _collider.enabled = state;
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

    public void ChargeShot(bool active)
    {
        _isCharging = active;
        _animator.SetBool("Charge", active);
    }

    private void ChargeWeaponUp()
    {
        swingCharge = Mathf.Min(swingCharge + Time.deltaTime, maxCharge);
        EventManager.InvokeChargeChanged(maxCharge, swingCharge );
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerCharacter player))
        {   
            if(_hitColliders.Contains(other)) return;
            _hitColliders.Add(other);
            var direction = Vector3.ClampMagnitude(player.gameObject.transform.position - transform.position, 1f);
            direction += Vector3.up;
            player.gameObject.GetComponent<Rigidbody>().AddForce(direction * 10f * swingCharge, ForceMode.Impulse);
        }
    }
    
    
}
