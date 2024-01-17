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
