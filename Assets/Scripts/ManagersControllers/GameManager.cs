using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static EventManager EventManager { get; private set; }
    private static TurnManager _turnManager;
    
    public static Camera MainCamera { get; private set; }
    
    private Team[] teams;
    [SerializeField] private int teamAmount = 2;
    [SerializeField] private int teamSize = 3;

    [SerializeField] private GameObject characterPrefab;

    [SerializeField] private float spawnDistance;
    [SerializeField] private Collider[] colArray;
    
    [SerializeField] private Color[] teamColors;

    public PlayerCharacter ActivePlayerCharacter { get; private set; }
    private int _activePlayerIndex;

    public CinemachineFreeLook freeCam;

    public bool IsPaused { get; private set; }= false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        EventManager = new EventManager();
        _turnManager = GetComponent<TurnManager>();
        _turnManager.Init(teamAmount);
        teams = new Team[teamAmount];
        MainCamera = Camera.main;

        SetCursorLock(true);
        EventManager.OnPlayerDied += OnPlayerDied;
        EventManager.OnTogglePlayerControl += TogglePlayerControl;
    }

    private void Start()
    {
        SetupGame();
    }
    
    private void OnDisable()
    {
        EventManager.OnPlayerDied -= OnPlayerDied;
        EventManager.OnTogglePlayerControl -= TogglePlayerControl;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            print("GameManager -> TurnChanged");
            EventManager.InvokeTurnChanged();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            print("GameManager -> Change active character");
            ChangeActivePlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0;
    }

    private void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1;
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
        ActivePlayerCharacter = teams[0].PlayerCharacters[0];
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
            teams[i] = newTeam;
        }
    }

    private void SpawnPlayers()
    {
        for(int i = 0; i < teams.Length; i++)
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
                playerCharacter.Team = teams[i];
                playerCharacter.characterNumber = j;
                var color = teams[i].TeamColor;
                newCharacter.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
                teams[i].PlayerCharacters.Add(playerCharacter);
            }
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

    // private void NextPlayer()
    // {
    //     var prevPlayer = ActivePlayerCharacter;
    //     var currentTeam = teams[_turnManager.currentTeamIndex];
    //     _activePlayerIndex++;
    //     _activePlayerIndex %= currentTeam.PlayerCharacters.Count;
    //     ActivePlayerCharacter = currentTeam.PlayerCharacters[_activePlayerIndex];
    //     ChangeActivePlayer(prevPlayer);
    // }

    private void ChangeActivePlayer()
    {
        ActivePlayerCharacter.GetComponent<PlayerInput>().DeactivateInput();
        print("previous player index: " + _activePlayerIndex);
        ActivePlayerCharacter = teams[_turnManager.currentTeamIndex].PlayerCharacters[GetNextPlayerIndex()];
        print("active player index: " + _activePlayerIndex);

        ActivePlayerCharacter.GetComponent<PlayerInput>().ActivateInput();
        var cameraTarget = ActivePlayerCharacter.transform.Find("CameraFollowTarget").transform;
        freeCam.Follow = cameraTarget;
        freeCam.LookAt = cameraTarget;
        EventManager.InvokeActiveCharacterChanged();

    }
    
    private int GetNextTeamIndex()
    {
        var nextTeamIndex = _turnManager.currentTeamIndex + 1;
        nextTeamIndex %= teamAmount;
        return nextTeamIndex;
    }
    
    private int GetNextPlayerIndex()
    {
        var nextPlayerIndex = _activePlayerIndex++;
        nextPlayerIndex %= teamSize;
        return nextPlayerIndex;
    }

    private void TogglePlayerControl(bool value)
    {
        if (value)
        {
            ActivePlayerCharacter.GetComponent<PlayerInput>().ActivateInput();
        }
        else
        {
            ActivePlayerCharacter.GetComponent<PlayerInput>().DeactivateInput();
        }
    }

    private void OnPlayerDied(PlayerCharacter character)
    {
        teams[character.Team.TeamNumber].PlayerCharacters.Remove(character);
        Destroy(character.gameObject);
    }


}

