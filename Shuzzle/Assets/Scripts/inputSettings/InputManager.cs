using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance;
    private PlayerControls _playerControls;
    
    public static InputManager instance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        _playerControls = new PlayerControls();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return _playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return _playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumped()
    {
        return _playerControls.Player.Jump.WasPressedThisFrame();
    }
    
    public bool DropItem()
    {
        return _playerControls.Player.Drop.WasPressedThisFrame();
    }
    
    public bool PickupItem()
    {
        return _playerControls.Player.Pickup.WasPressedThisFrame();
    }

    public bool Reload()
    {
        return _playerControls.Player.Reload.WasPressedThisFrame();
    }
    
    public bool Automatic()
    {
        return _playerControls.Player.Shoot.IsPressed();
    }
    public bool NotAutomatic()
    {
        return _playerControls.Player.Shoot.WasPressedThisFrame();
    }
}
