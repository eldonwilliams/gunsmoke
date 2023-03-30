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
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;

        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = 0;
        
        // A horizontally projected version of cameraTransform's forward component
        Vector3 forward = Vector3Utils.ProjectHorizontally(cameraTransform.forward);
        // A horizontally projected version of cameraTransform's right component
        Vector3 right = Vector3Utils.ProjectHorizontally(cameraTransform.right);
        // The movement vector of the character
        Vector3 movement = Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward;

        controller.Move(movement * Time.deltaTime * playerSpeed);

        // The mouses position
        Vector3 mousePos = Input.mousePosition;
        // A ray representing the mouse
        Ray mouseRay = cameraTransform.GetComponent<Camera>().ScreenPointToRay(mousePos);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, maxMouseDistance)) {
            holding.forward = Vector3Utils.ProjectHorizontally(hit.point - transform.position);
        } else if (movement != Vector3.zero) {
            holding.forward = movement;
        }

        if (movement != Vector3.zero) transform.forward = movement;

        if (Input.GetButtonDown("Jump") && grounded)
            velocity.y += Mathf.Sqrt(playerJumpHeight * -3.0f * gravity);
        
        if (Input.GetButton("Crouch") && transform.localScale == Vector3.one) {
            transform.localScale = Vector3.Scale(Vector3.one, new Vector3(1, playerCrouchScale, 1));
            controller.height = 2.0f * playerCrouchScale;
            controller.Move(new Vector3(0, -playerCrouchScale / 2, 0));
        } else if (!Input.GetButton("Crouch") && transform.localScale != Vector3.one) {
            transform.localScale = Vector3.one;
            controller.height = 2.0f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
