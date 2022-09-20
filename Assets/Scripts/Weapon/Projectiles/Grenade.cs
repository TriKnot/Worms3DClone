using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float radius = 10;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;
    
    [SerializeField] GameObject explosionPrefab;
    private CapsuleCollider _collider;
    private bool _hasExploded;
    
    private PoolOnImpact _poolOnImpact;


    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _hasExploded = false;
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
        yield return new WaitForSeconds(1f);
        _collider.enabled = true;
    }

    IEnumerator ReturnIfNotExploded()
    {
        yield return new WaitForSeconds(2f);
        _rigidbody.velocity = Vector3.zero;
        _poolOnImpact.ReturnToPool();
    }

    private void LateUpdate()
    {
        var velocity = _rigidbody.velocity;
        if (velocity != Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(-velocity, transform.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (_hasExploded) return;
        
        var hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerCharacter player))
            {
                player.Damage(damage);
            }
        }
        Explode();
    }

    private void Explode()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        _hasExploded = true;
        Destroy(explosion, 3f);
    }

    private void OnDisable()
    {
        _hasExploded = false;
    }
}
