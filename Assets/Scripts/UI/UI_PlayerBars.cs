using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerBars : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private Image staminaBarSprite;
    [SerializeField] private GameObject staminaBar;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private Color maxHealthColor;
    [SerializeField] private Color minHealthColor;
    private float _targetHealth;
    private float _targetStamina;
    private Camera _mainCamera;
    private Transform _barCanvasTransform;

    private void Start()
    {
        _mainCamera = Camera.main;
        _barCanvasTransform = gameObject.GetComponentInChildren<Canvas>().transform;
    }

    private void LateUpdate()
    {        
        var mainCameraPosition = _mainCamera.transform.position;

        //Update canvas rotation
        _barCanvasTransform.rotation = Quaternion.LookRotation(_barCanvasTransform.position - mainCameraPosition);
        
        //Update HealthBar
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, _targetHealth, Time.deltaTime * lerpSpeed);
        healthBarSprite.color = Color.Lerp(minHealthColor, maxHealthColor, healthBarSprite.fillAmount);
        
        //Update StaminaBar rotation
        staminaBar.SetActive(!(staminaBarSprite.fillAmount >= 1));
        staminaBarSprite.fillAmount = _targetStamina;
        staminaBarSprite.color = Color.Lerp(minHealthColor, maxHealthColor, staminaBarSprite.fillAmount);

    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _targetHealth = currentHealth / maxHealth;
    }    
    
    public void UpdateStaminaBar(float maxStamina, float currentStamina)
    {
        _targetStamina = 1 - (currentStamina / maxStamina);
    }
}
