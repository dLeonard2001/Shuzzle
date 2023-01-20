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
        walking,
        sprinting,
    }

    private playerState currentState;
    
    private bool walking;
    private bool sliding;
    private bool sprinting;
    private bool crouching;
    
    // Vectors to store our player's input/direction
    private Vector2 movement;
    private Vector3 move;
    private Vector3 playerVelocity;
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
    private bool frontWall;
    private bool onGround;
    private bool readyToJump;

    // Values to store the origin of transforms
    private float startYScale;

    private bool pause;

    // private IEnumerator SmoothlyLerpMoveSpeed()
    // {
    //     float time = 0;
    //     float difference = Mathf.Abs(playerSpeed - moveSpeed);
    //     float startValue = moveSpeed;
    //
    //     while (time < difference)
    //     {
    //         moveSpeed = Mathf.Lerp(startValue, playerSpeed, time / difference);
    //         time += Time.deltaTime;
    //         yield return null;
    //     }
    //     moveSpeed = playerSpeed;
    // }

    private void Start()
    {
        pause = false;
        RB = GetComponent<Rigidbody>();
        inputManager = InputManager.instance();
        startYScale = transform.localScale.y;
        currentState = playerState.walking;
        walking = true;
        maxSlideTimer = slideTimer;
        maxWallRunTimer = wallRunTimer;
        cam.m_Lens.FieldOfView = startFOV;
        origin = transform.position;
        readyToJump = false;
    }
    void Update()
    {
        if (inputManager.pauseGame())
        {
            if (pause)
            {
                pause = false;
            }
            else
            {
                pause = true;
            }
        }else if (pause)
        {
            return;
        }
        
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
            2);

        if (onGround)
        {
            wallRunTimer = maxWallRunTimer;
        }

        if (inputManager.PlayerJumped() && (onGround || rightWall || leftWall))
        {
            readyToJump = true;
        }
        
    }

    private void FixedUpdate()
    {
        playerVelocity = transform.forward * movement.y + transform.right * movement.x;
        transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        // player is on a wall, left or right
        // player is running
            // player is sliding
            // player is jumping
        // player is walking
            // player is jumping
            if (rightWall && wallRunTimer >= 0)
            {
                wallRunTimer -= Time.deltaTime;
                moveSpeed = wallRunSpeed;
                RB.useGravity = false;
                RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Acceleration);
            }else if (leftWall && wallRunTimer >= 0)
            {
                wallRunTimer -= Time.deltaTime;
                moveSpeed = wallRunSpeed;
                RB.useGravity = false;
                RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Acceleration);
            }else if(currentState == playerState.sprinting)
            {
                if (sliding && slideTimer >= 0)
                {
                    moveSpeed = slideSpeed;
                    slideTimer -= Time.deltaTime;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale*0.5f, transform.localScale.z);
                }
                else
                {
                    moveSpeed = sprintSpeed;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                }
            }else if (crouching)
            {
                moveSpeed = crouchSpeed;
                transform.localScale = new Vector3(transform.localScale.x, startYScale*0.5f, transform.localScale.z);
            }else if (currentState == playerState.walking)
            {
                moveSpeed = walkSpeed;
            }

            if (!rightWall && !leftWall || wallRunTimer <= 0)
            {
                RB.AddForce(playerVelocity.normalized * (moveSpeed * 5f), ForceMode.Acceleration); 
                RB.useGravity = true;
            }

            if (readyToJump)
            {
                if (rightWall)
                {
                    WallJump();
                }else if (leftWall)
                {
                    WallJump();
                }else
                {
                    RB.AddForce(new Vector3(RB.velocity.x,jumpHeight, RB.velocity.z), ForceMode.Impulse);
                    readyToJump = false;
                }
            }

            Vector3 flatVel = new Vector3(RB.velocity.x, 0f, RB.velocity.z);
            
            if (RB.velocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                RB.velocity = new Vector3(limitedVel.x, RB.velocity.y, limitedVel.z);
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
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(move, slopeHit.normal).normalized;
    }

    private void WallJump()
    {
        readyToJump = false;
        moveSpeed = wallJumpSpeed;
        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * 8 + wallNormal * 25;

        RB.velocity = new Vector3(RB.velocity.x, 0f, RB.velocity.z);
        RB.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("death_trigger"))
        {
            Debug.Log("Player fell off the map");
            transform.position = origin;
        }
    }

    private void stateHandling()
    {
        switch (currentState)
        {
            case playerState.walking:
                break;
        }
    }
    
    
    
    // Debugging 
    private void OnDrawGizmos()
    {
        
    }

    #region MyRegion
    // Sprinting
    public void PlayerSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentState = playerState.sprinting;
        }else if (context.canceled)
        {
            currentState = playerState.walking;
        }
    }
    
    // Crouching
    public void PlayerCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            walking = false;
            crouching = true;

        }else if (context.canceled)
        {
            currentState = playerState.walking;
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // Sliding
    // Must be sprinting to slide ( checked in Update() )
    public void PlayerSlide(InputAction.CallbackContext context)
    {
        if (context.started )
        {
            // currentState = playerState.sliding;
            walking = false;
            sliding = true;
            
        }else if (context.canceled)
        {
            if (!sprinting)
            {
                currentState = playerState.walking;
                walking = true;
            }
            currentState = playerState.sprinting;
            
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 10 * Time.deltaTime);
            sliding = false;
            slideTimer = maxSlideTimer;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    public void setGunZoom(int zoomFOV, bool b)
    {
        gunZoom = zoomFOV;
        weaponEquipped = b;
    }
    

    #endregion
}
