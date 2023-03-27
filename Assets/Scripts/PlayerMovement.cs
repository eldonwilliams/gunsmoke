using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 2.0f;
    public float playerJumpHeight = 1.0f;
    public float playerCrouchScale = 0.9f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool grounded;

    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        controller.minMoveDistance = 0;
    }

    
    void Update()
    {
        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = 0;

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(movement * Time.deltaTime * playerSpeed);

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
            //controller.Move(new Vector3(0, playerCrouchScale, 0));
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
