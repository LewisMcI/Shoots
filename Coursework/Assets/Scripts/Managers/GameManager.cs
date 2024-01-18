using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    // Singleton
    public static GameManager instance;

    [SerializeField]
    Leaderboards leaderboards;
    List<ulong> deadPlayers = new List<ulong>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance.gameObject);

            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    [ServerRpc]
    public void KillPlayerServerRPC(ulong clientID)
    {
        deadPlayers.Add(clientID);
        Debug.Log("Kill Player");
        // If 1 left
        if (deadPlayers.Count >= PlayerManager.instance.GetPlayerCount() - 1)
        {
            KeyValuePair<ulong, GameObject> firstKeyValuePair = PlayerManager.instance.clientPlayerControllerDictionary.First();

            ulong winnerClientId = firstKeyValuePair.Key;
            Debug.Log("Player " + winnerClientId + " has won!!!");
            leaderboards.AddWins(PlayerManager.instance.GetPlayerData(winnerClientId).name, 1);
            PlayerManager.instance.ResetControllers();
            deadPlayers = new List<ulong>();
            //GameOverServerRPC();
        }
    }
}
