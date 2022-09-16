using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private Team[] teams;
    [SerializeField] public int teamAmount = 2;
    private int currentTeamIndex;
    private int currentTurnIndex;
    public int teamSize = 3;

    [SerializeField] private GameObject characterPrefab;

    [SerializeField] private GameObject[] spawnPositions;

    [SerializeField] private float mapSize;
    [SerializeField] private float spawnDistance;
    
    [SerializeField] private Color[] teamColors;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        teams = new Team[teamAmount];
        
        StartGame();
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //NextTurn();
        }
    }

    private void NextTurn()
    {
        currentTeamIndex++;
        currentTeamIndex %= teams.Length;
        currentTurnIndex++;
    }

    private void StartGame()
    {
        CreateTeams();
        SpawnPlayers();
    }
    
    private void CreateTeams()
    {
        for(int i = 0; i < teamAmount; i++)
        {
            var newTeam = new Team
            {
                teamName = "Player " + ++currentTeamIndex,
                teamColor = teamColors[i],
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
                // GameObject newCharacter = Instantiate(characterPrefab, spawnPositions[i].transform.position + new Vector3(1*(j+1), 1, 1*(j+1)), Quaternion.identity);
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
                PlayerCharacter playerCharacter = newCharacter.GetComponent<PlayerCharacter>();
                playerCharacter.team = teams[i];
                newCharacter.GetComponent<MeshRenderer>().material.color = teams[i].teamColor;
                teams[i].characters.Add(playerCharacter);
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

}

