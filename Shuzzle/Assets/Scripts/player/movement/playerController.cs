using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class playerController : MonoBehaviour
{
    // =============== Notes - To-Do =============
        // 1. Change entire movement from CharacterController -> Rigidbody 
    // ===========================================
    
    [Header("Player Speed")]
    public float walkSpeed;
    public float sprintSpeed;
    public float airSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public float crouchSpeed;
    public float jumpHeight;
    public float wallJumpSpeed;
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
    public float slideTimer;
    private float maxSlideTimer;
    public float wallRunTimer;
    private float maxWallRunTimer;

    // Forces to use when on slopes
    [Header("Slope Forces")]
    public float slopeForceRayLength = 1;
    public float slopeForce;
    
    // Variables used specifically for camera related events
    [Header("Camera Settings")] 
    public float startFOV;
    public float maxFOV;
    private int gunZoom;
    
    // Gameobject references
    [Header("References")]
    public CinemachineVirtualCamera cam;
    public Camera mainCamera;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public LayerMask whatIsClimbable;
    public Transform playerCameraRoot;
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

    // Values to store the origin of transforms
    private float startYScale;

    private bool pause;

    // setup
    private void Start()
    {
        pause = false;
        RB = GetComponent<Rigidbody>();
        inputManager = InputManager.instance();
        
        startYScale = transform.localScale.y;
        currentState = playerState.ground;
        
        maxSlideTimer = slideTimer;
        maxWallRunTimer = wallRunTimer;
        
        cam.m_Lens.FieldOfView = startFOV;
        origin = transform.position;
        readyToJump = false;
    }
    
    // update function to setup variables and input for later use in fixedUpdate
    void Update()
    {
        movement = inputManager.GetPlayerMovement();

        if (inputManager.AimDownSight() && weaponEquipped)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, gunZoom, 20 * Time.deltaTime);
        }
        else
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 20 * Time.deltaTime);
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

        onGround = Physics.Raycast(transform.position, Vector3.down, out groundHit, 
            1.25f, whatIsGround);

        if (onGround)
        {
            wallRunTimer = maxWallRunTimer;
            currentState = playerState.ground;
        }

        if (inputManager.PlayerJumped() && (onGround || rightWall || leftWall))
        {
            readyToJump = true;
            Debug.Log(readyToJump);
        }
        
    }

    private void FixedUpdate()
    {
        playerVelocity = transform.forward * movement.y + transform.right * movement.x;

        // rotate the in fixedUpdate to remove jitter/rigid movement on objects
        transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        
        // Shooting will be possibly whenever
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
                    moveSpeed = jumpHeight + sprintSpeed - walkSpeed;
                    RB.AddForce(new Vector3(RB.velocity.x, jumpHeight, RB.velocity.z/2) * 2, ForceMode.Impulse);
                
                    readyToJump = false;
                    Debug.Log("jumping");
                    currentState = playerState.air;
                }
                else
                {
                    RB.AddForce(playerVelocity * moveSpeed, ForceMode.Impulse);
                }

                break;
            case playerState.air:
                // wallRun
                // wall Jump
                if (rightWall || leftWall && wallRunTimer > 0) // able to wall run
                {
                    Vector3 wallNormal = new Vector3();
                    if (rightWall)
                        wallNormal = rightWallHit.normal;
                    if (leftWall)
                        wallNormal = leftWallHit.normal;
                    
                    if (readyToJump)
                    {
                        WallJump(wallNormal);
                    }
                    else
                    {
                        // RB.velocity = new Vector3(RB.velocity.x, 0f, RB.velocity.z);
                        
                        // Debug.Log(wall);
                        if (wallRunTimer <= 0)
                        {
                            RB.useGravity = true;
                            moveSpeed = airSpeed;
                        }
                        else
                        {
                            RB.useGravity = false;
                            moveSpeed = wallRunSpeed + sprintSpeed - walkSpeed;
                            wallRunTimer -= Time.deltaTime;
                        
                            RB.AddForce(playerVelocity * wallRunSpeed * 4.5f, ForceMode.Acceleration);
                        } 
                    }
                }
                else
                {
                    RB.useGravity = true;
                    RB.AddForce(playerVelocity * airSpeed + Vector3.down, ForceMode.Force);
                }
                
                break;
            // 
        }

        // // player is on a wall, left or right
        // // player is running
        //     // player is sliding
        //     // player is jumping
        // // player is walking
        //     // player is jumping
        //     if (rightWall && wallRunTimer >= 0)
        //     {
        //         wallRunTimer -= Time.deltaTime;
        //         moveSpeed = wallRunSpeed;
        //         RB.useGravity = false;
        //         RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Force);
        //     }else if (leftWall && wallRunTimer >= 0)
        //     {
        //         wallRunTimer -= Time.deltaTime;
        //         moveSpeed = wallRunSpeed;
        //         RB.useGravity = false;
        //         RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Force);
        //     }else if(currentState == playerState.sprinting)
        //     {
        //         // if (sliding && slideTimer >= 0)
        //         // {
        //         //     moveSpeed = slideSpeed;
        //         //     slideTimer -= Time.deltaTime;
        //         //     transform.localScale = new Vector3(transform.localScale.x, startYScale*0.5f, transform.localScale.z);
        //         // }
        //         // else
        //         // {
        //         //     moveSpeed = sprintSpeed;
        //         //     transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        //         // }
        //     }else if (crouching)
        //     {
        //         moveSpeed = crouchSpeed;
        //         transform.localScale = new Vector3(transform.localScale.x, startYScale*0.5f, transform.localScale.z);
        //     }else if (currentState == playerState.walking)
        //     {
        //         moveSpeed = walkSpeed;
        //     }
        //
        //     if (!rightWall && !leftWall || wallRunTimer <= 0)
        //     {
        //         RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Force); 
        //         RB.useGravity = true;
        //     }
        //
        //     if (readyToJump)
        //     {
        //         if (rightWall)
        //         {
        //             WallJump();
        //         }else if (leftWall)
        //         {
        //             WallJump();
        //         }else
        //         {
        //             RB.AddForce(new Vector3(RB.velocity.x,jumpHeight, RB.velocity.z), ForceMode.Impulse);
        //             readyToJump = false;
        //         }
        //     }
        //
        
        
        if (RB.velocity.magnitude > moveSpeed)
        {
            // Debug.Log(moveSpeed);
            RB.velocity = Vector3.ClampMagnitude(RB.velocity, moveSpeed);
        }
    }

    private bool OnSlope()
    {
        if (inputManager.PlayerJumped())
            return false;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2))
        {
            // Debug.Log(slopeHit.normal);
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
        Vector3 forceToApply = wallNormal * 15 + transform.forward * 15 + Vector3.up * 20;
        
        Debug.Log(forceToApply);

        RB.velocity = new Vector3(RB.velocity.x, 0f, RB.velocity.z);
        RB.AddForce(forceToApply, ForceMode.Impulse);

        // reset our wall run timer, ONLY if we jumped off of a wall
        wallRunTimer = maxWallRunTimer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("death_trigger"))
        {
            Debug.Log("Player fell off the map");
            transform.position = origin;
        }
    }

    // Debugging 
    private void OnDrawGizmos()
    {
        
    }

    public void setGunZoom(int zoomFOV, bool b)
    {
        gunZoom = zoomFOV;
        weaponEquipped = b;
    }
}
