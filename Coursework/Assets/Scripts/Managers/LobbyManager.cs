using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour 
{
    public float startTime = 1.0f;
    public GameObject playerCardPrefab;
    private SpriteLookup spriteLookup;
    // Server Dictionary associating ClientID's and Ready Status
    Dictionary<ulong, bool> serverReadyDictionary = new Dictionary<ulong, bool>();

    // Local Dictionary associating ClientID's and Player Cards
    Dictionary<ulong, GameObject> clientPlayerCards = new Dictionary<ulong, GameObject>();

    bool hosting = false;
    private void Awake()
    {
        spriteLookup = GetComponent<SpriteLookup>();
        if (!spriteLookup)
            throw new Exception("Could not find Sprite Lookup Table");
    }
    public enum ButtonType
    {
        Host,
        Client,
        Server,
        Quit
    }

    public void ButtonPress(int intButtonType)
    { 
        ButtonType buttonType;
        PlayerManager.instance.OnPlayerAdded += PlayerJoined;
        PlayerManager.instance.OnPlayerRemoved += PlayerDisconnected;
        PlayerManager.instance.OnHostRemoved += HostDisconnected;
        if (intButtonType < Enum.GetNames(typeof(ButtonType)).Length)
            buttonType = (ButtonType)intButtonType;
        else
            throw new Exception("Button Type not found");
        switch (buttonType)
        {
            case ButtonType.Host:
                if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
                {
                    NetworkManager.Singleton.StartHost();
                    hosting = true;
                }
                else
                    throw new Exception("Trying to Host new lobby while in lobby.");
                break;
            case ButtonType.Client:
                if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
                    NetworkManager.Singleton.StartClient();
                else
                    throw new Exception("Trying to Join new lobby while in lobby.");
                break;
            case ButtonType.Server:
                if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
                    NetworkManager.Singleton.StartServer();
                else
                    throw new Exception("Trying to Start new Server while server is created.");

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

    void PlayerJoined(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId) 
            FlipReadyStatusServerRpc(clientId);
        AddPlayerCard(clientId, PlayerManager.instance.GetPlayerData(clientId));
    }

    void PlayerDisconnected(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
            RemovePlayerStatusServerRpc(clientId);
        RemovePlayerCard(clientId);
    }
    [SerializeField] Animator cameraAnimator;
    void HostDisconnected()
    {
        // Delete all Player Cards
        foreach (var kvp in clientPlayerCards)
        {
            Destroy(kvp.Value);
            clientPlayerCards.Remove(kvp.Key);
        }
        // Back to Menu
        cameraAnimator.SetTrigger("LobbyToMain");
    }

    [ServerRpc(RequireOwnership=false)]
    private void FlipReadyStatusServerRpc(ulong clientId)
    {
        // Player Just Joined
        if (!serverReadyDictionary.ContainsKey(clientId))
            serverReadyDictionary.Add(clientId, false);
        // Flip Player Status
        else
        {
            serverReadyDictionary[clientId] = !serverReadyDictionary[clientId];
            if (serverReadyDictionary[clientId])
                Debug.Log("Player: " + clientId + " is ready");
            else
                Debug.Log("Player: " + clientId + " is not ready");
        }

        if (CheckIfPlayersReady())
            NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership=false)]
    private void RemovePlayerStatusServerRpc(ulong clientId)
    {
        serverReadyDictionary.Remove(clientId);
        if (CheckIfPlayersReady())
            NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    void AddPlayerCard(ulong clientId, PlayerData playerData)
    {
        GameObject newPlayerCard = CreatePlayerCard(playerData);
        clientPlayerCards.Add(clientId, newPlayerCard);
        PositionPlayerCards();
    }
    void RemovePlayerCard(ulong clientId)
    {
        Destroy(clientPlayerCards[clientId]);
        clientPlayerCards.Remove(clientId);
        PositionPlayerCards();
    }

    public RectTransform centerRect;
    private float spacing = 2f;
    private void PositionPlayerCards()
    {
        int playerCount = PlayerManager.instance.GetPlayerCount() - 1;

        Vector3 centerPosition = centerRect.position;
        if (playerCount == 0)
        {
            clientPlayerCards.Values.First().GetComponent<RectTransform>().position = centerPosition;
            return;
        }
        float totalWidth = 0f;
        float halfTotalWidth = 0f;
        foreach (var kvp in clientPlayerCards)
        {
            totalWidth += kvp.Value.GetComponent<RectTransform>().rect.width / 100;
        }

        halfTotalWidth = (totalWidth + (spacing * (playerCount - 1))) / 2;

        int i = 0;
        foreach (var kvp in clientPlayerCards)
        {
            float xOffset = (i * (kvp.Value.GetComponent<RectTransform>().rect.width / 100 + spacing)) - halfTotalWidth;
            Vector3 cardPosition = new Vector3(centerPosition.x + xOffset, centerPosition.y, centerPosition.z);
            kvp.Value.GetComponent<RectTransform>().position = cardPosition;
            i++;
        }
    }

    public void ReadyUp()
    {
        FlipReadyStatusServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    bool canMoveToLobby = false;
    private bool CheckIfPlayersReady()
    {
        foreach (var kvp in serverReadyDictionary)
        {
            if (!kvp.Value)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator TryStartGameAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        if (CheckIfPlayersReady())
            NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    public GameObject CreatePlayerCard(PlayerData newPlayerData)
    {
        //Debug.Log("Instantiating Player Card with Name: " + newPlayerData.playerName);
        // Create Default Player Card
        GameObject playerCard = Instantiate(playerCardPrefab);
        // Set Player Name
        playerCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newPlayerData.playerName;

        // Get Sprites
        Sprite characterSprite = spriteLookup.GetCharacterSprite(newPlayerData.characterSpriteIndex);
        Sprite accessorySprite = spriteLookup.GetAccessoriesSprite(newPlayerData.accessorySpriteIndex);
        playerCard.transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = characterSprite;
        playerCard.transform.GetChild(1).GetChild(1).GetComponent<SpriteRenderer>().sprite = accessorySprite;

        return playerCard;
    }
}
