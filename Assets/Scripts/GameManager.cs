using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    [SerializeField] Vector3[] spawnPoints;
    private void Awake()
    {
        Vector3 randomPosition = spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber -1];
        GameObject playerInstantiated = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
