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
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleQuitGameConfirmmenu(bool active)
    {
        _confirmQuitMenu.SetActive(active);
    }

    public void ConfirmQuitToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
    
    public void ConfirmQuitToDesktop()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
}
