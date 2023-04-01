using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    ///  The player's speed
    /// </summary>
    [SerializeField, Tooltip("The player's speed")]
    private float PlayerSpeed = 2.0f;

    /// <summary>
    ///  How high the player jumps
    /// </summary>
    [SerializeField, Tooltip("How high the player jumps")]
    private float PlayerJumpHeight = 1.0f;

    /// <summary>
    ///  The percentage scale (on y axis) the player is changed when in crouch mode
    /// </summary>
    [SerializeField, Tooltip("The percentage scale (on y axis) the player is changed when in crouch mode")]
    private float PlayerCrouchScale = 0.9f;

    /// <summary>
    ///  Gravity modifier for pulling back down the player
    /// </summary>
    [SerializeField, Tooltip("Gravity modifier for pulling back down the player.")]
    private float Gravity = -9.81f;

    /// <summary>
    ///  A reference to the transform  of the holding
    /// </summary>
    [SerializeField, Tooltip("A reference to the transform of the holding")]
    private Transform Holding;

    /// <summary>
    ///  Max distance the mouse will accept raycasts beforenot pointing the held object
    /// </summary>
    [SerializeField, Tooltip("The maximum distance the mouse will accept raycasts before not pointing the held object")]
    private float MaxMouseDistance = 30.0f;
    
    // Private

    /// <summary>
    ///  The controller of the player's character
    /// </summary>
    private CharacterController _controller;

    /// <summary>
    ///  The velocity of the character as a Vector3
    /// </summary>
    private Vector3 _velocity;

    /// <summary>
    ///  Is the player on the ground?
    /// </summary>
    private bool _grounded;

    /// <summary>
    ///  A reference to Camera.main.transform
    /// </summary>
    private Transform _cameraTransform;

    void Start()
    {
        // Adds the CharacterController component at runtime and manages it in this script
        _controller = gameObject.AddComponent<CharacterController>();
        _controller.minMoveDistance = 0;

        _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        _grounded = _controller.isGrounded;
        if (_grounded && _velocity.y < 0) _velocity.y = 0;
        
        /*
            Using the forward and right vectors of the camera (and projecting them so y=0)
            we can make movement feel more natural where (W) key, for example, will always
            make the player go forward as seen from the character's perspective
        */
        Vector3 forward = Vector3Utils.ProjectHorizontally(_cameraTransform.forward);
        Vector3 right = Vector3Utils.ProjectHorizontally(_cameraTransform.right);
        // Where the player is moving
        Vector3 movement = Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward;

        _controller.Move(movement * Time.deltaTime * PlayerSpeed);

        /*
            This section of code is for aiming the gun where the mouse is.
            If the mouse doesn't intersect with anything, the gun will be aimed wherever the 
            character is walking
        */
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = _cameraTransform.GetComponent<Camera>().ScreenPointToRay(mousePos);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, MaxMouseDistance)) {
            Holding.forward = Vector3Utils.ProjectHorizontally(hit.point - transform.position);
        } else if (movement != Vector3.zero) {
            Holding.forward = movement;
        }

        // If we are moving, orient us towards where we are moving
        if (movement != Vector3.zero) transform.forward = movement;

        // If the jump key is pressed, and on the ground, add some velocity on the y axis
        if (Input.GetButtonDown("Jump") && _grounded)
            _velocity.y += Mathf.Sqrt(-PlayerJumpHeight * Gravity);
        
        // Crouch by scaling the character transform on the y axis
        if (Input.GetButtonDown("Crouch")) {
            transform.localScale = Vector3.Scale(Vector3.one, new Vector3(1, PlayerCrouchScale, 1));
            _controller.height = 2.0f * PlayerCrouchScale;
            _controller.Move(new Vector3(0, -PlayerCrouchScale / 2, 0));
        }
        
        // Un-Crouches by scaling character transform back up
        if (Input.GetButtonUp("Crouch")) {
            transform.localScale = Vector3.one;
            _controller.height = 2.0f;
        }

        // gravity :)
        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}