using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayers : NetworkBehaviour
{
    public GameObject playerPrefab;
    private void Start()
    {
        if (IsServer)
        {
            int playerCount = PlayerManager.instance.GetPlayerCount();
            for (int i = 0; i < playerCount; i++)
            {
                ulong clientId = PlayerManager.instance.GetPlayerId(i);

                GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
                player.name = "Player " + clientId;
                player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
            }
        }
    }
}
