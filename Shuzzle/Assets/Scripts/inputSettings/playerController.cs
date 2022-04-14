using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class playerController : MonoBehaviour
{
    [Header("Player Stats")] 
    public int health;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public float wallRunTimer;
    public float jumpHeight;
    public float crouchSpeed;
    public float slideTimer;
    private float maxWallRunTimer;
    private float maxSlideTimer;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private RaycastHit rightWallHit;
    private RaycastHit leftWallHit;
    private bool rightWall;
    private bool leftWall;
    
    private float startYScale;
    private float crouchYScale;
    private float minJumpHeight;
    
    

    private float playerSpeed;
    private float gravityValue = -9.81f;
    
    
    [Header("References")]
    public CinemachineVirtualCamera cam;
    public CineMachinePOVExtension cmScript;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public Transform playerCameraRoot;
    private CharacterController controller;
    private InputManager _inputManager;
    private Transform cameraTransform;
    private Rigidbody rb;

    private Vector3 playerVelocity;
    private Vector3 movement;
    private Vector3 move;
    
    private bool groundedPlayer;

    private bool walking;
    private bool sliding;
    private bool sprinting;
    private bool crouching;

    private float jumpTimer;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _inputManager = InputManager.instance();
        cameraTransform = Camera.main.transform;
        startYScale = transform.localScale.y;
        crouchYScale = transform.localScale.y * 0.5f;
        walking = true;
        minJumpHeight = 0.2f;
        maxSlideTimer = slideTimer;
        maxWallRunTimer = wallRunTimer;
    }
    
    // private IEnumerator SmoothlyLerpMoveSpeed()
    // {
    //     float time = 0;
    //     float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
    //     float startValue = moveSpeed;
    //
    //     while (time < difference)
    //     {
    //         moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
    //         if (OnSlope())
    //         {
    //             float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
    //             float slopeAngleIncrease = 1 + (slopeAngle / 90f);
    //
    //             time += Time.deltaTime * speedIncreasemultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
    //         }
    //         else
    //         {
    //             time += Time.deltaTime * speedIncreasemultiplier;
    //         }
    //         yield return null;
    //     }
    //     moveSpeed = desiredMoveSpeed;
    // }

    void Update()
    {
        movement = _inputManager.GetPlayerMovement();
        move = new Vector3();

        groundedPlayer = controller.isGrounded;
        Debug.DrawRay(playerCameraRoot.position, Vector3.down, Color.red);
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            wallRunTimer = maxWallRunTimer;
        }
        
        rightWall = Physics.Raycast(playerCameraRoot.position, Camera.main.transform.right, 
            out rightWallHit, 1, whatIsWall);
        leftWall = Physics.Raycast(playerCameraRoot.position, -Camera.main.transform.right, 
            out leftWallHit, 1, whatIsWall);
        
        if (rightWall && !groundedPlayer)
        {
            if (wallRunTimer <= 0f)
            {
                rightWall = false;
                playerVelocity.y += Physics.gravity.y * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
                return;
            }
            playerSpeed = wallRunSpeed;
            if (cam.m_Lens.FieldOfView < 90)
                cam.m_Lens.FieldOfView += 0.5f;
            wallRunTimer -= Time.deltaTime;
        }
        else if (leftWall && !groundedPlayer)
        {
            if (wallRunTimer <= 0f)
            {
                leftWall = false;
                playerVelocity.y += Physics.gravity.y * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
                return;
            }
            playerSpeed = wallRunSpeed;
            if (cam.m_Lens.FieldOfView < 90)
                cam.m_Lens.FieldOfView += 0.5f;
            wallRunTimer -= Time.deltaTime;
        }
        else
        {
            cam.m_Lens.FieldOfView = 80;
        }
        
        if (walking && !rightWall && !leftWall)
        {
            playerSpeed = walkSpeed;
        }
        else if (sprinting && !sliding && !rightWall && !leftWall)
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
            if (slideTimer <= 0f)
            {
                sliding = false;
                sprinting = true;
                Debug.Log("Player should stop sliding");
                slideTimer = maxSlideTimer;
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                cam.m_Lens.FieldOfView = 80;
                return;
            }

            cam.m_Lens.FieldOfView = 90;
            slideTimer -= Time.deltaTime;
            playerSpeed = slideSpeed;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
        move.x = movement.x * playerSpeed;
        move.y = 0f;
        move.z = movement.y * playerSpeed;
        
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        controller.Move(move * Time.deltaTime);
        
        // if (move != Vector3.zero)
        // {
        //     gameObject.transform.forward = move;
        // }

        // Changes the height position of the player..
        if (_inputManager.PlayerJumped() && groundedPlayer)
        {
            playerVelocity.y += jumpHeight;

        }

        if (!rightWall && !leftWall)
        {
            playerVelocity.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
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
        }else if (context.canceled)
        {
            walking = true;
            crouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // Sliding
    // Must be sprinting to slide ( checked in Update() )
    public void PlayerSlide(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            walking = false;
            sliding = true;
        }else if (context.canceled)
        {
            if (!sprinting)
            {
                cam.m_Lens.FieldOfView = 80;
                walking = true;
            }
            cam.m_Lens.FieldOfView = 80;
            sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }
    

    #endregion
}
