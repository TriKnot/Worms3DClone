using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            NextTurn();
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
        SpawnCharacters();
    }
    
    private void CreateTeams()
    {
        for(int i = 0; i < teamAmount; i++)
        {
            Team newTeam = new Team();
            newTeam.teamName = "Player " + ++currentTeamIndex;
            teams[i] = newTeam;
        }
    }

    private void SpawnCharacters()
    {
        for(int i = 0; i < teams.Length; i++)
        {
            for (int j = 0; j < teamSize; j++)
            {
                GameObject newCharacter = Instantiate(characterPrefab, spawnPositions[i].transform.position + new Vector3(1*(j+1), 1, 1*(j+1)), Quaternion.identity);
                Character character = newCharacter.GetComponent<Character>();
                character.team = teams[i];
                teams[i].characters.Add(character);
            }
        }
    }
}

//TODO Check out Object Pooling