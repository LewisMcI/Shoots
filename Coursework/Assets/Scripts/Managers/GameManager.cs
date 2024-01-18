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
    List<ulong> deadPlayers;
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

        if (deadPlayers.Count >= PlayerManager.instance.GetPlayerCount() - 1)
        {
            for (int i = 0; i < PlayerManager.instance.GetPlayerCount(); i++)
            {
                if (PlayerManager.instance.GetPlayerId(i) == deadPlayers[i])
                {
                    // PlayerManager.instance.GetPlayerId(i) won
                }

            }
            GameOverServerRPC();
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
