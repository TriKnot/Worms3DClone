using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float radius = 10;
    private void OnCollisionEnter(Collision collision)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out PlayerCharacter player))
            {
                player.Damage(damage);
            }
        }
        
        Destroy(gameObject);
    }
}
