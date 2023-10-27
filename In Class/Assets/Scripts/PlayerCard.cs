using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    private string playerName = "Player Name";
    private void Awake()
    {
        ulong id = transform.parent.GetComponent<NetworkObject>().OwnerClientId;
        Player[] players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            ulong playerId = player.GetComponent<NetworkObject>().OwnerClientId;
            if (playerId == id)
            {
                playerName = Player.Instance.playerName;
                GetComponent<TextMeshProUGUI>().text = playerName;
            }
        }
    }
}
