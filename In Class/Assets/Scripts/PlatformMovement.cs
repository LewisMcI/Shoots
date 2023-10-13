using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * 2, 10), transform.position.y, transform.position.z);
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
