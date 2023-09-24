using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPCMethods : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject bulletPrefab;
    /* This function can be called using
     * objectWithUsersPhotonView.GetComponent<PhotonView>().RPC("FireBullet", RpcTarget.All, position, direction, speed);
     */
    [PunRPC]
    public void FireBullet(Vector3 pos, Vector3 dir, float speed)
    {
        //GameObject bullet = PhotonNetwork.Instantiate("Bullet", pos, Quaternion.identity);
        GameObject bullet = GameObject.Instantiate(bulletPrefab, pos, Quaternion.identity);

        Bullet bulletComp = bullet.GetComponent<Bullet>();

        bulletComp.Dir = dir;
        bulletComp.Speed = speed;
        bulletComp.Activate();
    }
}
