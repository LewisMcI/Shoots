using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileMovement : NetworkBehaviour
{
    [SerializeField] private float projectileSpeed = 3.0f;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        rb.AddForce(transform.right * projectileSpeed);
    }

}
