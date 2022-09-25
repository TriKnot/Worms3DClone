using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    [Header("External References/Objects")]
    public Team Team;
    [HideInInspector] public int characterNumber;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject statusBarsPrefab;
    private UI_PlayerStatusBars _statusStatusBars;
    public Inventory Inventory { get; private set; }
    [SerializeField] private GameObject[] characterModels;
    private PlayerMovement _playerMovement;

    [Header("Character Stats")]
    private int _maxHealth = 5;
    private float _maxStamina = 20;
    public HealthSystem HealthSystem { get; private set; }
    public StaminaSystem StaminaSystem { get; private set; }
    private readonly float _maxWeaponCharge = 1;
    private float _weaponCharge = 0;
    private bool _isChargingWeapon = false;
    public delegate void WeaponChargeChanged(float maxCharge, float currentCharge);
    public event WeaponChargeChanged OnWeaponChargeChanged;

   private void Awake()
    {
        Inventory = new Inventory();
        HealthSystem = new HealthSystem(_maxHealth, this);
        StaminaSystem = new StaminaSystem(_maxStamina);
        gameObject.GetComponent<MeshFilter>().mesh = characterModels[Random.Range(0, characterModels.Length)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = characterModels[Random.Range(0, characterModels.Length)].GetComponent<MeshRenderer>().sharedMaterial;
        _playerMovement = GetComponent<PlayerMovement>();
        _statusStatusBars = Instantiate(statusBarsPrefab, transform).GetComponent<UI_PlayerStatusBars>();
        EventManager.OnActiveCharacterChanged += SetActiveCharacter;
    }

    private void Start()
    {
        Inventory.AddWeapon(weaponHolder.GetChild(0).gameObject);
        _statusStatusBars.Init(this);
    }

    private void OnDisable()
    {
        EventManager.OnActiveCharacterChanged -= SetActiveCharacter;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Inventory.ChangeWeapon();
        }
    }

    private void FixedUpdate()
    {
        if(_isChargingWeapon) ChargeWeaponUp();
    }

    public void FireWeapon(InputAction.CallbackContext context)
    {
        IWeapon weapon = Inventory.GetActiveWeapon();

        //If weapon is chargeable
        if (Inventory.GetActiveWeaponObject().TryGetComponent(out IChargeableWeapon chargeableWeapon))
        {
            if (!Inventory.GetActiveWeaponObject().TryGetComponent(out IMeleeWeapon meleeWeapon) && Inventory.GetActiveWeapon().GetAmmoCount() <= 0)
            {
                return;
            }
            if (context.started)
            {
                if(weapon.CanShoot())
                    _isChargingWeapon = true;
            }else if (context.canceled && _isChargingWeapon)
            { 
                chargeableWeapon.Shoot(_weaponCharge);
                _isChargingWeapon = false;
                _weaponCharge = 0;
                OnWeaponChargeChanged?.Invoke(_maxWeaponCharge, _weaponCharge);
            }
            chargeableWeapon.SetChargeAnimation(_isChargingWeapon);
            return;
        }
        //If weapon is not chargeable
        if(context.started )
        {
            if(weapon.CanShoot())
                weapon.Shoot();
        }
    }
    
    private void ChargeWeaponUp()
    {
        _weaponCharge = Mathf.Min(_weaponCharge + Time.fixedDeltaTime, _maxWeaponCharge);
        OnWeaponChargeChanged?.Invoke(_maxWeaponCharge, _weaponCharge);
    }


    private void SetActiveCharacter()
    {
        weaponHolder.gameObject.SetActive(GetComponent<PlayerInput>().inputIsActive);
    }

    public void Die()
    {
        EventManager.InvokePlayerDied(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.Instance.ActiveCharacter != this) return;
        if (!other.gameObject.TryGetComponent(out IWeapon weapon)) return;
        Inventory.AddWeapon(other.gameObject);
        var weaponTransform = weapon.GetWeaponObject().transform;
        weaponTransform.SetParent(weaponHolder);
        weaponTransform.localPosition = new Vector3(0,0,0);
        weaponTransform.localRotation = weaponTransform.rotation;
        weapon.SetCollider(false);
        weapon.OnPickup(this);
    }
}
