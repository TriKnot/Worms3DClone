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
    [SerializeField] private Image staminaBarLerp;
    [SerializeField] private GameObject staminaBar;
    private float _targetStamina;
    
    private Camera _mainCamera;
    private Transform _barCanvasTransform;
    private PlayerCharacter _playerCharacter;


    private void Awake()
    {
        _playerCharacter = gameObject.GetComponent<PlayerCharacter>();
        _playerCharacter.OnHealthChanged += UpdateHealthBar;
        _playerCharacter.OnStaminaChanged += UpdateStaminaBar;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _barCanvasTransform = gameObject.GetComponentInChildren<Canvas>().transform;
    }

    private void OnDestroy()
    {
        _playerCharacter.OnHealthChanged -= UpdateHealthBar;
        _playerCharacter.OnStaminaChanged -= UpdateStaminaBar;
    }

    private void LateUpdate()
    {
        RotateCanvas();
        AnimateHealthBar();
        if (GameManager.Instance.ActivePlayerCharacter.gameObject != gameObject) return;
        AnimateStaminaBar();
    }

    /// <summary>
    /// Rotate canvas towards the Camera
    /// </summary>
    private void RotateCanvas()
    {
        var mainCameraPosition = _mainCamera.transform.position;
        _barCanvasTransform.rotation = Quaternion.LookRotation(_barCanvasTransform.position - mainCameraPosition);
    }

    
    
    public void UpdateHealthBar(int maxHealth, int currentHealth)
    {
        _targetHealth = (float)currentHealth / (float)maxHealth;
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
}
