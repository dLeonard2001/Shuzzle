using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class manager : MonoBehaviour
{

    public GameObject pauseMenu;
    public TextMeshProUGUI playerInteraction;
    public Camera cam;
    public int redColor;
    public int greenColor;
    public int blueColor;
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
        

        // Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // RaycastHit hit;
        // Physics.Raycast(ray, out hit);
        //
        // if (hit.transform != null)
        // {
        //     if (hit.transform.CompareTag("weapon") && hit.transform.GetComponent<PickUpGun>().WithinRange())
        //     {
        //         playerInteraction.text = "Press E to pickup " + hit.transform.name;
        //     }
        //     else if (hit.transform.CompareTag("loot_kunai") || hit.transform.CompareTag("loot_grenade"))
        //     {
        //         if (hit.transform.CompareTag("loot_kunai") && hit.transform.GetComponent<Lootables>().grdScript.WithinRange())
        //         {
        //             playerInteraction.text = "Press E to pickup Kunai";
        //         }
        //         else if(hit.transform.CompareTag("loot_grenade") && hit.transform.GetComponent<Lootables>().grdScript.WithinRange())
        //         {
        //             playerInteraction.text = "Press E to pickup Grenade";
        //         }
        //     }else if (false)
        //     {
        //         // puzzle interaction
        //     }
        //     else
        //     {
        //         playerInteraction.text = "";
        //     }
        // }
    }
}
