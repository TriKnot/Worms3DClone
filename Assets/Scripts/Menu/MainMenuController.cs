using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject newGameMenu;

    [SerializeField] private Button[] levelButtons;

    [SerializeField] private TMP_Text playerAmountNumberText;
    [SerializeField] private Slider playerAmountSlider;
    [SerializeField] private TMP_Text teamSizeNumberText;
    [SerializeField] private Slider teamSizeSlider;

    private int _levelToLoad = 1;
    private int _teamSize;
    private int _playerAmount;
    
    
    public void NewGame()
    {
        newGameMenu.SetActive(true);
        SetupNewGameOptions();
    }

    public void StartGame()
    {
        SettingsManager.TeamSize = _teamSize;
        SettingsManager.TeamAmount = _playerAmount;
        
        SceneManager.LoadScene(_levelToLoad);
    }
    
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    public void Options()
    {
        optionsMenu.SetActive(true);
    }
    
    private void SetupNewGameOptions()
    {
        levelButtons[_levelToLoad - 1].Select();
        
        //Setup amount of players
        playerAmountSlider.minValue = SettingsManager.MinPlayers;
        playerAmountSlider.maxValue = SettingsManager.MaxPlayers;
        playerAmountSlider.value = SettingsManager.MinPlayers;

        //Setup team size
        teamSizeSlider.minValue = SettingsManager.MinTeamSize;
        teamSizeSlider.maxValue = SettingsManager.MaxTeamSize;
        teamSizeSlider.value = SettingsManager.MinTeamSize;
    }
    
    public void SetTeamSize(float teamSize)
    {
        _teamSize = (int)teamSize;
        teamSizeNumberText.text = _teamSize.ToString();
    }
    
    public void SetPlayerAmount(float amount)
    {
        _playerAmount = (int)amount;
        playerAmountNumberText.text = _playerAmount.ToString();
    }
    
    public void SetLevel(int level)
    {
        _levelToLoad = level;
        SetupNewGameOptions();
    }
    
    public void Back()
    {
        optionsMenu.SetActive(false);
        newGameMenu.SetActive(false);
    }

    
}

