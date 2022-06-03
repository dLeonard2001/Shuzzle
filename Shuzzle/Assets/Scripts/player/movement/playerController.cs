using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(CharacterController))]
public class playerController : MonoBehaviour
{
    // =============== Notes - To-Do =============
    
    // ===========================================
    
    [Header("Player Speed")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public float crouchSpeed;
    public float jumpHeight;
    private float lastDesiredMoveSpeed;
    private float playerSpeed; // desiredMoveSpeed
    private float moveSpeed;
    private RaycastHit slopeHit;
    private bool weaponEquipped;
    
    // Store if our player is on the ground
    private bool groundedPlayer;
    
    // Booleans to store what movement state the player is in
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
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public LayerMask whatIsClimbable;
    public Transform playerCameraRoot;
    private CharacterController controller;
    private InputManager _inputManager;
    private Transform cameraTransform;

    // Wall Hit Data
    private RaycastHit rightWallHit;
    private RaycastHit leftWallHit;
    
    // Booleans to check what wall(s) we are touching
    private bool rightWall;
    private bool leftWall;
    private bool frontWall;

    // Values to store the origin of transforms
    private float startYScale;
    private float crouchYScale;

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
        controller = GetComponent<CharacterController>();
        _inputManager = InputManager.instance();
        cameraTransform = Camera.main.transform;
        startYScale = transform.localScale.y;
        crouchYScale = transform.localScale.y * 0.5f;
        walking = true;
        maxSlideTimer = slideTimer;
        maxWallRunTimer = wallRunTimer;
        cam.m_Lens.FieldOfView = startFOV;
        origin = transform.position;
    }
    void Update()
    {
        if (_inputManager.pauseGame())
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

        if (_inputManager.AimDownSight() && weaponEquipped)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, gunZoom, 20 * Time.deltaTime);
        }
        movement = _inputManager.GetPlayerMovement();
        move = new Vector3();

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            playerVelocity.z = 0f;
            playerVelocity.y = 0f;
            playerVelocity.x = 0f;
            wallRunTimer = maxWallRunTimer;
            if (!sliding && !rightWall && !leftWall && !_inputManager.AimDownSight())
            {
                cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 10 * Time.deltaTime);
            }
        }

        move.x = movement.x;
        move.z = movement.y;
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;

        rightWall = Physics.Raycast(playerCameraRoot.position, Camera.main.transform.right, 
            out rightWallHit, 1, whatIsWall);
        if (!rightWall)
        {
            rightWall = Physics.Raycast(playerCameraRoot.position, Camera.main.transform.forward, 
                out rightWallHit, 1, whatIsWall);
        }
        leftWall = Physics.Raycast(playerCameraRoot.position, -Camera.main.transform.right, 
            out leftWallHit, 1, whatIsWall);
        if (!leftWall)
        {
            leftWall = Physics.Raycast(playerCameraRoot.position, -Camera.main.transform.forward, 
                out leftWallHit, 1, whatIsWall);
        }

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * 20 + wallNormal * 10;

        if (!rightWall && !leftWall && !frontWall)
        {
            playerVelocity.y -= -Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            playerVelocity.y = 0;
        }
        
        // ======= Wall Movement =======
        if (rightWall && !groundedPlayer)
        {
            if (_inputManager.PlayerJumped())
            {
                playerVelocity = new Vector3(move.x, 0f, move.z);
                playerVelocity = forceToApply;
                wallRunTimer = maxWallRunTimer;
            }
            else if (wallRunTimer <= 0f)
            {
                // Debug.Log("Slowly kicking player off of a wall");
                playerVelocity.y += -2;
            }
            else
            {
                if (sprinting)
                {
                    playerVelocity.y += 1.5f;
                }else if (crouching)
                {
                    playerVelocity.y -= 1.5f;
                }
                playerSpeed = wallRunSpeed;
                cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, maxFOV, 10 * Time.deltaTime);
                wallRunTimer -= Time.deltaTime;
            }
        }
        else if (leftWall && !groundedPlayer)
        {
            if (_inputManager.PlayerJumped())
            {
                playerVelocity = new Vector3(move.x, 0f, move.z);
                playerVelocity = forceToApply;
                wallRunTimer = maxWallRunTimer;
            }
            else if (wallRunTimer <= 0f)
            {
                // Debug.Log("Slowly kicking player off of a wall");
                playerVelocity.y += -2;
            }
            else
            {
                if (sprinting)
                {
                    playerVelocity.y += 1.5f;
                }else if (crouching)
                {
                    playerVelocity.y -= 1.5f;
                }
                playerSpeed = wallRunSpeed;
                cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, maxFOV, 10 * Time.deltaTime);
                wallRunTimer -= Time.deltaTime;
            }
        }

        // ======= Ground Movement =======
        if (walking && !rightWall && !leftWall)
        {
            playerSpeed = walkSpeed;
        }
        else if (sprinting && !crouching && !sliding && !rightWall && !leftWall)
        {
            playerSpeed = sprintSpeed;
        }
        else if (crouching && !rightWall && !leftWall)
        {
            playerSpeed = crouchSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
        else if (sliding && sprinting && !rightWall && !leftWall)
        {
            playerVelocity = Vector3.down * 10f;
            if (!OnSlope() || move.y > -0.1f)
            {
                // jumping out of a slide needs some tuning
                if (_inputManager.PlayerJumped())
                {
                    playerVelocity.y = 0;
                    playerVelocity.y = jumpHeight*2;
                    playerSpeed = slideSpeed;
                    sliding = false;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    controller.height = 2;
                    while(cam.m_Lens.FieldOfView > 75)
                    {
                        cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 10 * Time.deltaTime);
                    }
                }else if (slideTimer <= 0f)
                {
                    sliding = false;
                    sprinting = true;
                    // Debug.Log("Player should stop sliding");
                    slideTimer = maxSlideTimer;
                    transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                    cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 10 * Time.deltaTime);
                    controller.height = 2;
                }
                else
                {
                    cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, maxFOV, 10 * Time.deltaTime);
                    slideTimer -= Time.deltaTime;
                    playerSpeed = slideSpeed;
                    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                    controller.height = 1;
                }
            }
            else
            {
                // Debug.Log("Sliding down a slope");
                move += GetSlopeMoveDirection();
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                controller.height = 1;
            }
            
            // // Debug.Log(Mathf.Abs(playerSpeed - lastDesiredMoveSpeed) + " speed difference");
            // if (Mathf.Abs(playerSpeed - lastDesiredMoveSpeed) > 4 && moveSpeed != 0)
            // {
            //     Debug.Log(playerSpeed + " : " + lastDesiredMoveSpeed);
            //     Debug.Log("Starting Movement Coroutine");
            //     StopAllCoroutines();
            //     StartCoroutine(SmoothlyLerpMoveSpeed());
            // }
            // else
            // {
            //     moveSpeed = playerSpeed;
            // }
            // lastDesiredMoveSpeed = playerSpeed;
        }
        // ===============================
        
        move.x *= playerSpeed;
        move.z *= playerSpeed;
        
        
        if (_inputManager.PlayerJumped() && groundedPlayer)
        {
            playerVelocity.y = jumpHeight;
        }

        // if GetSlopeMoveDirection
            // has a -y value, then we are going down a slope
        // else
            // +y value then we are going up a slope
        if (OnSlope())
        {
            move += playerVelocity;
            move += Vector3.down * controller.height / 2 * slopeForce;
            controller.Move(move * Time.deltaTime);
        }
        else
        {
            move += playerVelocity;
            controller.Move(move * Time.deltaTime);
        }
        // Debug.Log("PlayerVelocity is: " + controller.velocity.magnitude);
    }

    private bool OnSlope()
    {
        if (_inputManager.PlayerJumped())
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "death_trigger")
        {
            Debug.Log("Player fell off the map");
            transform.position = origin;
        }
            
    }

    #region MyRegion
    // Sprinting
    public void PlayerSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            walking = false;
            sprinting = true;
        }else if (context.canceled)
        {
            walking = true;
            sprinting = false;
        }
    }
    
    // Crouching
    public void PlayerCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            walking = false;
            crouching = true;
            controller.height = 1;
        }else if (context.canceled)
        {
            walking = true;
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            controller.height = 2;
        }
    }

    // Sliding
    // Must be sprinting to slide ( checked in Update() )
    public void PlayerSlide(InputAction.CallbackContext context)
    {
        if (context.started )
        {
            walking = false;
            sliding = true;
            
        }else if (context.canceled)
        {
            if (!sprinting)
            {
                walking = true;
            }
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, startFOV, 10 * Time.deltaTime);
            sliding = false;
            slideTimer = maxSlideTimer;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            controller.height = 2;
        }
    }

    public void setGunZoom(int zoomFOV, bool b)
    {
        gunZoom = zoomFOV;
        weaponEquipped = b;
    }
    

    #endregion
}
