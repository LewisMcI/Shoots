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
        transform.position = initPos + new Vector3((Mathf.PingPong(Time.time * 2, travelDistance) - (travelDistance / 2.0f)), 0.0f, 0.0f);
    }
/*
    private Transform previousTransform;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        previousTransform = collision.gameObject.transform.parent;
        collision.gameObject.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.transform.parent = previousTransform;
    }*/
}
