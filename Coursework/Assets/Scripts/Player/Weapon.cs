using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    GameObject projectile;
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Check if can shoot bullet
            ShootBullet(inputPos);
        }
    }

    private void ShootBullet(Vector3 mousePos)
    {
        // Start Anim
        // Ask server to shoot bullet
        ShootBulletServerRPC(NetworkManager.Singleton.LocalClientId, mousePos);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootBulletServerRPC(ulong clientId, Vector3 mousePos)
    {
        GameObject bullet = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);

        mousePos.z = 0;

        // Calculate the direction to move based on the mouse position
        Vector3 moveDirection = (mousePos - bullet.transform.position).normalized;

        // Apply force in the calculated direction
        bullet.GetComponent<Rigidbody2D>().AddForce(moveDirection * 1000.0f);
        AudioManager.instance.PlaySoundToAll("Shoot");
    }
}
