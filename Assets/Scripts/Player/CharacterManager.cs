using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
    [Header("External References/Objects")]
    public Team Team;
    public int CharacterNumber { get; set; }
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject statusBarsPrefab;
    private UI_PlayerStatusBars _statusStatusBars;
    public WeaponController WeaponController { get; private set; }
    public Inventory Inventory { get; private set; }
    [SerializeField] private GameObject[] characterModels;

    [Header("Character Stats")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float maxStamina = 20;
    public HealthSystem HealthSystem { get; private set; }
    public StaminaSystem StaminaSystem { get; private set; }

    
    public Transform CameraFollow { get; private set; }

    public bool IsActiveCharacter { get; set; }


    private void Awake()
    {
        Inventory = new Inventory();
        HealthSystem = new HealthSystem(maxHealth, this);
        StaminaSystem = new StaminaSystem(maxStamina);
        WeaponController = GetComponent<WeaponController>();
        CameraFollow = transform.Find("CameraFollowTarget").transform;
    }

    public void Init()
    {
        gameObject.GetComponent<MeshFilter>().mesh = characterModels[Team.TeamNumber].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = characterModels[Team.TeamNumber].GetComponent<MeshRenderer>().sharedMaterial;
        Inventory.AddWeapon(weaponHolder.GetChild(0).gameObject);
        
        _statusStatusBars = Instantiate(statusBarsPrefab, transform).GetComponent<UI_PlayerStatusBars>();
        _statusStatusBars.Init(this);
        
        EventManager.OnActiveCharacterChanged += SetActiveCharacter;
        EventManager.OnTurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        EventManager.OnActiveCharacterChanged -= SetActiveCharacter;
        EventManager.OnTurnChanged -= OnTurnChanged;
    }
    
    public void FireWeapon(InputAction.CallbackContext context)
    {
        WeaponController.FireWeapon(context);
    }
    
    private void SetActiveCharacter()
    {
        bool active = GameManager.Instance.ActiveCharacter == this;

        weaponHolder.gameObject.SetActive(active);

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

    private void OnTurnChanged()
    {
        StaminaSystem.IncreaseStamina(StaminaSystem.MaxStamina);
    }
}
