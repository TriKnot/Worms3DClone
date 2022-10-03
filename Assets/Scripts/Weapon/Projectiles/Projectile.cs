using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    private PoolOnImpact _poolOnImpact;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    
    private void Start()
    {
        _poolOnImpact = GetComponent<PoolOnImpact>();
    }
    
    private void OnEnable()
    {
        _collider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay());
        StartCoroutine(ReturnIfNotExploded());
    }

    IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(.01f);
        _collider.enabled = true;
    }

    IEnumerator ReturnIfNotExploded()
    {
        yield return new WaitForSeconds(20f);
        _rigidbody.velocity = Vector3.zero;
        _poolOnImpact.ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent( out CharacterManager player))
        {
            player.HealthSystem.Damage(damage);
        }
    }
}
