using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    pistol,
    submachine,
    rifle,
    undefined
}
public abstract class Weapon : MonoBehaviourPunCallbacks
{
    protected string name { get; set; }
    protected WeaponType type { get; set; }
    public int damage { get; set; }
    protected int maxAmmo { get; set; }
    protected int maxClip { get; set; }

    public Weapon()
    {
        name = "Untitled";
        type = WeaponType.undefined;
        maxAmmo = 0;
        maxClip = 0;
        damage = 0;
    }
    public Weapon(string name, WeaponType type, int maxAmmo, int maxClip, int damage)
    {
        this.name = name;
        this.type = type;
        this.maxAmmo = maxAmmo;
        this.maxClip = maxClip;
        this.damage = damage;
    }

    public virtual void Fire()
    {
        Debug.Log("Fire Weapon");
    }


    public override string ToString()
    {
        return type.ToString() + ": " + name;
    }
}

public class MP5 : Weapon
{
    public MP5(string name) : base(name, WeaponType.submachine, 30, 300, 30)
    {
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    public override void Fire()
    {
        base.Fire();
        PlayerMovement.Instance.PhotonView.RPC("FireBullet", RpcTarget.All, transform.position, transform.forward, 25.0f);
    }
}