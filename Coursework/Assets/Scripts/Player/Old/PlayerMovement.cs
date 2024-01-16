using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 5f;

    [SerializeField]
    private Animator movementAnimator;

    bool isGrounded = false;

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

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

    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            movementAnimator.SetBool("isJump", false);
            isGrounded = true;
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
