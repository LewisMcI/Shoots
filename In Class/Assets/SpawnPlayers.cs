using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayers : NetworkBehaviour
{
    public GameObject playerPrefab;
    private void Awake()
    {
        if (IsServer)
        {
            Debug.Log("Is Server");
            GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            player.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
