using UnityEngine;

public interface IWeapon
{
    
    
    void Shoot();

    GameObject GetWeaponObject();
    
    void SetCollider(bool state);

    int GetAmmoCount();
    

    void AddAmmo(int amount);

    void OnTurnChanged();

}
