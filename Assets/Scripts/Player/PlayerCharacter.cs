using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    [HideInInspector] public int characterNumber;
    private UI_PlayerBars _playerBars;

    [SerializeField] private Transform _weaponHolder;
    
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private GameObject[] _characters;

    [Header("Health")]
    private int _health;
    [SerializeField] private int _maxHealth = 5;
    public delegate void HealthChanged(int maxHealth,int health);
    public event HealthChanged OnHealthChanged;

    [Header("Stamina")] 
    private float _stamina;
    [SerializeField] private float _maxStamina = 20f;
    private PlayerMovement _playerMovement;
    public delegate void StaminaChanged(float maxStamina,float stamina);
    public event StaminaChanged OnStaminaChanged;

    private void Awake()
    {
        Inventory = new Inventory();
        //Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
        gameObject.GetComponent<MeshFilter>().mesh = _characters[Random.Range(0, _characters.Length-1)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = _characters[Random.Range(0, _characters.Length-1)].GetComponent<MeshRenderer>().sharedMaterial;
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.OnMoved += UpdateStamina;
    }

    private void Start()
    {
        _playerBars = GetComponent<UI_PlayerBars>();
        _health = _maxHealth;
        _stamina = _maxStamina;
        OnHealthChanged?.Invoke(_maxHealth, _health);
        OnStaminaChanged?.Invoke(_maxStamina, _stamina);
        Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
    }

    private void OnDestroy()
    {
        _playerMovement.OnMoved -= UpdateStamina;
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if(_health <= 0)
        {
            _health = 0;
            Die();
        } 
        OnHealthChanged?.Invoke(_maxHealth, _health);
    }

    private void UpdateStamina(float value)
    {
        _stamina = Mathf.Max(_stamina - value, 0);
        OnStaminaChanged?.Invoke(_maxStamina, _stamina);
    }
    
    private void Die()
    {
        GameManager.Instance.PlayerDied(this);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.Instance.ActivePlayerCharacter != this) return;
        if (other.gameObject.TryGetComponent(out IWeapon weapon))
        {
            Inventory.AddWeapon(other.gameObject);
            var weaponTransform = weapon.GetWeaponObject().transform;
            weaponTransform.SetParent(_weaponHolder);
            weaponTransform.localPosition = new Vector3(0,0,0);
            weaponTransform.localRotation = weaponTransform.rotation;
            weapon.SetCollider(false);
        }
    }
}
