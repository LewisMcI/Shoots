using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed;
    Vector3 dir;
    bool active = false;
    Rigidbody rb;

    public float Speed { set => speed = value; }
    public Vector3 Dir { set => dir = value; }

    private void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // Calculate the rotation quaternion to align the up axis with the direction of travel.
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);

        // Apply the rotation to the cylinder's transform.
        transform.rotation = rotation;
    }

    public void Activate()
    {
        active = true;
        previousPosition = transform.position;
        rb.AddForce(dir * speed, ForceMode.VelocityChange);
    }

    Vector3 previousPosition;
    private void Update()
    {
        if (!active)
            return;

        // Check if there's a non-zero velocity.
        if (rb.velocity != Vector3.zero)
        {
            // Calculate the direction of travel based on the velocity.
            Vector3 direction = rb.velocity.normalized;

            // Calculate the rotation quaternion to align the up axis with the direction of travel.
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Apply the rotation to the cylinder's transform.
            transform.rotation = rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        Destroy(gameObject);
    }
}
