using TMPro;
using UnityEngine;

public class GameOverOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winningPlayerText;

    private void Awake()
    {
         
        gameObject.SetActive(false);
        EventManager.OnGameEnded += ShowGameOverOverlay;
    }

    private void OnDisable()
    {
        EventManager.OnGameEnded -= ShowGameOverOverlay;
    }
    
    private void ShowGameOverOverlay(Team winningTeam)
    {
        gameObject.SetActive(true);
        string text = "Congratulations\n" + winningTeam.TeamName + "\nYou won!";
        winningPlayerText.text = text;
    }
    
    public void ReplayButton()
    {
        EventManager.InvokeRestartGame();
    }
    
    public void MainMenuButton()
    {
        EventManager.InvokeBackToMainMenu();
    }
}
