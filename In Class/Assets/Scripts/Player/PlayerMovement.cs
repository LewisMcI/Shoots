using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;
    [SerializeField] private Transform spawnObjectTransform;
    Transform spawnedObjectTransform;

    [SerializeField]
    private Animator movementAnimator;
    private NetworkVariable<int> theScoreForEachPlayer =
    new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    bool isGrounded = false;

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            createBulletShotFromClientServerRpc(transform.position.x, transform.position.y, transform.position.z, transform.rotation);
        }
        movementAnimator.SetBool("isMove", false);

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            movementAnimator.SetBool("isMove", true);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            movementAnimator.SetBool("isMove", true);

        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.W)) && isGrounded)
        {
            transform.position += new Vector3(0f, 3.0f, 0f);
            movementAnimator.SetBool("isJump", true);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isCrouch", true);
        }
        else
            movementAnimator.SetBool("isCrouch", false);

    }
    [ServerRpc]
    private void createBulletShotFromClientServerRpc(float positionx, float positiony, float positionz, Quaternion vector3rotation)
    {
        spawnedObjectTransform = Instantiate(spawnObjectTransform, new Vector3(positionx, positiony, positionz), vector3rotation);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            movementAnimator.SetBool("isJump", false);
            isGrounded = true;
        }

        if (target.gameObject.tag.Equals("Mushrooms") == true)
        {
            if (!IsOwner) return;
            theScoreForEachPlayer.Value = theScoreForEachPlayer.Value + 1;
            Destroy(target.gameObject);
        }
    }
    void OnCollisionExit2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            isGrounded = false;
        }
    }
}
