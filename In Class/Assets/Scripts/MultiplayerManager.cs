using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiPlayerManager : NetworkBehaviour
{
    public GameObject playerCard;
    public RectTransform centerRect;
    private float spacing = 2f;

    private List<GameObject> playerCards = new List<GameObject>();
    public enum ButtonType
    {
        Host,
        Client,
        Server,
        Quit
    } 
    public void ButtonPress(int intButtonType)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Connected;
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
                    AddPlayer();
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
        else
        {
            StatusLabels();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            AddPlayer();
    }
    void AddPlayer()
    {
        Debug.Log("Add New Player");
        // Create new Player Card
        GameObject newPlayerCard = (GameObject)Instantiate(playerCard, Vector3.zero, Quaternion.identity);
        playerCards.Add(newPlayerCard);
        // Position
        PositionPlayerCards();
    }

    private void PositionPlayerCards()
    {
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
    private void Connected(ulong clientId)
    {
        Debug.Log("Connected To Server");
        //NetworkManager.SceneManager.LoadScene("Lobby Screen", UnityEngine.SceneManagement.LoadSceneMode.Single);

        //GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }

    static void StatusLabels()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.EndArea();
    }
}
