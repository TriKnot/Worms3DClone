using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CharacterManager : MonoBehaviour
{
    [Header("External References/Objects")]
    public Team Team;
    [HideInInspector] public int characterNumber;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private GameObject statusBarsPrefab;
    private UI_PlayerStatusBars _statusStatusBars;
    public WeaponController WeaponController { get; private set; }
    public Inventory Inventory { get; private set; }
    [SerializeField] private GameObject[] characterModels;

    [Header("Character Stats")]
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private float _maxStamina = 20;
    public HealthSystem HealthSystem { get; private set; }
    public StaminaSystem StaminaSystem { get; private set; }

    
    public Transform CameraFollow { get; private set; }

    public bool IsActiveCharacter { get; set; }

    private void Awake()
    {
        Inventory = new Inventory();
        HealthSystem = new HealthSystem(_maxHealth, this);
        StaminaSystem = new StaminaSystem(_maxStamina);
        gameObject.GetComponent<MeshFilter>().mesh = characterModels[Random.Range(0, characterModels.Length)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = characterModels[Random.Range(0, characterModels.Length)].GetComponent<MeshRenderer>().sharedMaterial;
        _statusStatusBars = Instantiate(statusBarsPrefab, transform).GetComponent<UI_PlayerStatusBars>();
        CameraFollow = transform.Find("CameraFollowTarget").transform;
        WeaponController = GetComponent<WeaponController>();
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
}
