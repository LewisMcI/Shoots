using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboards : MonoBehaviour
{

    private void Awake()
    {
        StartCoroutine(GetData());
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

    string[] theResultString;
    bool getDataSuccess;

    IEnumerator GetData()
    {
        string[] playerNames = { "Lewis" };
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
            // You can then use JSONUtility or other JSON parsing libraries to convert the JSON response into a usable object in Unity
            // Example: YourDataClass data = JsonUtility.FromJson<YourDataClass>(jsonResponse);
        }
    }

    void OnGUI()
    {
        //show the parsed information to screen
        if (getDataSuccess)
        {
            for (int i = 0; i < theResultString.Length - 1; i++)
            {
                GUI.Label(new Rect(10, 60 + (15 * i + 1), 300, 20), theResultString[i]);
            }
        }
    }
}
