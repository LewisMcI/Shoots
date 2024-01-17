using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public delegate void PlayerAddedEventHandler(ulong clientId);
public delegate void PlayerRemovedEventHandler(ulong clientId);
public delegate void HostLeftEventHandler();
public class PlayerManager : NetworkBehaviour
{
    // Singleton
    public static PlayerManager instance;

    // Player Delegates
    public event PlayerAddedEventHandler OnPlayerAdded;
    public event PlayerRemovedEventHandler OnPlayerRemoved;
    public event HostLeftEventHandler OnHostRemoved;

    // Local Dictionary associating ClientID's and Player Data
    Dictionary<ulong, PlayerData> clientPlayerDictionary = new Dictionary<ulong, PlayerData>();

    // Local Dictionary associating ClientID's and Player Controllers
    public Dictionary<ulong, GameObject> clientPlayerControllerDictionary = new Dictionary<ulong, GameObject>();

    // Server Dictionary associating ClientID's and Serialized Player Data
    Dictionary<ulong, string> serverPlayerDictionary = new Dictionary<ulong, string>();

    // Values
    public int GetPlayerCount() { return clientPlayerDictionary.Count; }
    public ulong GetPlayerId(int index)
    {
        int i = 0;
        foreach (var kvp in serverPlayerDictionary)
        {
            if (index == i)
                return kvp.Key;
            i++;
        }
        throw new Exception("Tried get PlayerID that does not exist");
    }

    public PlayerData GetPlayerData(ulong clientId)
    {
        return clientPlayerDictionary[clientId];
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);

            instance = this;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;
        NetworkManager.Singleton.OnClientStopped += OnHostLeft;

