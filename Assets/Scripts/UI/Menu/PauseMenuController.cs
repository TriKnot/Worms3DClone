using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuController : MonoBehaviour
{

    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _confirmQuitMenu;
    
    private Transform[] _menuItems;

    private void Start()
    {
        _optionsMenu.SetActive(false);
        _confirmQuitMenu.SetActive(false);

        _menuItems = GetComponentsInChildren<Transform>();

    }

    public void OnGamePaused(bool isPaused)
    {
        if (!isPaused)
        {
            _optionsMenu.SetActive(false);
            _confirmQuitMenu.SetActive(false);
        }
    }
    
    public void ToggleOptions(bool active)
    {
        _optionsMenu.SetActive(active);
    }
    
    public void ResumeGame()
    {
        EventManager.InvokeGamePaused(false);
    }

    public void RestartGame()
    {
        EventManager.InvokeRestartGame();
    }

    public void ToggleQuitGameConfirmmenu(bool active)
    {
        _confirmQuitMenu.SetActive(active);
    }

    public void ConfirmQuitToMainMenu()
    {
        EventManager.InvokeBackToMainMenu();
    }
    
    public void ConfirmQuitToDesktop()
    {
        EventManager.InvokeQuitToDesktop();
    }
    
}
