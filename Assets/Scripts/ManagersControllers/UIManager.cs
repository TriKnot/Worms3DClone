using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Element Prefabs (Drag & Drop)")]
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject turnTimerPrefab;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject confirmQuitMenuPrefab;
    
    private GameObject _canvas;
    private GameObject _pauseMenu;
    private TextMeshProUGUI _turnTimer;
    private GameObject _optionsMenu;
    private GameObject _confirmQuitMenu;

    private float _turnTime;

    private void Start()
    {
        //Instantiate the canvas and UI elements
        _canvas = Instantiate(canvasPrefab);
        _turnTimer = Instantiate(turnTimerPrefab, _canvas.transform).GetComponent<TextMeshProUGUI>();
        
        //Instantiate the pause menu
        _pauseMenu = Instantiate(pauseMenuPrefab, _canvas.transform);
        _pauseMenu.SetActive(false);
        _optionsMenu = Instantiate(optionsMenu, _pauseMenu.transform);
        _optionsMenu.SetActive(false);
        _confirmQuitMenu = Instantiate(confirmQuitMenuPrefab, _pauseMenu.transform);
        _confirmQuitMenu.SetActive(false);


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

        _optionsMenu.SetActive(false);
    }
    
    public void TurnTimerOnGUI(float turnTime)
    {
        _turnTime = turnTime;
        _turnTimer.SetText(_turnTime.ToString("F0"));
    }
    
    public void ResumeGame()
    {
        EventManager.InvokeGamePaused(false);
    }
    
    public void ToggleOptions(bool active)
    {
        _optionsMenu.SetActive(active);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        _confirmQuitMenu.SetActive(true);
    }

    public void ConfirmQuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void ConfirmQuitToDesktop()
    {
        Application.Quit();
    }
}
