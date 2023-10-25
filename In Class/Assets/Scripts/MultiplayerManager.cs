using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiPlayerManager : NetworkBehaviour
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

    private void Connected(ulong clientId)
    {
        Debug.Log("Connected To Server");
        NetworkManager.SceneManager.LoadScene("Lobby Screen", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
