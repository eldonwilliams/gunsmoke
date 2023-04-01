using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    ///  The player's speed
    /// </summary>
    [SerializeField, Tooltip("The player's speed")]
    private float playerSpeed = 2.0f;

    /// <summary>
    ///  How high the player jumps
    /// </summary>
    [SerializeField, Tooltip("How high the player jumps")]
    private float playerJumpHeight = 1.0f;

    /// <summary>
    ///  The percentage scale (on y axis) the player is changed when in crouch mode
    /// </summary>
    [SerializeField, Tooltip("The percentage scale (on y axis) the player is changed when in crouch mode")]
    private float playerCrouchScale = 0.9f;

    /// <summary>
    ///  Gravity modifier for pulling back down the player
    /// </summary>
    [SerializeField, Tooltip("Gravity modifier for pulling back down the player.")]
    private float gravity = -9.81f;

    /// <summary>
    ///  A reference to the transform  of the holding
    /// </summary>
    [SerializeField, Tooltip("A reference to the transform of the holding")]
    private Transform holding;

    /// <summary>
    ///  Max distance the mouse will accept raycasts beforenot pointing the held object
    /// </summary>
    [SerializeField, Tooltip("The maximum distance the mouse will accept raycasts before not pointing the held object")]
    private float maxMouseDistance = 30.0f;
    
    // Private

    /// <summary>
    ///  The controller of the player's character
    /// </summary>
    private CharacterController controller;

    /// <summary>
    ///  The velocity of the character as a Vector3
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    ///  Is the player on the ground?
    /// </summary>
    private bool grounded;

    /// <summary>
    ///  A reference to Camera.main.transform
    /// </summary>
    private Transform cameraTransform;

    void Start()
    {
        // Adds the CharacterController component at runtime and manages it in this script
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;

        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = 0;
        
        /*
            Using the forward and right vectors of the camera (and projecting them so y=0)
            we can make movement feel more natural where (W) key, for example, will always
            make the player go forward as seen from the character's perspective
        */
        Vector3 forward = Vector3Utils.ProjectHorizontally(cameraTransform.forward);
        Vector3 right = Vector3Utils.ProjectHorizontally(cameraTransform.right);
        // Where the player is moving
        Vector3 movement = Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward;

        controller.Move(movement * Time.deltaTime * playerSpeed);

        /*
            This section of code is for aiming the gun where the mouse is.
            If the mouse doesn't intersect with anything, the gun will be aimed wherever the 
            character is walking
        */
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = cameraTransform.GetComponent<Camera>().ScreenPointToRay(mousePos);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, maxMouseDistance)) {
            holding.forward = Vector3Utils.ProjectHorizontally(hit.point - transform.position);
        } else if (movement != Vector3.zero) {
            holding.forward = movement;
        }

        // If we are moving, orient us towards where we are moving
        if (movement != Vector3.zero) transform.forward = movement;

        // If the jump key is pressed, and on the ground, add some velocity on the y axis
        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y += Mathf.Sqrt(-playerJumpHeight * gravity);
        
        // Crouch by scaling the character transform on the y axis
        if (Input.GetButtonDown("Crouch")) {
            transform.localScale = Vector3.Scale(Vector3.one, new Vector3(1, playerCrouchScale, 1));
            controller.height = 2.0f * playerCrouchScale;
            controller.Move(new Vector3(0, -playerCrouchScale / 2, 0));
        }
        
        // Un-Crouches by scaling character transform back up
        if (Input.GetButtonUp("Crouch")) {
            transform.localScale = Vector3.one;
            controller.height = 2.0f;
        }

        // gravity :)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
