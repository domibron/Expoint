using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    TeamASpawnPoint[] TeamASpawnPoints;
    TeamBSpawnPoint[] TeamBSpawnPoints;

    private void Awake()
    {
        Instance = this;
        TeamASpawnPoints = GetComponentsInChildren<TeamASpawnPoint>();
        TeamBSpawnPoints = GetComponentsInChildren<TeamBSpawnPoint>();
    }

    public Transform GetTeamASpawnPoint()
    {
        return TeamASpawnPoints[Random.Range(0, TeamASpawnPoints.Length)].transform;
    }

    public Transform GetTeamBSpawnPoint()
    {
        return TeamBSpawnPoints[Random.Range(0, TeamBSpawnPoints.Length)].transform;
    }

}
