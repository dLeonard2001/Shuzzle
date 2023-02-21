using System;
using System.Collections;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("Player Speed")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float wallJumpSpeed;
    private float lastDesiredMoveSpeed;
    private float playerSpeed; // desiredMoveSpeed
    private float moveSpeed;
    private RaycastHit slopeHit;
    private bool weaponEquipped;

    // Booleans to store what movement state the player is in
    private enum playerState
    {
        ground,
        air,
    }

    private playerState currentState;

    private bool walking;
    private bool sliding;
    private bool sprinting;
    private bool crouching;
    
    // Vectors to store our player's input/direction
    private Vector2 movement;
    private Vector3 playerVelocity;
    
    // Save our starting position, to be able to reset the player position on death/respawn
    private Vector3 origin;
    
    // Use timers to signal when a player should stop doing something
    [Header("Timers")]
    [SerializeField] private float slideTimer;
    private float maxSlideTimer;
    [SerializeField] private float wallRunTimer;
    private float maxWallRunTimer;

    // Forces to use when on slopes
    [Header("Slope Forces")]
    [SerializeField] private float slopeForceRayLength = 1;
    [SerializeField] private float slopeForce;
    
    // Variables used specifically for camera related events
    [Header("Camera Config")]
    [SerializeField] [Range(10, 100)] private float LookSensitivity;
    [SerializeField] private float camRotateDuration;
    [SerializeField] private float camRotateDegrees;
    [SerializeField] private float camRotateSpeed = 1.0f;
    [SerializeField] private float fov;
    [SerializeField] private float startFOV;
    [SerializeField] private float maxFOV;
    private Vector3 wallDirection;
    private Vector2 camMovement;
    private float yRotation;
    private bool wall_cr_active;
    private float camRotateTimer;
    private int gunZoom;
    
    // Gameobject references
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsClimbable;
    [SerializeField] private Transform playerCameraRoot;
    private Rigidbody RB;
    private InputManager inputManager;

    // Wall Hit Data
    private RaycastHit rightWallHit;
    private RaycastHit leftWallHit;
    private RaycastHit groundHit;
    
    // Booleans to check what wall(s) we are touching
    private bool rightWall;
    private bool leftWall;
    private bool onGround;
    private bool readyToJump;

    private bool pause;

    // Debugging 
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.right);
        Gizmos.DrawRay(transform.position, -transform.right);
        
        Gizmos.DrawWireSphere(transform.position, 1.25f);
    }

    // setup
    private void Start()
    {
        pause = false;
        RB = GetComponent<Rigidbody>();
        inputManager = InputManager.instance();
        
        currentState = playerState.ground;
        
        maxSlideTimer = slideTimer;
        maxWallRunTimer = wallRunTimer;
        
        origin = transform.position;
        readyToJump = false;
    }
    
    // update function to setup variables and input for later use in fixedUpdate
    void Update()
    {
        // Random.Range(min, max);
            // Random.Range(inclusive, exclusive);
            // Random.Range(0, 1) = Random.Range(0, 0);
            // exclusive = n -1
            // Random.Range(k, n - 1);

            movement = inputManager.GetPlayerMovement();

        if (inputManager.AimDownSight() && weaponEquipped)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, gunZoom, 20 * Time.deltaTime);
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, startFOV, 20 * Time.deltaTime);
        }

        rightWall = Physics.Raycast(playerCameraRoot.position, mainCamera.transform.right, 
            out rightWallHit, 1, whatIsWall);
        if (!rightWall)
        {
            rightWall = Physics.Raycast(playerCameraRoot.position, mainCamera.transform.forward, 
                out rightWallHit, 1, whatIsWall);
        }
        leftWall = Physics.Raycast(playerCameraRoot.position, -mainCamera.transform.right, 
            out leftWallHit, 1, whatIsWall);
        if (!leftWall)
        {
            leftWall = Physics.Raycast(playerCameraRoot.position, -mainCamera.transform.forward, 
                out leftWallHit, 1, whatIsWall);
        }

        onGround = Physics.CheckSphere(transform.position, 1.25f, whatIsGround);
        
        if (onGround)
        {
            wallRunTimer = maxWallRunTimer;
            currentState = playerState.ground;
        }

        if (inputManager.PlayerJumped() && (onGround || rightWall || leftWall))
        {
            readyToJump = true;

            
        }

        camMovement = inputManager.GetMouseDelta();
    }

    private void FixedUpdate()
    {
        playerVelocity = transform.forward * movement.y + transform.right * movement.x;
        float x = camMovement.x * LookSensitivity * Time.deltaTime;
        float y = camMovement.y * LookSensitivity * Time.deltaTime;
        float t = camRotateTimer / camRotateDuration;

        yRotation -= y;
        yRotation = Mathf.Clamp(yRotation, -90, 90);
        
        mainCamera.transform.localEulerAngles = 
            Vector3.right * yRotation + wallDirection * Mathf.Lerp(0, camRotateDegrees, Mathf.SmoothStep(0, 1, t));
        
        transform.Rotate(Vector3.up * x);

        // Shooting will be possibly in any state
        
        switch (currentState)
        {
            case playerState.ground:
                // sprinting
                    // must sprint to slide OR have enough speed to slide

                if (inputManager.Sprint())
                {
                    moveSpeed = sprintSpeed;
                }else // walking
                {
                    moveSpeed = walkSpeed;
                }
                
                if (readyToJump) // jump
                {
                    moveSpeed = jumpHeight;
                    RB.AddForce( moveSpeed * new Vector3(0f, jumpHeight, 0f).normalized, ForceMode.Impulse);
                
                    readyToJump = false;
                    
                    currentState = playerState.air;
                }
                else
                {
                    RB.AddForce(playerVelocity.normalized * moveSpeed, ForceMode.Impulse);
                }

                break;
            case playerState.air:
                // wallRun
                // wall Jump
                if (rightWall || leftWall && wallRunTimer > 0) // able to wall run
                {
                    camRotateTimer += Time.deltaTime * camRotateSpeed;

                    wallDirection = rightWall ? Vector3.forward : -Vector3.forward;

                    if (readyToJump)
                    {
                        WallJump(rightWall ? rightWallHit.normal : leftWallHit.normal);
                    }
                    else
                    {
                        if (wallRunTimer <= 0)
                        {
                            RB.useGravity = true;
                            moveSpeed = airSpeed;
                        }
                        else
                        {
                            RB.useGravity = false;
                            moveSpeed = wallRunSpeed;
                            wallRunTimer -= Time.deltaTime;
                        
                            RB.AddForce(playerVelocity.normalized * moveSpeed, ForceMode.Impulse);
                        }
                    }
                }
                else
                {
                    RB.useGravity = true;
                    moveSpeed = airSpeed;
                    RB.AddForce(playerVelocity * airSpeed + Vector3.down, ForceMode.Force);
                }

                break;
        }

        if (RB.velocity.magnitude > moveSpeed)
        {
            RB.velocity = Vector3.ClampMagnitude(RB.velocity, moveSpeed);
        }
    }

    private bool OnSlope()
    {
        if (inputManager.PlayerJumped())
            return false;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2))
        {
            
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }
    
    // slope math
    // private Vector3 GetSlopeMoveDirection()
    // {
    //     return Vector3.ProjectOnPlane(move, slopeHit.normal).normalized;
    // }

    private void WallJump(Vector3 wallNormal)
    {
        readyToJump = false;
        
        // our force to apply based on the wall 
        Vector3 forceToApply = wallNormal * 20f + transform.forward * 25f + Vector3.up * 15f;
        
        RB.AddForce(forceToApply, ForceMode.Impulse);

        // reset our wall run timer, ONLY if we jumped off of a wall
        wallRunTimer = maxWallRunTimer;

        StartCoroutine(ResetCameraRotation());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("death_trigger"))
        {
            Debug.Log("Player fell off the map");
            transform.position = origin;
        }
    }

    private IEnumerator ResetCameraRotation()
    {
        wall_cr_active = true;
        
        while (camRotateTimer >= 0)
        {
            camRotateTimer -= Time.deltaTime * camRotateSpeed;

            yield return null;
        }

        camRotateTimer = 0;
        
        wall_cr_active = false;
    }
    
    public void setGunZoom(int zoomFOV, bool b)
    {
        gunZoom = zoomFOV;
        weaponEquipped = b;
    }

    
}
