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
    [SerializeField] private GameObject activePlayerText;
    [SerializeField] private GameObject turnNumberText;

    private GameObject _canvas;
    private GameObject _pauseMenu;
    private PauseMenuController _pauseMenuController;
    private TextMeshProUGUI _turnTimer;
    private TextMeshProUGUI _activePlayerText;
    private TextMeshProUGUI _turnNumberText;

    private float _turnTime;

    private void Awake()
    {
        //Instantiate the canvas and UI elements
        _canvas = Instantiate(canvasPrefab);
        _turnTimer = Instantiate(turnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
        _activePlayerText = Instantiate(activePlayerText, _canvas.transform).GetComponent<TextMeshProUGUI>();
        _turnNumberText = Instantiate(turnNumberText, _canvas.transform).GetComponent<TextMeshProUGUI>();
        

        //Instantiate the pause menu
        _pauseMenu = Instantiate(pauseMenuPrefab, _canvas.transform);
        _pauseMenuController = _pauseMenu.GetComponent<PauseMenuController>();
        _pauseMenu.SetActive(false);
        
        //Instantiate the gameOver overlay
        Instantiate(gameOverOverlayPrefab, _canvas.transform);

        //Subscribe to events
        EventManager.OnGamePaused += OnGamePaused;
        EventManager.OnTurnChanged += OnTurnChanged;
    }

    private void OnDisable()
    {
        EventManager.OnGamePaused -= OnGamePaused;
        EventManager.OnTurnChanged -= OnTurnChanged;
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
    
    private void OnTurnChanged()
    {
        SetActivePlayerText();
        SetTurnNumberText();
    }
    
    public void SetActivePlayerText()
    {
        _activePlayerText.SetText(GameManager.Instance.ActiveCharacter.Team.TeamName);
        _activePlayerText.color = GameManager.Instance.ActiveCharacter.Team.TeamColor;
    }
    
    public void SetTurnNumberText()
    {
        _turnNumberText.SetText("Turn: " + GameManager.Instance.TurnManager.CurrentTurnIndex);
    }

}
