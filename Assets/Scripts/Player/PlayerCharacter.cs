using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public Team team;
    [HideInInspector] public int characterNumber;
    private int _health;
    [SerializeField] private int _maxHealth = 5;
    private UI_PlayerBars _playerBars;

    [SerializeField] private Transform _weaponHolder;
    
    public Inventory Inventory { get; private set; }
    
    [SerializeField] private GameObject[] _characters;

    private void Awake()
    {
        Inventory = new Inventory();
        //Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
        gameObject.GetComponent<MeshFilter>().mesh = _characters[Random.Range(0, _characters.Length-1)].GetComponent<MeshFilter>().sharedMesh;
        gameObject.GetComponent<MeshRenderer>().material = _characters[Random.Range(0, _characters.Length-1)].GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void Start()
    {
        _playerBars = GetComponent<UI_PlayerBars>();
        _health = _maxHealth;
        _playerBars.UpdateHealthBar(_maxHealth, _health);
        Inventory.AddWeapon(_weaponHolder.GetChild(0).gameObject);
    }

    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        if(_health <= 0)
        {
            _health = 0;
            Die();
        } 
        _playerBars.UpdateHealthBar(_maxHealth, _health);
    }
    
    private void Die()
    {
        GameManager.Instance.PlayerDied(this);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
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
