using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private float lerpSpeed = 2f;
    [SerializeField] private Color maxHealthColor;
    [SerializeField] private Color minHealthColor;
    private float _targetHealth;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, _targetHealth, Time.deltaTime * lerpSpeed);
        healthBarSprite.color = Color.Lerp(minHealthColor, maxHealthColor, healthBarSprite.fillAmount);
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
            _targetHealth = currentHealth / maxHealth;
    }
}
