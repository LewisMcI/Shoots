using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    float damage = 10.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.tag == "Player")
        {
            ulong clientId = GetComponent<NetworkObject>().OwnerClientId;
            ulong otherClientId = collision.GetComponent<NetworkObject>().OwnerClientId;
            if (clientId == otherClientId)
                return;

            PlayerManager.instance.PlayerDealDamage(otherClientId, damage);
        }

        Debug.Log("Hit " + collision.name);
        Destroy(gameObject);
    }
}
