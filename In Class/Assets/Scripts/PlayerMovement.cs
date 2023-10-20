using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Transform spawnObjectTransform; 
    Transform spawnedObjectTransform;
    public float speed = 5f;

    private Animator movementAnimator;
    private NetworkVariable<int> theScoreForEachPlayer = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private GameObject[] thePlayers;
    public override void OnNetworkSpawn()
    {
        movementAnimator = this.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!IsOwner)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            createBulletShotFromClientServerRpc(transform.position.x, transform.position.y, transform.position.z, transform.rotation);
        }
        movementAnimator.SetBool("isMoving", false);

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isMoving", true);
            HandleMovementServerRpc(1, this.NetworkObjectId);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isMoving", true);
            HandleMovementServerRpc(2, this.NetworkObjectId);

        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isJump", true);
            HandleMovementServerRpc(3, this.NetworkObjectId);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isCrouch", true);
            HandleMovementServerRpc(4, this.NetworkObjectId);
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

    void OnGUI()
    {
        thePlayers = GameObject.FindGameObjectsWithTag("Player");
        int x = 0;

        for (int i = 0; i < thePlayers.Length; i++)
        {
            GUI.Label(new Rect(10, 60 + (15 * x), 300, 20), "Player " +
                (i + 1) + "'s Score: " +
                thePlayers[i].GetComponent<PlayerMovement>().theScoreForEachPlayer.Value);
            x++;
        }
    }


    void OnCollisionEnter2D(Collision2D target)
    {
        if (target.gameObject.layer.Equals(LayerMask.NameToLayer("Grounds")) == true)
        {
            movementAnimator.SetBool("isJump", false);
        }

        if (target.gameObject.tag.Equals("Mushrooms") == true)
        {
            if (!IsOwner) return;
            theScoreForEachPlayer.Value = theScoreForEachPlayer.Value + 1;
            Destroy(target.gameObject);
        }
    }

}
