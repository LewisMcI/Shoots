using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    GameObject myPlayer;

    private void Awake()
    {
        if (IsOwner)
        {
            myPlayer = PlayerManager.instance.GetPlayerController(NetworkManager.LocalClientId);
        }
    }
    void Update()
    {
        if (IsOwner)
        {
            transform.LookAt(myPlayer.transform.position);
        }
    }
}
