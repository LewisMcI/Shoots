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
            // Check if can shoot bullet
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        // Start Anim
        // Ask server to shoot bullet
        ShootBulletServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc (RequireOwnership=false)]
    private void ShootBulletServerRPC(ulong clientId)
    {
        // Check if can shoot bullet
        // Spawn bullet for all players
        GameObject bullet = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);
        AudioManager.instance.PlaySoundToAll("Shoot");
    }

    [ClientRpc]
    private void SpawnBulletClientRPC()
    {
        GameObject bullet = Instantiate(projectile, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
    }
}
