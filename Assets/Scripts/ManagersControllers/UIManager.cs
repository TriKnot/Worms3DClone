using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Element Prefabs (Drag & Drop)")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject smallTurnTimerPrefab;
    [SerializeField] private GameObject largeTurnTimerPrefab;
    [SerializeField] private GameObject gameOverOverlayPrefab;
    [SerializeField] private GameObject activePlayerText;
    [SerializeField] private GameObject turnNumberText;

    private GameObject _canvas;
    private GameObject _pauseMenu;
    private PauseMenuController _pauseMenuController;
    private TextMeshProUGUI _smallTurnTimer;
    private TextMeshProUGUI _largeTurnTimer;
    private TextMeshProUGUI _activePlayerText;
    private TextMeshProUGUI _turnNumberText;

    private float _turnTime;

    private void Awake()
    {
        //Instantiate the canvas and UI elements
        _canvas = Instantiate(canvasPrefab);
        _smallTurnTimer = Instantiate(smallTurnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
        _largeTurnTimer = Instantiate(largeTurnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
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
        EventManager.OnGameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        EventManager.OnGamePaused -= OnGamePaused;
        EventManager.OnTurnChanged -= OnTurnChanged;
        EventManager.OnGameEnded -= OnGameEnded;
    }

    private void OnGamePaused(bool isPaused)
    {
        _pauseMenuController.OnGamePaused(isPaused);
        _pauseMenu.SetActive(isPaused);
        if(isPaused)
        {
            _smallTurnTimer.SetText("Paused: " + _turnTime.ToString("F0"));
        }
    }
    
    public void TurnTimerOnGUI(float turnTime)
    {
        _turnTime = turnTime;
        if(turnTime > 10)
        {
            _smallTurnTimer.gameObject.SetActive(true);            
            _largeTurnTimer.gameObject.SetActive(false);
            _smallTurnTimer.SetText(_turnTime.ToString("F0"));
        }
        else
        {
            _smallTurnTimer.gameObject.SetActive(false);
            _largeTurnTimer.gameObject.SetActive(true);
            _largeTurnTimer.SetText(_turnTime.ToString("F0"));
        }    
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

    private void OnGameEnded(Team winner)
    {
        _smallTurnTimer.gameObject.SetActive(false);
        _largeTurnTimer.gameObject.SetActive(false);
        _activePlayerText.gameObject.SetActive(false);
    }

}
