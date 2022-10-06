using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CharacterManager characterManager))
        {
            characterManager.Die();
        }
    }
}
