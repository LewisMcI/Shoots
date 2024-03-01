using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField]
    // Will move left and right by this distance
    float travelDistance;
    Vector3 initPos;

    private void Awake()
    {
        initPos = transform.position;
    }
    void Update()
    {
        transform.position = initPos + new Vector3(0.0f, (Mathf.PingPong(Time.time * 2, travelDistance) - (travelDistance / 2.0f)), 0.0f);
    }

    // Dictionary holding all objects on the platform and their previous parent.
    Dictionary<GameObject, Transform> objectsOnPlatform = new Dictionary<GameObject, Transform>();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Transform collidedTransform = collision.transform;

        // Check if the collided object is above the collider
        if (collidedTransform.position.y > transform.position.y)
        {
            objectsOnPlatform.Add(collision.gameObject, collidedTransform.parent);
            collidedTransform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!objectsOnPlatform.ContainsKey(collision.gameObject))
            return;
        collision.transform.parent = objectsOnPlatform[collision.gameObject];
        objectsOnPlatform.Remove(collision.gameObject);
    }
}
