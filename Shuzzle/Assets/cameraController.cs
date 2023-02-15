using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    [SerializeField] [Range(10, 100)]private float Sensitivity;
    [SerializeField] private float fov;
    [SerializeField] private Transform player;

    [Header("Wall Run Config")]
    [SerializeField] private float degrees;
    [SerializeField] private float rotateDuration;
    private float rotateTime;
    private bool cr_active;

    private Camera mainCam;
    private Vector2 camMovement;
    private float xRotation;

    private InputManager inputManager;

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        inputManager = InputManager.instance();

        mainCam.fieldOfView = fov;
    }

    private void FixedUpdate()
    {
        camMovement = inputManager.GetMouseDelta();
        
        float x = camMovement.x * Sensitivity * Time.deltaTime; // x input is used for the y-axis for rotation
        float y = camMovement.y * Sensitivity * Time.deltaTime; // y input is used for x-axis for rotation

        xRotation -= y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        // rotates the camera (script should be on camera object)
        transform.localEulerAngles = Vector3.right * xRotation;
        
        // rotates the player on the y-axis
        player.Rotate(Vector3.up * x);
    }

    // TODO
    public void RotateCameraOnWallRun(float num)
    {
        transform.localEulerAngles = Vector3.forward * num;
    }

    
}
