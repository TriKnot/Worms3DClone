using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Element Prefabs (Drag & Drop)")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject turnTimerPrefab;
    [SerializeField] private GameObject optionsMenu;
    
    private GameObject _canvas;
    private GameObject _pauseMenu;
    private TextMeshProUGUI _turnTimer;
    public GameObject OptionsMenu { get; private set; }
    
    private float _turnTime;

    private void Awake()
    {
        //Instantiate the canvas and UI elements
        _canvas = Instantiate(canvasPrefab);
        _turnTimer = Instantiate(turnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
        
        //Instantiate the pause menu
        _pauseMenu = Instantiate(pauseMenuPrefab, _canvas.transform);
        _pauseMenu.GetComponent<UI_PausMenuButtonFunctions>().Init(this);
        _pauseMenu.SetActive(false);
        OptionsMenu = Instantiate(optionsMenu, _pauseMenu.transform);
        OptionsMenu.SetActive(false);
        
        //Subscribe to events
        EventManager.OnGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        EventManager.OnGamePaused -= OnGamePaused;
    }

    private void OnGamePaused(bool isPaused)
    {
        _pauseMenu.SetActive(isPaused);
        if(isPaused)
        {
            _turnTimer.SetText("Paused: " + _turnTime.ToString("F0"));
        }

        OptionsMenu.SetActive(false);
    }
    
    public void TurnTimerOnGUI(float turnTime)
    {
        _turnTime = turnTime;
        _turnTimer.SetText(_turnTime.ToString("F0"));
    }
    
}
