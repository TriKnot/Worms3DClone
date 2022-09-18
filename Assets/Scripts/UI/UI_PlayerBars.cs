using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerBars : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image healthBarLerp;
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
        healthBarLerp.fillAmount = Mathf.MoveTowards(healthBarLerp.fillAmount, _targetHealth, Time.deltaTime * lerpSpeed);
        healthBarFill.fillAmount = _targetHealth;
        healthBarFill.color = Color.Lerp(minHealthColor, maxHealthColor, healthBarFill.fillAmount);

        if (GameManager.Instance.ActivePlayerCharacter.gameObject != gameObject) return;
        //Update StaminaBar 
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
