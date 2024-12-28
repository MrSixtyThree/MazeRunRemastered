using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 Written by: Declan Greenwell
25/09/2023
 */

//
//[RequireComponent(typeof(CharacterController))] // Dependency for Character Controller
[RequireComponent(typeof(NavMeshAgent))] // Dependency for Character Controller

[AddComponentMenu("Movement Script")]  // Add menu component
public class PlayerMove : MonoBehaviour
{
    // Public Variables
    public float movementSpeed = 5.0f; // Movement Speed of Player
    public float gravity = -9.8f;
    public float terminalVelocity = -20.0f;
    public float jumpSpeed = 15.0f; // Jump Speed of Player
    public bool enableJumping = true; // Bool to control if jumping is enabled or not
    public float groundCheckDistance = 1f;

    // Private Variables & References
    private float verticalSpeed; // Variable of players speed vertically
    private bool doubleJump; // Boolean to determine if player has double jumped or not
    //private CharacterController charController; // Reference to players character controller component
    private NavMeshAgent navMeshAgent;
    public UserData activePlayer;

    // Functions
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get reference for char controller
        activePlayer = GameObject.Find("Menu Controller").GetComponent<UIController>().activePlayer;
        movementSpeed *= activePlayer.getMovementSpeed();
    }


    void Update() // Responsible for updating player location every frame as it moves
    {
        // Input for X axis
        float deltaX = Input.GetAxis("Horizontal") * movementSpeed;

        // Input for Z Axis
        float deltaZ = Input.GetAxis("Vertical") * movementSpeed;

        // Input for Y Axis (Jumping)
        /*if (enableJumping)
        {
            if (Input.GetButtonDown("Jump") && navMeshAgent.isGrounded) // First jump, player is grounded and attempting to jump
            {
                doubleJump = false;
                verticalSpeed = jumpSpeed;
            }
            else if (Input.GetButtonDown("Jump") && !charController.isGrounded && doubleJump) // Already double jumped and attempting to jump again
            {
                verticalSpeed += gravity * 5 * Time.deltaTime;

                if (verticalSpeed < terminalVelocity)
                {
                    verticalSpeed = terminalVelocity;
                }
            }
            else if (Input.GetButtonDown("Jump") && !charController.isGrounded && !doubleJump) // Double jump
            {
                verticalSpeed = jumpSpeed;
                doubleJump = true;
            }
            else if (!charController.isGrounded) // If no input and character not on the ground, bring to ground
            {
                verticalSpeed += gravity * 5 * Time.deltaTime;

                if (verticalSpeed < terminalVelocity)
                {
                    verticalSpeed = terminalVelocity;
                }
            }
        }
        else
        {
            verticalSpeed = gravity;
        }
        */
        //verticalSpeed = gravity;


        // Vector3 for players new position based on inputs and clamp magnitude for diagonal movement
        Vector3 movement = new Vector3(deltaX, 1.0f, deltaZ);
        movement = Vector3.ClampMagnitude(movement, movementSpeed);

        // Ensure movement is indepedent of framerate
        movement *= Time.deltaTime;

        // Transform from local space to global space
        movement = transform.TransformDirection(movement);

        // Apply calculated position to character
        checkGround();
        navMeshAgent.Move(movement);
        

    }

    void checkGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }

}
