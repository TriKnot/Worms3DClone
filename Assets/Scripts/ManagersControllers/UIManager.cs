using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Element Prefabs (Drag & Drop)")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject turnTimerPrefab;
    [SerializeField] private GameObject gameOverOverlayPrefab;
    
    private GameObject _canvas;
    private GameObject _pauseMenu;
    private PauseMenuController _pauseMenuController;
    private TextMeshProUGUI _turnTimer;

    private float _turnTime;

    private void Start()
    {
        //Instantiate the canvas and UI elements
        _canvas = Instantiate(canvasPrefab);
        _turnTimer = Instantiate(turnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
        
        //Instantiate the pause menu
        _pauseMenu = Instantiate(pauseMenuPrefab, _canvas.transform);
        _pauseMenuController = _pauseMenu.GetComponent<PauseMenuController>();
        _pauseMenu.SetActive(false);
        
        //Instantiate the GameOverOverlay
        Instantiate(gameOverOverlayPrefab, _canvas.transform);


        //Subscribe to events
        EventManager.OnGamePaused += OnGamePaused;
    }

    private void OnDisable()
    {
        EventManager.OnGamePaused -= OnGamePaused;
    }

    private void OnGamePaused(bool isPaused)
    {
        _pauseMenuController.OnGamePaused(isPaused);
        _pauseMenu.SetActive(isPaused);
        if(isPaused)
        {
            _turnTimer.SetText("Paused: " + _turnTime.ToString("F0"));
        }
    }
    
    public void TurnTimerOnGUI(float turnTime)
    {
        _turnTime = turnTime;
        _turnTimer.SetText(_turnTime.ToString("F0"));
    }
    
}
