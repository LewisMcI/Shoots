using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    // Local Dictionary associating ClientID's and Player Data
    Dictionary<ulong, PlayerData> playerDictionary = new Dictionary<ulong, PlayerData>();

    // Server Dictionary associating ClientID's and Serialized Player Data
    Dictionary<ulong, string> serverDictionary = new Dictionary<ulong, string>();
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
        BroadcastPlayerJoinClientRpc(newClientId, playerDataJson, SerializeStringArray(players), SerializeUlongArray(clientIds));
    }
    string SerializeStringArray(string[] stringArray)
    {
        return JsonSerializer.SerializeJsonArray(stringArray);
    }
    string[] DeserializeStringArray(string serializedArray)
    {
        return JsonSerializer.DeserializeJsonArray(serializedArray);
    }
    string SerializeUlongArray(ulong[] ulongArray)
    {
        string serializedArray = string.Join(",", ulongArray);
        return serializedArray;
    }
    ulong[] DeserializeUlongArray(string serializedArray)
    {
        string[] stringValues = serializedArray.Split(',');
        ulong[] ulongArray = new ulong[stringValues.Length];

        for (int i = 0; i < stringValues.Length; i++)
        {
            if (ulong.TryParse(stringValues[i], out ulong value))
            {
                ulongArray[i] = value;
            }
            else
            {
                // Handle parsing errors as needed (e.g., throw an exception or use a default value).
                // Here, we set the element to 0 for simplicity.
                ulongArray[i] = 0;
            }
        }

        return ulongArray;

    }
    [ClientRpc]
    private void BroadcastPlayerJoinClientRpc(ulong newClientId, string newPlayerDataJson, string serializedPlayerList, string serializedClientIdList)
    {
        // Player
        if (localClientId == newClientId)
        {
            string[] existingPlayersStringArray = DeserializeStringArray(serializedPlayerList);
            ulong[] existingPlayersClientIdArray = DeserializeUlongArray(serializedClientIdList);
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
    public List<GameObject> OrderGameObjectsByName(GameObject[] gameObjects)
    {
        var orderedList = gameObjects
             .OrderBy(obj =>
                 obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
             .ToList();

        return orderedList;
    }
    void AddPlayerCard(ulong clientId, string playerDataJson)
    {
        PlayerData playerData = Player.Instance.DeserializePlayerData(playerDataJson);
        playerDictionary.Add(clientId, playerData);
        Player.Instance.CreatePlayerCard(playerData);
        PositionPlayerCards();
    }

    private void PositionPlayerCards()
    {
        List<GameObject> playerCards = OrderGameObjectsByName(GameObject.FindGameObjectsWithTag("Player Cards"));
        if (playerCards.Count == 0)
            return;
        Vector3 centerPosition = centerRect.position;
        float totalWidth = 0f;
        float halfTotalWidth = 0f;
        int playerCount = playerCards.Count;
        for (int i = 1; i < playerCount; i++)
        {
            totalWidth += playerCards[i].GetComponent<RectTransform>().rect.width / 100;
        }
        halfTotalWidth = (totalWidth + (spacing * (playerCount - 1))) / 2;
        for (int i = 0; i < playerCount; i++)
        {
            float xOffset = (i * (playerCards[i].GetComponent<RectTransform>().rect.width / 100 + spacing)) - halfTotalWidth;
            Vector3 cardPosition = new Vector3(centerPosition.x + xOffset, centerPosition.y, centerPosition.z);
            playerCards[i].GetComponent<RectTransform>().position = cardPosition;
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
        BroadcastPlayerLeftClientRpc(oldClientId);
    }
    [ClientRpc]
    private void BroadcastPlayerLeftClientRpc(ulong oldClientId)
    {
        if (localClientId != oldClientId)
        {
            Debug.Log("Local: Player: " + oldClientId + " has Left");
            playerDictionary.Remove(oldClientId);
        }
    }
}
