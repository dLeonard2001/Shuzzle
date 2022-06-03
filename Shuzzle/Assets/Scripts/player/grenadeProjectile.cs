using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeProjectile : MonoBehaviour
{
    private bool targetHit;
    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
            return;
        
        targetHit = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        
        // transform.SetParent(collision.transform);
    }
}
