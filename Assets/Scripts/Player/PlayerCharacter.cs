using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    [HideInInspector] public int characterNumber;

    [SerializeField] private Transform _weaponHolder;
    
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private GameObject[] _characters;

    [Header("Health")]
    private int _health;
    [SerializeField] private int _maxHealth = 5;

    [Header("Stamina")] 
    private float _stamina;
    [SerializeField] private float _maxStamina = 20f;
    private PlayerMovement _playerMovement;

    private bool _isActiveCharacter;
    
    private void Awake()
    {
        Inventory = new Inventory();
        gameObject.GetComponent<MeshFilter>().mesh = _characters[Random.Range(0, _characters.Length)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = _characters[Random.Range(0, _characters.Length)].GetComponent<MeshRenderer>().sharedMaterial;
        _playerMovement = GetComponent<PlayerMovement>();
        EventManager.OnActiveCharacterChanged += SetActiveCharacter;
    }

    private void Start()
    {
        _health = _maxHealth;
        _stamina = _maxStamina;
        EventManager.InvokeHealthChanged(_maxHealth, _health);
        Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
        EventManager.InvokeStaminaChanged(_maxStamina, _stamina);
    }

    private void OnDestroy()
    {
        _playerMovement.OnMoved -= UpdateStamina;
    }
    
    public void SetActiveCharacter(PlayerCharacter character)
    {
        _isActiveCharacter = character == this;
        if (_isActiveCharacter)
        {
            _playerMovement.OnMoved += UpdateStamina;
            EventManager.InvokeStaminaChanged(_maxStamina, _stamina);
            _weaponHolder.gameObject.SetActive(true);
        }
        else
        {
            _playerMovement.OnMoved -= UpdateStamina;
            _weaponHolder.gameObject.SetActive(false);
        }
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if(_health <= 0)
        {
            _health = 0;
            Die();
        } 
        EventManager.InvokeHealthChanged(_maxHealth, _health);
    }

    private void UpdateStamina(float value)
    {
        if(!_isActiveCharacter) return;
        _stamina = Mathf.Max(_stamina - value, 0);
        EventManager.InvokeStaminaChanged(_maxStamina, _stamina);
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
