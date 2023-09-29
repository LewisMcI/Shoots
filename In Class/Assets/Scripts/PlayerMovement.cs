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
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isMoving", true);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isJump", true);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isCrouch", true);
        }
        else
            movementAnimator.SetBool("isCrouch", false);
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            movementAnimator.SetBool("isJump", false);
        }
    }

}
