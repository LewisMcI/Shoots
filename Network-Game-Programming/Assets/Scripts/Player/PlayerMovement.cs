using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.HighDefinition;

public class PlayerMovement : MonoBehaviour
{
    // Ground Variables - All either set or adjusted in the inspector
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.01f;

    // Variables for movement set in inspector
    public float speed = 2f;
    public float jumpHeight = 5f;

    Vector3 newPosition;
    Vector3 newPosition2;

    // Rigidbody component of this object
    Rigidbody rb;
    // Bool to check if the player is currently touching the floor
    bool isGrounded;

    public GameObject cam;
    public GameObject playerWeapon;

    PhotonView photonView;
    public float x;
    public float z;
    Transform parent;
    public GameObject modelWithRig;
    public static PlayerMovement Instance
    {
        get;
        private set;
    }
    public PhotonView PhotonView { get => photonView; }

    void Awake()
    {
        // Gets the Rigidbody component of this object
        rb = (Rigidbody)GetComponent(typeof(Rigidbody));
        // Gets the View component of this object

        photonView = PhotonView.Get(this);
        if (PhotonView.IsMine)
        {
            cam.GetComponent<CameraController>().enabled = true;
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            cam.GetComponent<HDAdditionalCameraData>().enabled = true;
            playerWeapon.transform.GetChild(0).GetComponent<Weapon>().enabled = true;
        }
        else
        {
            Destroy(this);
        }

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // If the player is pressing the Jump key set in the InputManager settings
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Changes the upwards velocity of the object to the players jumpHeight
            rb.velocity = new Vector3(0, jumpHeight, 0);
        }


        /* Uses InputManager settings for horizontal and vertical, returns a value between -1 and 1 for all forms of input
         * Allows joystick movement to be any number between -1 and 1 depending on how far the joystick has been pushed,
         * For Keyboard movement, in this example, holding down W will slowly rise to 1 and then stay there until released.
         * */
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            // TODO: Walking Anim

            /* Stores inputs as a new Vector3 (Y is zero because we are only dealing with forwards/backwards and sideways currently
        * Not upwards and downwards
            */
            Vector3 rbInput = new Vector3(x, 0, z);

            // Returns the Y Rotation of the object
            float rotation = rb.rotation.eulerAngles.y;
            /* First we find the direction the character is currently facing, we then times it by the input vector,
             * speed and Time.deltaTime
             * 
             * Notes
             * Quaternion * Vector3 = Rotated Vector (can imagine it like literally rotating the new Vector3 in the Quaternion direction
             * Quaternion.Euler returns a rotation around the Y-Axis
             * */
            if (this.transform.parent == null)
            {
                newPosition = Quaternion.Euler(0, rotation, 0) * rbInput * speed * Time.deltaTime + rb.position;
                rb.position = newPosition;
            }
            else
            {
                newPosition2 = Quaternion.Euler(0, rotation, 0) * rbInput + rb.position;
                transform.position = Vector3.MoveTowards(transform.position, newPosition2, speed * Time.deltaTime);
            }
        }
        else
        {
            // Todo: Disable Walking anim
        }
    }
}

