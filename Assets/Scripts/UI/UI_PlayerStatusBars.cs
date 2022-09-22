using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStatusBars : MonoBehaviour
{
    [Header("HealthBar")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarLerp;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private Color maxHealthColor;
    [SerializeField] private Color minHealthColor;
    private float _targetHealth = 1;
    
    [Header("StaminaBar")]
    [SerializeField] private Image staminaBarSprite;
    [SerializeField] private GameObject staminaBar;
    private float _targetStamina = 1;
    
    [Header("ChargeBar")]
    [SerializeField] private Image chargeBarSprite;
    [SerializeField] private GameObject chargeBar;
    private float _targetCharge = 0;
    
    [Header("Ammo")]
    [SerializeField] private Transform ammoPane;
    private List<Image> _ammoList = new List<Image>();
    
    private Camera _mainCamera;
    private Transform _barCanvasTransform;
    private PlayerCharacter _playerCharacter;
    private HealthSystem _healthSystem;
    private StaminaSystem _staminaSystem;



    public void Init(PlayerCharacter parent)
    {
        //Setup references
        _playerCharacter = parent;
        _healthSystem = _playerCharacter.HealthSystem;
        _staminaSystem = _playerCharacter.StaminaSystem;
        _mainCamera = Camera.main;
        _barCanvasTransform = gameObject.GetComponentInChildren<Canvas>().transform;
        //Subscribe to events
        _healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        _staminaSystem.OnStaminaChanged += StaminaSystem_OnStaminaChanged;
        
        EventManager.OnChargeChanged += UpdateChargeBar;
        EventManager.OnAmmoChanged += UpdateAmmo;
        
        foreach (Transform child in ammoPane)
        {
            _ammoList.Add(child.gameObject.GetComponent<Image>());
        }

    }
    private void OnDestroy()
    {
        //Unsubscribe from events
        _playerCharacter.HealthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        _playerCharacter.StaminaSystem.OnStaminaChanged -= StaminaSystem_OnStaminaChanged;
        EventManager.OnChargeChanged -= UpdateChargeBar;
        EventManager.OnAmmoChanged -= UpdateAmmo;
    }

    private void LateUpdate()
    {
        RotateCanvas();
        AnimateHealthBar();
        AnimateStaminaBar();
        AnimateChargeBar();
    }

    /// <summary>
    /// Rotate canvas towards the Camera
    /// </summary>
    private void RotateCanvas()
    {
         var mainCameraPosition = _mainCamera.transform.position;
        _barCanvasTransform.LookAt(mainCameraPosition);
    }

    
    
    public void HealthSystem_OnHealthChanged()
    {
        _targetHealth = _healthSystem.GetHealthPercent();
    }

    private void AnimateHealthBar()
    {
        //Update HealthBar
        healthBarLerp.fillAmount = Mathf.MoveTowards(healthBarLerp.fillAmount, _targetHealth, Time.deltaTime * lerpSpeed);
        healthBarFill.fillAmount = _targetHealth;
        healthBarFill.color = Color.Lerp(minHealthColor, maxHealthColor, healthBarFill.fillAmount);
    }

    public void StaminaSystem_OnStaminaChanged()
    {
        _targetStamina = _staminaSystem.GetStaminaPercent();
    }

    private void AnimateStaminaBar()
    {
        //Update StaminaBar 
        staminaBar.SetActive(!(staminaBarSprite.fillAmount >= 1));
        staminaBarSprite.fillAmount = _targetStamina;
        staminaBarSprite.color = Color.Lerp(minHealthColor, maxHealthColor, staminaBarSprite.fillAmount);
    }
    
    private void UpdateChargeBar(float maxCharge, float currentCharge)
    {
        if(GameManager.Instance.ActivePlayerCharacter != _playerCharacter) return;
        _targetCharge = currentCharge / maxCharge;
    }
    
    private void AnimateChargeBar()
    {
        //Update ChargeBar
        chargeBar.SetActive((chargeBarSprite.fillAmount > 0));
        chargeBarSprite.fillAmount = _targetCharge;
        chargeBarSprite.color = Color.Lerp(minHealthColor, maxHealthColor, chargeBarSprite.fillAmount);
    }
    
    public void UpdateAmmo(int ammo)
    {
        if(GameManager.Instance.ActivePlayerCharacter != _playerCharacter) return;
        if(ammo > 0) ammoPane.gameObject.SetActive(true);
        for (int i = 0; i < _ammoList.Count; i++)
        {
            _ammoList[i].enabled = (i < ammo);
        }
    }
}
