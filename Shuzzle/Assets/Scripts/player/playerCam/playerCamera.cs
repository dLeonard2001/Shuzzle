using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class playerCamera : MonoBehaviour
{

    public float sensX;
    public float sensY;
    public Transform cineMachineCamTarget;
    public float RotationSpeed = 1.0f;

    private float mouseX;
    private float mouseY;

    private float cineMachinePitch;
    private float rotationVelocity;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.timeScale > 0)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX; 
            mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;
        }
            
        // float mouseY= Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;
        if (mouseX > 0f || mouseY > 0f)
        {
            // Debug.Log(mouseX + ":" + mouseY);
            float deltaTimeMultiplier = 1.0f;
            
            cineMachinePitch += mouseY * RotationSpeed * deltaTimeMultiplier;
            rotationVelocity = mouseX * RotationSpeed * deltaTimeMultiplier;

            cineMachinePitch = Mathf.Clamp(cineMachinePitch, -90f, 90f);
            
            cineMachineCamTarget.transform.localRotation = Quaternion.Euler(cineMachinePitch, 0f, 0f);

            transform.Rotate(Vector3.up * rotationVelocity);
        }
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
