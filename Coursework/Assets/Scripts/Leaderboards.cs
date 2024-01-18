using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static Leaderboards;

public class Leaderboards : MonoBehaviour
{
    [System.Serializable]
    public class PlayerWins
    {
        public string player_name;
        public int wins;
    }

    [System.Serializable]
    public class PlayerWinsList
    {
        public PlayerWins[] players;
    }
    List<string> playerNames = new List<string>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PlayerManager.instance.OnPlayerAdded += AddPlayer;
        PlayerManager.instance.OnPlayerRemoved += RemovePlayer;
    }

    private void AddPlayer(ulong clientId)
    {
        playerNames.Add(PlayerManager.instance.GetPlayerData(clientId).name);
        StartCoroutine(GetData(playerNames.ToArray()));
    }

    private void RemovePlayer(ulong clientId)
    {
        playerNames.Remove(PlayerManager.instance.GetPlayerData(clientId).name);
        StartCoroutine(GetData(playerNames.ToArray()));
    }
    [ServerRpc]
    public void AddWins(string name, int wins)
    {
        StartCoroutine(AddWinsToDB(name, wins));
    }
    IEnumerator AddWinsToDB(string name, int numOfWins)
    {
        // Prepare the form data
        WWWForm form = new WWWForm();
        form.AddField("player_name", name);
        form.AddField("wins_to_add", numOfWins);

        string githubRawURL = "http://localhost/unityMultiplayerLeaderboard/leaderboardaddwins.php";

        using (UnityWebRequest www = UnityWebRequest.Post(githubRawURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Error: " + www.error);
            else
                Debug.Log("Update successful: " + www.downloadHandler.text);

            www.Dispose();
        }
    }

    List<string> theResultString = new List<string>();
    bool getDataSuccess;

    IEnumerator GetData(string[] playerNames)
    {
        string playerNamesString = string.Join(",", playerNames);

        // Create the URL with the player names as a parameter
        string urlWithParameters = $"{"http://localhost/unityMultiplayerLeaderboard/leaderboardgetwins.php"}?player_names={playerNamesString}";

        // Create a UnityWebRequest to send a GET request to the PHP script
        UnityWebRequest www = UnityWebRequest.Get(urlWithParameters);

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error: {www.error}");
        }
        else
        {
            // Parse the JSON response
            string jsonResponse = www.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);
            // Use JsonUtility to parse the JSON array into a PlayerDataList object
            PlayerWinsList playerDataList = JsonUtility.FromJson<PlayerWinsList>("{\"players\":" + jsonResponse + "}");

            // Access the array of PlayerData objects
            PlayerWins[] playerDataArray = playerDataList.players;
            
            theResultString = new List<string>();

            // Display the parsed data
            foreach (PlayerWins playerData in playerDataArray)
            {
                theResultString.Add("Player " + playerData.player_name + " has " + playerData.wins + " wins.");

                Debug.Log("Player Name: " + playerData.player_name);
                Debug.Log("Wins: " + playerData.wins);
            }
            getDataSuccess = true;
            Debug.Log("Success");
            // You can then use JSONUtility or other JSON parsing libraries to convert the JSON response into a usable object in Unity
            // Example: YourDataClass data = JsonUtility.FromJson<YourDataClass>(jsonResponse);
        }
    }

    void OnGUI()
    {
        //show the parsed information to screen
        if (getDataSuccess)
        {
            for (int i = 0; i < theResultString.Count - 1; i++)
            {
                Debug.Log("GUI");
                GUI.Label(new Rect(10, 60 + (15 * i + 1), 300, 20), theResultString[i]);
            }
        }
    }
}
