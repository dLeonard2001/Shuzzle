using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{

    [Header("Block Config")] 
    public int blockNumber;
    private bool alreadyHit;

    [Header("References")] 
    public puzzleManager puzzleManager;

    private void Start()
    {
        alreadyHit = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (alreadyHit)
        {
            Debug.Log("Already hit");
        }
        else
        {
            Debug.Log("Block: " + blockNumber + " hit");
            puzzleManager.PlayerInput(blockNumber);
        }
    }

    public void ResetBlock()
    {
        alreadyHit = false;
    }
}
