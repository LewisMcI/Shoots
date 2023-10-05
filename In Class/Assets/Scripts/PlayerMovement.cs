using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    private Animator movementAnimator;

    public override void OnNetworkSpawn()
    {
        movementAnimator = this.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

        movementAnimator.SetBool("isMoving", false);

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isMoving", true);
            HandleMovementServerRpc(1, this.NetworkObjectId);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isMoving", true);
            HandleMovementServerRpc(2, this.NetworkObjectId);

        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isJump", true);
            HandleMovementServerRpc(3, this.NetworkObjectId);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isCrouch", true);
            HandleMovementServerRpc(4, this.NetworkObjectId);
        }
        else
            movementAnimator.SetBool("isCrouch", false);
    }

    [ServerRpc]
    void HandleMovementServerRpc(int movementdirection, ulong theIDOftheCharacterThatMoves)
    {
        Debug.Log("the player " + theIDOftheCharacterThatMoves + " just moves from position " + NetworkManager.Singleton.ConnectedClients[0].PlayerObject.transform.position);
        HandleMovementClientRpc(movementdirection);
    }

    [ClientRpc]
    void HandleMovementClientRpc(int movementDirection)
    {
        switch (movementDirection)
        {
            case 1:
                transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
                break;
            case 2:
                transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
                break;
            case 3:
                transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
                movementAnimator.SetBool("isJump", true);
                break;
            case 4:
                transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
                break;

        }
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            movementAnimator.SetBool("isJump", false);
        }
    }

}
