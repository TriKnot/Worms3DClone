using UnityEngine;

public class UI_PausMenuButtonFunctions : MonoBehaviour
{
    private UIManager _uiManager;

    public void Init(UIManager uiManager)
    {
        _uiManager = uiManager;
    }
    
    public void ResumeGame()
    {
        EventManager.InvokeGamePaused(false);
    }
    
    public void ToggleOptions(bool active)
    {
        _uiManager.OptionsMenu.SetActive(active);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