        DontDestroyOnLoad(gameObject);
    }
    #region playerJoined
    /* OnPlayerJoined
     * Called within the connected client and the server
     */
    private void OnPlayerJoined(ulong clientId)
    {
        // Only call on clients
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Get Local Players PlayerData
            string playerDataJson = JsonSerializer.SerializePlayerData(Player.Instance.PlayerData);

            // Notify Server
            NotifyPlayerJoinedServerRpc(NetworkManager.Singleton.LocalClientId, playerDataJson);
        }
        else
            Debug.Log("Local: Player" + clientId + " Joined");
    }

    [ServerRpc(RequireOwnership=false)]
    private void NotifyPlayerJoinedServerRpc(ulong newPlayerClientId, string newSerializedPlayerData)
    {
        // Store Serverside
        serverPlayerDictionary.Add(newPlayerClientId, newSerializedPlayerData);

        // Pass through dictionary values to player.
        string[] players = new string[serverPlayerDictionary.Count];
        ulong[] clientIds = new ulong[serverPlayerDictionary.Count];
        int i = 0;
        foreach (var kvp in serverPlayerDictionary)
        {
            players[i] = kvp.Value;
            clientIds[i] = kvp.Key;
            i++;
        }

        string serializedPlayerClientIdList = JsonSerializer.SerializeUlongArray(clientIds);
        string serializedPlayerDataList = JsonSerializer.SerializeJsonArray(players);

        NotifyPlayerJoinedClientRpc(serializedPlayerClientIdList,  serializedPlayerDataList);
    }

    [ClientRpc]
    private void NotifyPlayerJoinedClientRpc(string serializedPlayerClientIdList, string serializedPlayerDataList)
    {
        // Deserialize PlayerData
        ulong[] playerClientIdList = JsonSerializer.DeserializeUlongArray(serializedPlayerClientIdList);
        string[] playerDataList = JsonSerializer.DeserializeJsonArray(serializedPlayerDataList);

        for (int i = 0; i < playerClientIdList.Length; i++)
        {
            if (clientPlayerDictionary.ContainsKey(playerClientIdList[i]))
                continue;
            PlayerData thisPlayerData = JsonSerializer.DeserializePlayerData(playerDataList[i]);

            AddPlayer(playerClientIdList[i], thisPlayerData);
        }
    }

    private void AddPlayer(ulong clientId, PlayerData playerData)
    {
        clientPlayerDictionary.Add(clientId, playerData);

        Debug.Log("Local: Player " + clientId + ", with Name: " + playerData.name + " has been Added");

        OnPlayerAdded?.Invoke(clientId);
    }
    #endregion
    #region playerLeft
    /* OnPlayerLeft
    * Called within the connected client and the server
    */
    private void OnPlayerLeft(ulong clientId)
    {
        if (IsServer)
        {
            // Notify Clients
            NotifyPlayerLeftClientRpc(clientId);
            serverPlayerDictionary.Remove(clientId);
            Debug.Log("Server: Player" + clientId + " Left");
        }
    }

    [ClientRpc]
    private void NotifyPlayerLeftClientRpc(ulong clientId)
    {
        Debug.Log("Local: Player" + clientId + " Left");

        RemovePlayer(clientId);
    }
    private void RemovePlayer(ulong clientId)
    {
        clientPlayerDictionary.Remove(clientId);

        Debug.Log("Local: Player " + clientId + " has been Removed");

        OnPlayerRemoved?.Invoke(clientId);
    }
    #endregion
    #region hostLeft
    private void OnHostLeft(bool obj)
    {
        Debug.Log("Local: Host Left");

        List<ulong> keysToRemove = new List<ulong>();
        foreach (var kvp in clientPlayerDictionary)
        {
            // Add keys to the removal list
            keysToRemove.Add(kvp.Key);
        }

        // Iterate over the removal list and remove items from the dictionary
        foreach (var key in keysToRemove)
        {
            clientPlayerDictionary.Remove(key);
        }


        OnHostRemoved?.Invoke();
    }
    #endregion
    #region maxHealth
    // Only call when SERVER wants to update player max health
    public void UpdatePlayerMaxHealth(ulong clientId, float newHealth)
    {
        UpdatePlayerMaxHealthServerRpc(clientId, newHealth);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerMaxHealthServerRpc(ulong clientId, float newHealth)
    {
        UpdatePlayerMaxHealthClientRpc(clientId, newHealth);
    }

    [ClientRpc]
    private void UpdatePlayerMaxHealthClientRpc(ulong clientId, float newHealth)
    {
        PlayerData playerData = clientPlayerDictionary[clientId];
        playerData.maxHealth = newHealth;
        clientPlayerDictionary[clientId] = playerData;
    }
    #endregion
    #region currHealth
    // Only call when SERVER wants to deal player damage
    public void PlayerDealDamage(ulong clientId, float damage)
    {
        PlayerDealDamageServerRpc(clientId, damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerDealDamageServerRpc(ulong clientId, float damage)
    {
        PlayerDealDamageClientRpc(clientId, damage);

        PlayerData playerData = clientPlayerDictionary[clientId];
        playerData.currHealth = playerData.currHealth - damage;

        if (playerData.currHealth <= 0) { GameManager.instance.GameOverServerRPC(); }
    }

    [ClientRpc]
    private void PlayerDealDamageClientRpc(ulong clientId, float damage)
    {
        PlayerData playerData = clientPlayerDictionary[clientId];
        playerData.currHealth = playerData.currHealth - damage;
        clientPlayerDictionary[clientId] = playerData;
        Debug.Log("New HP for Client: " + clientId + " = " + playerData.currHealth);

        Vector3 scale;
        if (playerData.currHealth <= 0) { scale = new Vector3(0.0f, 1.0f, 1.0f); }
        else { scale = new Vector3(playerData.currHealth / playerData.maxHealth, 1.0f, 1.0f); }
        Transform playerHealthBar = clientPlayerControllerDictionary[clientId].transform.GetChild(1).GetChild(0);
        playerHealthBar.localScale = scale;
    }
    #endregion
}
