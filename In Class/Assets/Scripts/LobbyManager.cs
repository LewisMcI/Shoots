using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    // Local Dictionary associating ClientID's and Player Cards
    Dictionary<ulong, GameObject> playerCardDictionary = new Dictionary<ulong, GameObject>();

    // Local Dictionary associating ClientID's and Player Data
    Dictionary<ulong, PlayerData> playerDictionary = new Dictionary<ulong, PlayerData>();

    // Server Dictionary associating ClientID's and Serialized Player Data
    Dictionary<ulong, string> serverDictionary = new Dictionary<ulong, string>();

    // Server Dictionary associating ClientID's and Serialized Ready Up Status
    Dictionary<ulong, bool> serverReadyUpDictionary = new Dictionary<ulong, bool>();


    public enum ButtonType
    {
        Host,
        Client,
        Server,
        Quit
    }
    public void ButtonPress(int intButtonType)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerLeft;

        ButtonType buttonType;
        if (intButtonType < Enum.GetNames(typeof(ButtonType)).Length)
            buttonType = (ButtonType)intButtonType;
        else
            throw new Exception("Button Type not found");
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            switch (buttonType)
            {
                case ButtonType.Host:
                    NetworkManager.Singleton.StartHost();   
                    break;
                case ButtonType.Client:
                    NetworkManager.Singleton.StartClient();
                    break;
                case ButtonType.Server:
                    NetworkManager.Singleton.StartServer();
                    break;
                case ButtonType.Quit:
                    #if UNITY_STANDALONE
                    Application.Quit();
                    #endif
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    break;
            }
        }
    }

    ulong localClientId = ulong.MaxValue;
    /* OnPlayerJoined
     * Called within the connected client and the server
     */
    private void OnPlayerJoined(ulong clientId)
    {
        // Sent ClientID for Client and Server
        if (localClientId == ulong.MaxValue)
            localClientId = NetworkManager.Singleton.LocalClientId;

        // Is Client
        if ((IsServer && clientId == localClientId) || (!IsServer && IsClient))
        {
            string playerDataJson = Player.Instance.SerializePlayerData();
            BroadcastPlayerJoinServerRpc(clientId, playerDataJson);
        }
        // Is Server
        else;
    }

    [ServerRpc(RequireOwnership=false)]
    private void BroadcastPlayerJoinServerRpc(ulong newClientId, string playerDataJson)
    {
        Debug.Log("Server: Added new player");
        serverDictionary.Add(newClientId, playerDataJson);
        serverReadyUpDictionary.Add(newClientId, false);

        // Pass through dictionary values to player.
        string[] players = new string[serverDictionary.Count];
        ulong[] clientIds = new ulong[serverDictionary.Count];
        int i = 0;
        foreach (var kvp in serverDictionary)
        {
            players[i] = kvp.Value;
            clientIds[i] = kvp.Key;
            i++;
        }
        BroadcastPlayerJoinClientRpc(newClientId, playerDataJson, JsonSerializer.SerializeJsonArray(players), JsonSerializer.SerializeUlongArray(clientIds));
    }
    [ClientRpc]
    private void BroadcastPlayerJoinClientRpc(ulong newClientId, string newPlayerDataJson, string serializedPlayerList, string serializedClientIdList)
    {
        // Player
        if (localClientId == newClientId)
        {
            string[] existingPlayersStringArray = JsonSerializer.DeserializeJsonArray(serializedPlayerList);
            ulong[] existingPlayersClientIdArray = JsonSerializer.DeserializeUlongArray(serializedClientIdList);
            for (int i = 0; i < existingPlayersStringArray.Length; i++)
            {
                string existingPlayerDataString = existingPlayersStringArray[i];
                ulong existingPlayerClientId = existingPlayersClientIdArray[i];

                PlayerData existingPlayer = Player.Instance.DeserializePlayerData(existingPlayerDataString);
                if (existingPlayerClientId == localClientId)
                    Debug.Log("Local: Current Player " + existingPlayerClientId + ", with Name: " + existingPlayer.playerName + " has been Added");
                else
                    Debug.Log("Local: Existing Player " + existingPlayerClientId + ", with Name: " + existingPlayer.playerName + " has been Added");


                AddPlayerCard(existingPlayerClientId, existingPlayerDataString);
            }
        }
        // Other
        else
        {
            PlayerData newPlayerData = Player.Instance.DeserializePlayerData(newPlayerDataJson);
            Debug.Log("Local: New Player " + newClientId + ", with Name: " + newPlayerData.playerName + " has Joined");
            AddPlayerCard(newClientId, newPlayerDataJson);
        }
    }

    public RectTransform centerRect;
    private float spacing = 2f;

    void AddPlayerCard(ulong clientId, string playerDataJson)
    {
        PlayerData playerData = Player.Instance.DeserializePlayerData(playerDataJson);
        playerDictionary.Add(clientId, playerData);
        GameObject newPlayerCard = Player.Instance.CreatePlayerCard(playerData);
        playerCardDictionary.Add(clientId, newPlayerCard);
        PositionPlayerCards();
    }

    private void PositionPlayerCards()
    {
        int playerCount = playerCardDictionary.Count - 1;

        Vector3 centerPosition = centerRect.position;
        if (playerCount == 0)
        {
            playerCardDictionary.Values.First().GetComponent<RectTransform>().position = centerPosition;
            return;
        }
        float totalWidth = 0f;
        float halfTotalWidth = 0f;
        foreach (var kvp in playerCardDictionary)
        {
            totalWidth += kvp.Value.GetComponent<RectTransform>().rect.width / 100;
        }

        halfTotalWidth = (totalWidth + (spacing * (playerCount - 1))) / 2;

        int i = 0;
        foreach (var kvp in playerCardDictionary)
        {
            float xOffset = (i * (kvp.Value.GetComponent<RectTransform>().rect.width / 100 + spacing)) - halfTotalWidth;
            Vector3 cardPosition = new Vector3(centerPosition.x + xOffset, centerPosition.y, centerPosition.z);
            kvp.Value.GetComponent<RectTransform>().position = cardPosition;
            i++;
        }
    }

    /* OnPlayerLeft
     * Called within the connected client and the server
     */
    private void OnPlayerLeft(ulong clientId)
    {
        // Is Owner of Player
        if ((IsServer && clientId == localClientId) || (!IsServer && IsClient))
            Debug.Log("Player has Left"); // Player will leave if server shuts down
        serverDictionary.Remove(clientId);
        BroadcastPlayerLeftServerRpc(clientId);
    }
    [ServerRpc]
    private void BroadcastPlayerLeftServerRpc(ulong oldClientId)
    {
        Debug.Log("Server: Removed player");
        serverDictionary.Remove(oldClientId);
        serverReadyUpDictionary.Remove(oldClientId);

        BroadcastPlayerLeftClientRpc(oldClientId);
    }
    [ClientRpc]
    private void BroadcastPlayerLeftClientRpc(ulong oldClientId)
    {
        if (localClientId != oldClientId)
        {
            Debug.Log("Local: Player: " + oldClientId + " has Left");
            RemovePlayer(oldClientId);
        }
    }

    void RemovePlayer(ulong clientId)
    {
        GameObject oldCard = playerCardDictionary[clientId];
        playerCardDictionary.Remove(clientId);
        playerDictionary.Remove(clientId);
        // Destroy old playercard
        Destroy(oldCard);

        PositionPlayerCards();
    }

    public void ReadyUp()
    {
        BroadcastPlayerReadyUpServerRpc(localClientId);
    }

    [ServerRpc(RequireOwnership=false)]
    private void BroadcastPlayerReadyUpServerRpc(ulong clientId)
    {
        serverReadyUpDictionary[clientId] = !serverReadyUpDictionary[clientId];
        CheckIfPlayersReady();
    }

    private void CheckIfPlayersReady()
    {
        foreach (var kvp in serverReadyUpDictionary)
        {
            if (!kvp.Value)
            {
                TryCancelReadyUp();
                return;
            }
        }
        canMoveToLobby = true;
        StartGame();
    }
    bool canMoveToLobby = false;
    private void TryCancelReadyUp()
    {
        canMoveToLobby = false;
    }

    private void StartGame()
    {
        StartCoroutine(StartGameAfterTime(1.0f));
    }

    IEnumerator StartGameAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (canMoveToLobby)
            NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }
}
