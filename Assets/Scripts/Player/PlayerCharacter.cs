using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    public Team Team;
    [HideInInspector] public int characterNumber;

    [SerializeField] private Transform weaponHolder;
    
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private GameObject[] characters;

    private int _maxHealth = 5;
    private float _maxStamina = 20;
    public HealthSystem HealthSystem { get; private set; }
    public StaminaSystem StaminaSystem { get; private set; }
    
    private PlayerMovement _playerMovement;

    private bool _isActiveCharacter;

    [SerializeField] private GameObject _statusBarsPrefab;

    private UI_PlayerStatusBars _statusStatusBars;

   private void Awake()
    {
        Inventory = new Inventory();
        HealthSystem = new HealthSystem(_maxHealth, this);
        StaminaSystem = new StaminaSystem(_maxStamina);
        gameObject.GetComponent<MeshFilter>().mesh = characters[Random.Range(0, characters.Length)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = characters[Random.Range(0, characters.Length)].GetComponent<MeshRenderer>().sharedMaterial;
        _playerMovement = GetComponent<PlayerMovement>();
        _statusStatusBars = Instantiate(_statusBarsPrefab, transform).GetComponent<UI_PlayerStatusBars>();
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

    public void SetActiveCharacter(PlayerCharacter character)
    {
        _isActiveCharacter = character == this;
        if (_isActiveCharacter)
        {
            weaponHolder.gameObject.SetActive(true);
        }
        else
        {
            weaponHolder.gameObject.SetActive(false);
        }
    }

    public void Die()
    {
        EventManager.InvokePlayerDied(this);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.Instance.ActivePlayerCharacter != this) return;
        if (other.gameObject.TryGetComponent(out IWeapon weapon))
        {
            Inventory.AddWeapon(other.gameObject);
            var weaponTransform = weapon.GetWeaponObject().transform;
            weaponTransform.SetParent(weaponHolder);
            weaponTransform.localPosition = new Vector3(0,0,0);
            weaponTransform.localRotation = weaponTransform.rotation;
            weapon.SetCollider(false);
        }
    }
}
