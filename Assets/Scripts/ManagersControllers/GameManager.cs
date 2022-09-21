using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static EventManager EventManager { get; private set; }
    private static TurnManager _turnManager;
    
    private Team[] teams;
    [SerializeField] private int teamAmount = 2;
    [SerializeField] private int teamSize = 3;

    [SerializeField] private GameObject characterPrefab;

    [SerializeField] private float mapSize;
    [SerializeField] private float spawnDistance;
    
    [SerializeField] private Color[] teamColors;

    public PlayerCharacter ActivePlayerCharacter { get; private set; }
    private int activePlayerIndex;

    public CinemachineVirtualCamera vCam;
    public CinemachineFreeLook freeCam;

    private bool paused = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        EventManager = new EventManager();
        _turnManager = new TurnManager(teamAmount);
        teams = new Team[teamAmount];
        

        SetCursorLock(true);
        
    }

    private void Start()
    {
        SetupGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            NextTurn();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            NextPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
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
        paused = true;
        Time.timeScale = 0;
    }

    private void Resume()
    {
        paused = false;
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
        SetUpNewActivePlayer(ActivePlayerCharacter);
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
                var maxTries = 15;
                do {
                    spawnLocation = RandomNavmeshLocation(mapSize) + new Vector3(0, 1, 0);
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
                playerCharacter.team = teams[i];
                playerCharacter.characterNumber = j;
                var color = teams[i].TeamColor;
                newCharacter.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
                teams[i].PlayerCharacters.Add(playerCharacter);
            }
        }
    }
    
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
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

    private void NextTurn()
    {
        var prevPlayer = ActivePlayerCharacter;
        _turnManager.NextTurn();
        ActivePlayerCharacter = teams[_turnManager.currentTeamIndex].PlayerCharacters[0];
        ActivePlayerCharacter.GetComponent<PlayerInput>().ActivateInput();
        activePlayerIndex = 0;
        SetUpNewActivePlayer(prevPlayer);
    }

    private void NextPlayer()
    {
        var prevPlayer = ActivePlayerCharacter;
        var currentTeam = teams[_turnManager.currentTeamIndex];
        activePlayerIndex++;
        activePlayerIndex %= currentTeam.PlayerCharacters.Count;
        ActivePlayerCharacter = currentTeam.PlayerCharacters[activePlayerIndex];
        SetUpNewActivePlayer(prevPlayer);
    }

    private void SetUpNewActivePlayer(PlayerCharacter previousPlayer)
    {
        previousPlayer.GetComponent<PlayerInput>().DeactivateInput();
        ActivePlayerCharacter.GetComponent<PlayerInput>().ActivateInput();
        var cameraTarget = ActivePlayerCharacter.transform.Find("CameraFollowTarget").transform;
        freeCam.Follow = cameraTarget;
        freeCam.LookAt = cameraTarget;
        EventManager.InvokeActiveCharacterChanged(ActivePlayerCharacter);
    }

    public void PlayerDied(PlayerCharacter character)
    {
        teams[character.team.TeamNumber].PlayerCharacters.Remove(character);
    }

}

