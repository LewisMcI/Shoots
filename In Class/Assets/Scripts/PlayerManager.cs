using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    // Local Dictionary associating ClientID's and Player Data
    Dictionary<ulong, PlayerData> playerDictionary = new Dictionary<ulong, PlayerData>();
    // Server Dictionary associating ClientID's and Serialized Player Data
    Dictionary<ulong, string> serverDictionary = new Dictionary<ulong, string>();
    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
    }
    /* OnPlayerJoined
     * Called within the connected client and the server
     */
    private void OnPlayerJoined(ulong clientId)
    {
        if (!IsServer)
            OnPlayerJoinedClient(clientId);
    }

    private void OnPlayerJoinedClient(ulong newPlayerClientId)
    {
        // Get Local Players PlayerData
        string playerDataJson = Player.Instance.SerializePlayerData();
        // Notify Server
        NotifyPlayerJoinedServerRpc(newPlayerClientId, playerDataJson);
    }

    [ServerRpc]
    private void NotifyPlayerJoinedServerRpc(ulong newPlayerClientId, string serializedPlayerData)
    {
        // Store Serverside
        serverDictionary.Add(newPlayerClientId, serializedPlayerData);
    }

    /* OnPlayerLeft
    * Called within the connected client and the server
    */
    private void OnPlayerLeft(ulong clientId)
    {
        // Is Client
        if ((IsServer && clientId == NetworkManager.Singleton.LocalClientId) || (!IsServer && IsClient))
            OnPlayerLeftClient();
        // Is Server
        else
            OnPlayerLeftServer();
    }

    private void OnPlayerLeftServer()
    {

    }

    private void OnPlayerLeftClient()
    {

    }
}
