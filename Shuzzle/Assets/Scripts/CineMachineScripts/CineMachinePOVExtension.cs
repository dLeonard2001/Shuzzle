using UnityEngine;
using Cinemachine;
public class CineMachinePOVExtension : CinemachineExtension
{
    [SerializeField] 
    private float clampAngle = 90f;
    [SerializeField] 
    private float horizontalSpeed = 5f;
    [SerializeField] 
    private float verticalSpeed = 5f;
    [SerializeField] 
    public bool tilt;
        

    private InputManager inputManager;
    private Vector3 startingRotation;
    protected override void Awake()
    {
        inputManager = InputManager.instance();
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (startingRotation == null) 
                    startingRotation = transform.localRotation.eulerAngles;
                Vector2 deltaInput = inputManager.GetMouseDelta();
                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
            }
        }
    }
}
