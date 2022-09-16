using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponChargeBar : MonoBehaviour
{
    [SerializeField] private Image chargeBarSprite;
    [SerializeField] private GameObject chargeBarCanvas;
    [SerializeField] private Color minChargeColor;
    [SerializeField] private Color maxChargeColor;
    private Transform _chargeBarTransform;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        _chargeBarTransform = chargeBarCanvas.transform;
        SetActive(false);
    }

    private void Update()
    {
        _chargeBarTransform.rotation = Quaternion.LookRotation(_chargeBarTransform.position - mainCamera.transform.position) * Quaternion.Euler(0,0,90f);
    }

    public void UpdateChargeBar(float currentCharge, float maxCharge)
    {
        chargeBarSprite.fillAmount = currentCharge / maxCharge;
        chargeBarSprite.color = Color.Lerp(minChargeColor, maxChargeColor, currentCharge / maxCharge);
    }
    
    public void SetActive(bool active)
    {
        chargeBarCanvas.SetActive(active);
    }
}