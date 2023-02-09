using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetector : MonoBehaviour
{
    [SerializeField] private short detectionRadius;
    [SerializeField] private LayerMask isItem;

    

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = detectionRadius;
    }

    private void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Debug.Log(other);
        }
    }
}
