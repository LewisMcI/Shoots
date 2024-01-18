using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    float damage = 10.0f;
    private Vector3 mousePosition;

    private void Awake()
    {
        if (IsServer)
            return;

        // Save the mouse position on Awake
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // Calculate the direction to move based on the mouse position
        Vector3 moveDirection = (mousePosition - transform.position).normalized;

        // Apply force in the calculated direction
        GetComponent<Rigidbody2D>().AddForce(moveDirection * 1000.0f);
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

            PlayerManager.instance.PlayerDealDamage(otherClientId, damage);
        }

        Debug.Log("Hit " + collision.name);
        Destroy(gameObject);
    }
}
