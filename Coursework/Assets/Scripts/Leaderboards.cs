using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboards : MonoBehaviour
{
    [ServerRpc]
    void AddWins(string name, int wins)
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
        }
    }
}
