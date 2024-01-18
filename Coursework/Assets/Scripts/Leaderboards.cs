using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboards : MonoBehaviour
{
    bool XIsPressed;
    bool ZIsPressed;
    Dictionary<string, int> playerWinsDictionary = new Dictionary<string, int>();

    void Start()
    {
        XIsPressed = false;
        ZIsPressed = false;


        playerWinsDictionary.Add("Ektor", 0);
        playerWinsDictionary.Add("Lewis", 999);
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.X) && !XIsPressed)
        {
            //to make sure X is only pressed once
            XIsPressed = true;
            sendDataToServerScript();
        }

        if (Input.GetKey(KeyCode.Z) && !ZIsPressed)
        {
            //to make sure X is only pressed once
            ZIsPressed = true;
            StartCoroutine(GetData());
        }

    }

    void sendDataToServerScript()
    {
        StartCoroutine(toDatabase());
    }

    IEnumerator toDatabase()
    {
        int xcount = 0;
        foreach (var entry in playerWinsDictionary)
        {
            WWWForm form = new WWWForm();
            form.AddField("playername", entry.Key);
            form.AddField("wins", entry.Value);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/unityMultiplayerLeaderboard/leaderboard.php", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                xcount++;

                Debug.Log(www.downloadHandler.text);
            }
            www.Dispose();
        }
    }

    string[] theResultString;
    bool getDataSuccess;

    IEnumerator GetData()
    {
        //the delimiter basically to parse the data which in this case we are going to use ‘&’ to distinguish new row / data record
        char[] delimiterChars = { '&' };

        //connect to the php script
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/unityMultiplayerLeaderboard/leaderboardend.php");
        yield return www.SendWebRequest();


        //check if the result is correct. If the result is back, split / parse the information by the delimiter
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            getDataSuccess = false;
        }
        else
        {
            theResultString = www.downloadHandler.text.Split(delimiterChars);
            getDataSuccess = true;
        }
        Debug.Log(getDataSuccess);
        www.Dispose();
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
