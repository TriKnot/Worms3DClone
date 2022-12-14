using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public TurnManager TurnManager { get; private set; }
    private CameraController _cameraController;
    public UIManager UIManager { get; private set; }
    public static Camera MainCamera { get; private set; }
    
    private List<Team> _teams;
    private int _teamAmount = 2;
    private int _teamSize = 3;

    [SerializeField] private GameObject characterPrefab;

    [SerializeField] private float spawnDistance;
    [SerializeField] private GameObject spawnBounds;
    private Collider[] _spawnColliderArray;
    
    [SerializeField] private Color[] teamColors;

    public CharacterManager ActiveCharacter { get; private set; }
    public int CurrentCharacterIndex { get; private set; }
    public bool IsPaused { get; private set; }

    public int CurrentTeamIndex { get; private set; }


    [SerializeField] private GameObject[] weaponsPrefabs;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }else{
            Instance = this;
        }
        
        TurnManager = GetComponent<TurnManager>();
        _cameraController = GetComponent<CameraController>();
        MainCamera = Camera.main;
        UIManager = GetComponent<UIManager>();

        SetCursorLock(true);
        EventManager.OnPlayerDied += OnPlayerDied;
        EventManager.OnTurnChanged += OnTurnChanged;
        EventManager.OnGamePaused += OnGamePaused;
        EventManager.OnRestartGame += OnRestartGame;
        EventManager.OnBackToMainMenu += OnBackToMainMenu;
        EventManager.OnQuitToDesktop += OnQuitToDesktop;
        EventManager.OnGameEnded += OnGameEnded;
        
    }

    private void Start()
    {
        GetSettings();
        SetupGame();
    }

    private void GetSettings()
    {
        _teamAmount = SettingsManager.TeamAmount;
        _teamSize = SettingsManager.TeamSize;
    }
    
    private void OnDisable()
    {
        EventManager.OnPlayerDied -= OnPlayerDied;
        EventManager.OnTurnChanged -= OnTurnChanged;
        EventManager.OnGamePaused -= OnGamePaused;
        EventManager.OnRestartGame -= OnRestartGame;
        EventManager.OnBackToMainMenu -= OnBackToMainMenu;
        EventManager.OnQuitToDesktop -= OnQuitToDesktop;
        EventManager.OnGameEnded -= OnGameEnded;
    }
    
    private void OnGamePaused(bool isPaused)
    {
        IsPaused = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        SetCursorLock(!isPaused);
    }
    
    private void SetCursorLock(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    private void SetupGame()
    {
        CreateTeams();
        FindAllSpawnBounds();
        SpawnPlayers();
        ActiveCharacter = _teams[0].PlayerCharacters[0];
        ChangeActiveCharacter();
        EventManager.InvokeTurnChanged();
    }
    
    private void CreateTeams()
    {
        _teams = new List<Team>();
        for(int i = 0; i < _teamAmount; i++)
        {
            var newTeam = new Team("Player " + (i + 1), i, teamColors[i]);
            // {
            //     TeamName = "Player " + (i+1),
            //     TeamColor = teamColors[i],
            //     TeamNumber = i
            // };
            _teams.Add(newTeam);
        }
    }

    private void SpawnPlayers()
    {
        for(int i = 0; i < _teams.Count; i++)
        {
            for (int j = 0; j < _teamSize; j++)
            {
                var spawnLocation = Vector3.zero;
                var maxTries = 100;
                do {
                    spawnLocation = RandomSpawnLocation();
                    maxTries--;
                    if (maxTries <= 0)
                    {
                        Debug.LogError("Could not find a valid spawn location");
                        break;
                    }
                } while (!VerifySpawnLocation(spawnLocation, spawnDistance));

                GameObject newCharacter = Instantiate(characterPrefab, spawnLocation, Quaternion.identity);
                newCharacter.GetComponent<PlayerInput>().DeactivateInput();
                CharacterManager characterManager = newCharacter.GetComponent<CharacterManager>();
                characterManager.Team = _teams[i];
                characterManager.CharacterNumber = j;
                _teams[i].AddCharacter(characterManager);
                characterManager.Init();
            }
        }
        MoveAllSpawnLocations();
    }

    private void FindAllSpawnBounds()
    {
        _spawnColliderArray = spawnBounds.GetComponentsInChildren<Collider>();
    }
    
    private void MoveAllSpawnLocations()
    {
        spawnBounds.transform.position += new Vector3(0, 100, 0);
    }

    private Vector3 RandomSpawnLocation()
    {
        var col = _spawnColliderArray[Random.Range(0, _spawnColliderArray.Length)];
        var finalPosition = new Vector3(
            Random.Range(col.bounds.min.x, col.bounds.max.x),
            Random.Range(col.bounds.min.y, col.bounds.max.y),
            Random.Range(col.bounds.min.z, col.bounds.max.z));
        return finalPosition;
    }

    private bool VerifySpawnLocation(Vector3 location, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, radius);
        foreach (var hit in hitColliders)
        {
            if(hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return false;
            }
        }
        return true;
    }

    private void ChangeActiveTeam()
    {
        CurrentTeamIndex ++;
        CurrentTeamIndex %= _teams.Count;
    }
    
    public void ChangeActiveCharacter()
    {
        //Change active player
        //Set old active player to inactive
        ActiveCharacter.IsActiveCharacter = false;
        
        //Find new active player and set active
        var currentTeam = _teams[CurrentTeamIndex];
        CurrentCharacterIndex++;
        CurrentCharacterIndex %= currentTeam.PlayerCharacters.Count;
        ActiveCharacter = currentTeam.PlayerCharacters[CurrentCharacterIndex];
        ActiveCharacter.IsActiveCharacter = true;
       
       
        //Set camera to new target
        var cameraTarget = ActiveCharacter.CameraFollow;
        _cameraController.SetCameraTarget(cameraTarget);

        //Change active player input
        EventManager.InvokeActiveCharacterChanged();
    }
    private void OnPlayerDied(CharacterManager deadCharacter)
    {
        var teamIndex = _teams.IndexOf(deadCharacter.Team);
        _teams[teamIndex].PlayerCharacters.Remove(deadCharacter);
        if (_teams[teamIndex].PlayerCharacters.Count <= 0)
        {
            _teams.Remove(deadCharacter.Team);
        }
        if(_teams.Count <= 1)
        {
            //Game over
            EventManager.InvokeGameEnded(deadCharacter.Team);
        }
        if(deadCharacter == ActiveCharacter)
        {
            EventManager.InvokeTurnChanged();
        }
        print("Player" + deadCharacter.Team + " died");
        Destroy(deadCharacter.gameObject);
    }

    private void OnTurnChanged()
    {
        ChangeActiveTeam();
        ChangeActiveCharacter();
        DropNewWeapons();
    }
    
    //TODO Check bug with playername changing sometimes when one team dies

    public void TogglePause()
    {
        EventManager.InvokeGamePaused(!IsPaused);
    }
    
    public void OnRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    
    public void OnBackToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        SetCursorLock(false);
    }
    
    public void OnQuitToDesktop()
    {
        Application.Quit();
        Time.timeScale = 1;
        SetCursorLock(false);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    private void OnGameEnded(Team winningTeam)
    {
        Time.timeScale = 0;
        SetCursorLock(false);
        CurrentTeamIndex = 0;
    }

    private void DropNewWeapons()
    {
        for (int i = 0; i < TurnManager.CurrentTurnIndex; i++)
        {
            var spawnLocation = RandomSpawnLocation();
            Instantiate(weaponsPrefabs[Random.Range(0, weaponsPrefabs.Length)], spawnLocation, Quaternion.identity);
        }
    }

    
}

