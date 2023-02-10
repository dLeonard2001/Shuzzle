using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

public class ItemDetector : MonoBehaviour
{
    [Header("Item Pickup Config")]
    [SerializeField] 
    private float PickUpRange;
    [SerializeField] [Range(0.25f, 10f)] [Tooltip("0 is the weakness/slowest pull strength, 10 is the strongest/fastest pull strength")]
    private float pullStrength;  
    [SerializeField] private short DetectionRadius;
    [SerializeField] private int MaxPickUpSize;

    [SerializeField] private LayerMask isItem;

    private Collider[] items;


    private void Awake()
    {
        items = new Collider[MaxPickUpSize];
    }
    
    private void Update()
    {
        // clear out any items from before
        items = new Collider[MaxPickUpSize];
        
        // check for items nearby
        Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, items, isItem);
        
        // start pulling the items to the player
        MoveItemTowardsPlayer();
    }

    private void MoveItemTowardsPlayer()
    {
        // find the path to our player
        Vector3 toPlayer = Vector3.zero;

        foreach (var coll in items)
        {
            if (coll == null) break;

            toPlayer = transform.position - coll.transform.position;

            if (toPlayer.magnitude >= PickUpRange)
            {
                coll.transform.position += 
                    new Vector3(toPlayer.x * pullStrength * Time.fixedDeltaTime, 0f, toPlayer.z * pullStrength *Time.fixedDeltaTime);
            }
            else
            {
                Collect(coll);
            }
        }
    }

    private static void Collect(Collider item)
    {
        Destroy(item.gameObject);
    }
}
