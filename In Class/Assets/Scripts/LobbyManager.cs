using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
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

    private List<ulong> connectedClientIds = new List<ulong>();
    ulong localClientId = 9999;
    /* OnPlayerJoined
     * Called within the connected client and the server
     */
    private void OnPlayerJoined(ulong clientId)
    {
        
        if (localClientId == 9999)
            localClientId = NetworkManager.Singleton.LocalClientId;
        connectedClientIds.Add(clientId);
        // Is Owner of Player
        if ((IsServer && clientId == localClientId) || (!IsServer && IsClient))
        {
            Debug.Log("Player: " + clientId + " has Joined");
            string playerName = Player.Instance.playerName;
            Debug.Log("New Player Name: " + playerName);
            BroadcastPlayerJoinServerRpc(clientId, playerName);
        }
        // Is Server
        else
        {
            Debug.Log("Server: New Client has Joined");
        }
    }
    void AddPlayerCard(string playerName)
    {
        Player.Instance.CreatePlayerCard(playerName);
        PositionPlayerCards();
    }

    [ServerRpc(RequireOwnership=false)]
    private void BroadcastPlayerJoinServerRpc(ulong newClientId, string playerName)
    {
        BroadcastPlayerJoinClientRpc(newClientId, playerName);
    }
    [ClientRpc]
    private void BroadcastPlayerJoinClientRpc(ulong newClientId, string playerName)
    {
        Debug.Log("Broadcasting Player Join with Name: " + playerName);
        AddPlayerCard(playerName);

        // Everyone BUT the new player
        if (localClientId != newClientId)
        {
            Debug.Log("Broadcast: Player: " + newClientId + " has Joined");
            string targetPlayerName = Player.Instance.playerName;
            SendLateJoinerClientIDServerRpc(newClientId, localClientId, targetPlayerName);
        }
    }
    [ServerRpc(RequireOwnership=false)]
    private void SendLateJoinerClientIDServerRpc(ulong lateJoinerClientId, ulong existingClientId, string playerName)
    {
        SendLateJoinerClientIDClientRpc(lateJoinerClientId, existingClientId, playerName);
    }
    [ClientRpc]
    private void SendLateJoinerClientIDClientRpc(ulong lateJoinerClientId, ulong existingClientId, string playerName)
    {
        if (localClientId == lateJoinerClientId)
        {
            AddPlayerCard(playerName);
            Debug.Log("Added Player: " + existingClientId + " to late joiner");
        }
    }
    public RectTransform centerRect;
    private float spacing = 2f;
    private void PositionPlayerCards()
    {
        List<GameObject> playerCards = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player Cards"));
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
        connectedClientIds.Remove(clientId);
        BroadcastPlayerLeftClientRpc(clientId);
    }

    [ClientRpc]
    private void BroadcastPlayerLeftClientRpc(ulong newClientId)
    {
        if (localClientId != newClientId)
        {
            Debug.Log("Broadcast: Player: " + newClientId + " has Left");
        }
    }
}
