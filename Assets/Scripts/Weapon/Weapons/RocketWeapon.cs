using UnityEngine;

public class RocketWeapon : MonoBehaviour, IWeapon, IChargeableWeapon
{
    private ProjectilePool _projectilePool;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject rocketPrefab;
    private float projectileMass;

    [SerializeField] private float shotSpeedMultiplier = 3f;
    private CapsuleCollider _collider;
    [SerializeField] private GameObject _shootEffect;

    private int _ammo ;
    [SerializeField] private int maxAmmo = 2;
    
    private GameObject _currentRocket;
    
    private WeaponController _weaponController;
    
    [SerializeField] private Material lineMaterial;
    private Rigidbody _rigidbody;
    private bool pickedUp = false;

    private void Awake()
    {
        _projectilePool = new ProjectilePool(rocketPrefab);
        _collider = GetComponent<CapsuleCollider>();
        projectileMass = rocketPrefab.GetComponent<Rigidbody>().mass;
        _rigidbody = GetComponent<Rigidbody>();
        _ammo = maxAmmo;
        EventManager.OnTurnChanged += OnTurnChanged;
    }

    private void OnEnable()
    {
        ToggleVisibleRocket(true);
    }
    
    private void OnDestroy()
    {
        EventManager.OnTurnChanged -= OnTurnChanged;
        if(pickedUp)
        {
            _weaponController.OnWeaponChargeChanged -= OnWeaponChargeChanged;
            pickedUp = false;
        }    
    }

    public void Shoot()
    {
        Shoot(0);
    }
    public void Shoot(float shotCharge)
    {
        _currentRocket = _projectilePool.GetBullet();
        if(_currentRocket != null)
        {
            _currentRocket.GetComponent<RocketProjectile>().Init(this);
            _currentRocket.transform.position = firePoint.transform.position;
            _currentRocket.transform.rotation = firePoint.transform.rotation;
            _currentRocket.SetActive(true);
            _currentRocket.GetComponent<Rigidbody>().AddForce(-transform.forward * shotCharge * shotSpeedMultiplier, ForceMode.Impulse);
            _shootEffect.SetActive(true);
            _ammo--;
        }
    }

    public void SetChargeAnimation(bool active)
    {
        //No charge animation (yet?)
    }

    public void ToggleVisibleRocket(bool active)
    {
        var showRocket = active && _ammo > 0;
        firePoint.SetActive(showRocket);
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
        return _ammo;
    }


    public void AddAmmo(int amount)
    {
        _ammo = _ammo + amount > maxAmmo ? maxAmmo : _ammo + amount;
    }
    
    public void OnTurnChanged()
    {
        //Uncomment for testing
        //_ammo = maxAmmo;
    }

    public void OnPickup(CharacterManager player)
    {
        pickedUp = true;
        _weaponController = player.GetComponent<WeaponController>();
        _weaponController.OnWeaponChargeChanged += OnWeaponChargeChanged;
        Destroy(_rigidbody);
        transform.localRotation = Quaternion.Euler(0,180 ,0);
    }

    private void OnWeaponChargeChanged(float maxCharge, float currentCharge)
    {
        _weaponController.ShowLineAimCurved(-transform.forward * (currentCharge * shotSpeedMultiplier * 1.15f), firePoint.transform.position, projectileMass, 0.7f, lineMaterial);
    }
    
    public bool CanShoot()
    {
        return (_ammo > 0 && (_currentRocket == null || _currentRocket.activeSelf == false));
    }
}

