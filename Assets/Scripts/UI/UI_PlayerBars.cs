using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerBars : MonoBehaviour
{
    [Header("HealthBar")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarLerp;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private Color maxHealthColor;
    [SerializeField] private Color minHealthColor;
    private float _targetHealth;
    
    [Header("StaminaBar")]
    [SerializeField] private Image staminaBarSprite;
    [SerializeField] private GameObject staminaBar;
    private float _targetStamina;
    
    [Header("ChargeBar")]
    [SerializeField] private Image chargeBarSprite;
    [SerializeField] private GameObject chargeBar;
    private float _targetCharge;
    
    [Header("Ammo")]
    [SerializeField] private Transform ammoPane;
    private List<Image> _ammoList = new List<Image>();
    
    private Camera _mainCamera;
    private Transform _barCanvasTransform;
    private PlayerCharacter _playerCharacter;


    private void Awake()
    {
        _playerCharacter = gameObject.GetComponent<PlayerCharacter>();
        _playerCharacter.OnStaminaChanged += UpdateStaminaBar;
        _playerCharacter.OnHealthChanged += UpdateHealthBar;
        EventManager.OnChargeChanged += UpdateChargeBar;
        EventManager.OnAmmoChanged += UpdateAmmo;
        _mainCamera = Camera.main;
        _barCanvasTransform = gameObject.GetComponentInChildren<Canvas>().transform;
        chargeBar.SetActive(false);
        foreach (Transform child in ammoPane)
        {
            _ammoList.Add(child.gameObject.GetComponent<Image>());
        }

    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
        _playerCharacter.OnStaminaChanged -= UpdateStaminaBar;
        _playerCharacter.OnHealthChanged -= UpdateHealthBar;
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
        _barCanvasTransform.rotation = Quaternion.LookRotation(_barCanvasTransform.position - mainCameraPosition);
    }

    
    
    public void UpdateHealthBar(int health, int maxHealth)
    {
        _targetHealth = (float)health / (float)maxHealth;
    }

    private void AnimateHealthBar()
    {
        //Update HealthBar
        healthBarLerp.fillAmount = Mathf.MoveTowards(healthBarLerp.fillAmount, _targetHealth, Time.deltaTime * lerpSpeed);
        healthBarFill.fillAmount = _targetHealth;
        healthBarFill.color = Color.Lerp(minHealthColor, maxHealthColor, healthBarFill.fillAmount);
    }
    
    public void UpdateStaminaBar(float maxStamina, float currentStamina)
    {
        _targetStamina = currentStamina / maxStamina;
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
