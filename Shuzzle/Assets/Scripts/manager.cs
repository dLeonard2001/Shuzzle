using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class manager : MonoBehaviour
{

    public GameObject pauseMenu;
    private InputManager inputManager;
    private bool IsPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        inputManager = InputManager.instance();
    }
    // Update is called once per frame
    void Update()
    {
        if (inputManager.pauseGame() && !IsPaused)
        {
            IsPaused = true;
            pauseMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }else if (inputManager.pauseGame() && IsPaused)
        {
            IsPaused = false;
            pauseMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }
}
