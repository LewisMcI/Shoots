using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [SerializeField]
    PlayerHealth playerHealth;
    [SerializeField]
    PlayerController playerController;

    ulong clientId;

    public void SetClientId(ulong clientId)
    {
        this.clientId = clientId;
    }

    public void TakeDamage(int damage)
    {
        if (playerHealth != null) { playerHealth.DealDamage(damage); }
        else { throw new Exception("No Player Health script found for Player " + clientId); }
    }
}
