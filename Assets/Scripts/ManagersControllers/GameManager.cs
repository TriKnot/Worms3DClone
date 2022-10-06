using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    }
    
    private void CreateTeams()
    {
        _teams = new List<Team>();
        for(int i = 0; i < _teamAmount; i++)
        {
            var newTeam = new Team
            {
                TeamName = "Player " + (i+1),
                TeamColor = teamColors[i],
                TeamNumber = i
            };
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
                characterManager.characterNumber = j;
                var color = _teams[i].TeamColor;
                newCharacter.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
                _teams[i].PlayerCharacters.Add(characterManager);
            }
        }
        RemoveAllSpawnLocations();
    }

    private void FindAllSpawnBounds()
    {
        _spawnColliderArray = spawnBounds.GetComponentsInChildren<Collider>();
    }
    
    private void RemoveAllSpawnLocations()
    {
        Destroy(spawnBounds);
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
        CurrentTeamIndex %= _teamAmount;
    }
    
    public void ChangeActiveCharacter()
    {
        //Change active player
        //Setold active player to inactive
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
        _teams[deadCharacter.Team.TeamNumber].PlayerCharacters.Remove(deadCharacter);
        if (_teams[deadCharacter.Team.TeamNumber].PlayerCharacters.Count <= 0)
        {
            _teams.Remove(deadCharacter.Team);
        }
        if(_teams.Count <= 1)
        {
            //Game over
            EventManager.InvokeGameEnded(deadCharacter.Team);
        }
        Destroy(deadCharacter.gameObject);
    }

    private void OnTurnChanged()
    {
        ChangeActiveTeam();
        ChangeActiveCharacter();
    }

    public void TogglePause()
    {
        EventManager.InvokeGamePaused(!IsPaused);
    }
    
}

