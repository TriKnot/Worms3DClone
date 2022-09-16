using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Destroyable : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Bullet")) 
        {
            return;
        }
        
        print($"OnCollisionEnter Destroyable{gameObject.name} X {collision.gameObject.name}");
        
        StartCoroutine(Destroy());
    }
    
    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
