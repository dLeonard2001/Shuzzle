using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class playerCamera : MonoBehaviour
{

    public float sensX;
    public float sensY;
    public Transform mainCam;
    public CinemachineVirtualCamera cam;
    
    private float xRotation;
    private float yRotation;

    private float mouseX;
    private float mouseY;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX; 
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;
        }
            
        // float mouseY= Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        mainCam.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
