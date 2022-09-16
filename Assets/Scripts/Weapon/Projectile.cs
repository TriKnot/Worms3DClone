using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            collision.gameObject.GetComponent<PlayerCharacter>().Damage(damage);
        }
        
        Destroy(gameObject);
    }
}
