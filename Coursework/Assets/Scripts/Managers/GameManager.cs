using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
            // Should only be one
            foreach (var item in PlayerManager.instance.clientPlayerControllerDictionary)
            {
                ulong winnerClientId = item.Key;
                Debug.Log("Player " + winnerClientId + " has won!!!");
            }
            //GameOverServerRPC();
        }
    }

    [ServerRpc]
    public void GameOverServerRPC()
    {
        PlayerManager.instance.OnPlayerRemoved += WaitForAllPlayers;
        Debug.Log(PlayerManager.instance.GetPlayerCount());
        GameOverClientRPC();
    }

    private void WaitForAllPlayers(ulong clientId)
    {
        if (PlayerManager.instance.GetPlayerCount() == 1)
            MoveToMain();
        Debug.Log(PlayerManager.instance.GetPlayerCount());
    }

    [ClientRpc]
    private void GameOverClientRPC()
    {
        if (IsServer)
            return;

        MoveToMain();
    }

    void MoveToMain()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("Managers"));
        foreach (var root in gameObject.scene.GetRootGameObjects())
        {
            if (root == gameObject)
                continue;
            Destroy(root);
        }

        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

}
