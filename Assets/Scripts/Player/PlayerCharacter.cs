using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Team Team;
    [HideInInspector] public int characterNumber;

    [SerializeField] private Transform weaponHolder;
    
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private GameObject[] characters;

    public int Health { get; private set; }
    [SerializeField] private int maxHealth = 5;

    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

    public float Stamina { get; private set; }
    [SerializeField] private int maxStamina = 20;

    public float MaxStamina
    {
        get
        {
            return maxStamina;
        }
    }

    private PlayerMovement _playerMovement;

    private bool _isActiveCharacter;

    public delegate void HealthChanged(int health, int maxHealth);
    public event HealthChanged OnHealthChanged;
    public delegate void StaminaChanged(float stamina, float maxStamina);
    public event StaminaChanged OnStaminaChanged;
    
    private void Awake()
    {
        Inventory = new Inventory();
        gameObject.GetComponent<MeshFilter>().mesh = characters[Random.Range(0, characters.Length)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = characters[Random.Range(0, characters.Length)].GetComponent<MeshRenderer>().sharedMaterial;
        _playerMovement = GetComponent<PlayerMovement>();
        EventManager.OnActiveCharacterChanged += SetActiveCharacter;
    }

    private void Start()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;
        OnHealthChanged?.Invoke(Health, MaxHealth);
        Inventory.AddWeapon(weaponHolder.GetChild(0).gameObject);
        OnStaminaChanged?.Invoke(Stamina, MaxStamina);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Inventory.ChangeWeapon();
        }
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
            OnStaminaChanged?.Invoke(Stamina, MaxStamina);
            weaponHolder.gameObject.SetActive(true);
        }
        else
        {
            _playerMovement.OnMoved -= UpdateStamina;
            weaponHolder.gameObject.SetActive(false);
        }
    }

    public void Damage(int damageAmount)
    {
        Health -= damageAmount;
        if(Health <= 0)
        {
            Health = 0;
            Die();
        } 
        OnHealthChanged?.Invoke(Health, MaxHealth);
    }

    private void UpdateStamina(float value)
    {
        if(!_isActiveCharacter) return;
        Stamina = Mathf.Max(Stamina - value, 0);
        OnStaminaChanged?.Invoke(Stamina, MaxStamina);
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
            weaponTransform.SetParent(weaponHolder);
            weaponTransform.localPosition = new Vector3(0,0,0);
            weaponTransform.localRotation = weaponTransform.rotation;
            weapon.SetCollider(false);
        }
    }
}
