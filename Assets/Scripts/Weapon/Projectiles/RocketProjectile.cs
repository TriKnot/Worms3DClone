using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private float explosionForce = 10;
    [SerializeField] private float explosionUpwardModifier = 2;
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
        EventManager.InvokeTogglePlayerControl(false);
    }
    
    private void OnDisable()
    {
        _hasExploded = false;
        EventManager.InvokeTogglePlayerControl(true);
    }
    

    IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(.3f);
        _collider.enabled = true;
    }

    IEnumerator ReturnIfNotExploded()
    {
        yield return new WaitForSeconds(5f);
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
        
        var hits = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerCharacter player))
            {
                player.HealthSystem.Damage(damage);
                player.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardModifier);
            }
        }
        Explode();
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireSphere(transform.position, explosionRadius);
    //     
    // }

    private void Explode()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        _hasExploded = true;
        Destroy(explosion, 3f);
    }

}
