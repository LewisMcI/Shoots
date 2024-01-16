using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private void Awake()
    {
        if (IsServer)
            return;
        GetComponent<Rigidbody2D>().AddForce(new Vector3(1000.0f, 0));
    }

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
        }
        Debug.Log("Hit " + collision.name);
        Destroy(gameObject);
    }
}
