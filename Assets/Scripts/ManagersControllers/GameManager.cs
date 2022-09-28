using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private TurnManager _turnManager;
    private CameraController _cameraController;
    public UIManager UIManager { get; private set; }
    public static Camera MainCamera { get; private set; }
    
    private Team[] _teams;
    [SerializeField] private int teamAmount = 2;
    [SerializeField] private int teamSize = 3;

    [SerializeField] private GameObject characterPrefab;

    [SerializeField] private float spawnDistance;
    [SerializeField] private Collider[] colArray;
    
    [SerializeField] private Color[] teamColors;

    public PlayerCharacter ActiveCharacter { get; private set; }
    public int CurrentCharacterIndex { get; private set; }

    public bool IsPaused { get; private set; }

    public int CurrentTeamIndex { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        _turnManager = GetComponent<TurnManager>();
        _cameraController = GetComponent<CameraController>();
        _teams = new Team[teamAmount];
        MainCamera = Camera.main;
        UIManager = GetComponent<UIManager>();

        SetCursorLock(true);
        EventManager.OnPlayerDied += OnPlayerDied;
        EventManager.OnTogglePlayerControl += TogglePlayerControl;
        EventManager.OnTurnChanged += OnTurnChanged;
        EventManager.OnGamePaused += OnGamePaused;
    }

    private void Start()
    {
        SetupGame();
    }
    
    private void OnDisable()
    {
        EventManager.OnPlayerDied -= OnPlayerDied;
        EventManager.OnTogglePlayerControl -= TogglePlayerControl;
        EventManager.OnTurnChanged -= OnTurnChanged;
        EventManager.OnGamePaused -= OnGamePaused;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            _turnManager.EndTurn();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeActivePlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.InvokeGamePaused(!IsPaused);
        }
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
        SpawnPlayers();
        ActiveCharacter = _teams[0].PlayerCharacters[0];
        ChangeActivePlayer();
    }
    
    private void CreateTeams()
    {
        for(int i = 0; i < teamAmount; i++)
        {
            var newTeam = new Team
            {
                TeamName = "Player " + (i+1),
                TeamColor = teamColors[i],
                TeamNumber = i
            };
            _teams[i] = newTeam;
        }
    }

    private void SpawnPlayers()
    {
        for(int i = 0; i < _teams.Length; i++)
        {
            for (int j = 0; j < teamSize; j++)
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
                PlayerCharacter playerCharacter = newCharacter.GetComponent<PlayerCharacter>();
                playerCharacter.Team = _teams[i];
                playerCharacter.characterNumber = j;
                var color = _teams[i].TeamColor;
                newCharacter.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
                _teams[i].PlayerCharacters.Add(playerCharacter);
            }
        }

        foreach (var col in colArray)
        {
            Destroy(col.gameObject);
        }
    }

    private Vector3 RandomSpawnLocation()
    {
        var col = colArray[Random.Range(0, colArray.Length)];
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
        CurrentTeamIndex %= teamAmount;
    }
    
    private void ChangeActivePlayer()
    {
        //Change active player
        var prevPlayer = ActiveCharacter;
        var currentTeam = _teams[CurrentTeamIndex];
        CurrentCharacterIndex++;
        CurrentCharacterIndex %= currentTeam.PlayerCharacters.Count;
        ActiveCharacter = currentTeam.PlayerCharacters[CurrentCharacterIndex];
       
        //Set controls to active player
        prevPlayer.GetComponent<PlayerInput>().DeactivateInput();
        ActiveCharacter.GetComponent<PlayerInput>().ActivateInput();

        //Set camera to new target
        var cameraTarget = ActiveCharacter.GetComponent<PlayerMovement>().CameraFollow;
        _cameraController.SetCameraTarget(cameraTarget);
        EventManager.InvokeActiveCharacterChanged();
        
    }

    private void TogglePlayerControl(bool value)
    {
        if (value)
        {
            ActiveCharacter.GetComponent<PlayerInput>().ActivateInput();
        }
        else
        {
            ActiveCharacter.GetComponent<PlayerInput>().DeactivateInput();
        }
    }

    private void OnPlayerDied(PlayerCharacter character)
    {
        _teams[character.Team.TeamNumber].PlayerCharacters.Remove(character);
        Destroy(character.gameObject);
    }

    private void OnTurnChanged()
    {
        ChangeActiveTeam();
        ChangeActivePlayer();
    }
    
}

