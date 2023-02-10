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
    [SerializeField] private float PickUpRange;
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
        items = new Collider[MaxPickUpSize];
        
        Physics.OverlapSphereNonAlloc(transform.position, DetectionRadius, items, isItem);
        
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
                coll.transform.position += new Vector3(toPlayer.x * Time.fixedDeltaTime, 0f, toPlayer.z * Time.fixedDeltaTime);
            }
            else
            {
                Collect(coll);
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) break;

            toPlayer = transform.position - items[i].transform.position;

            if (toPlayer.magnitude >= PickUpRange)
            {
                items[i].transform.position += new Vector3(toPlayer.x * Time.fixedDeltaTime, 0f, toPlayer.z * Time.fixedDeltaTime);
            }
            else
            {
                Collect(items[i]);
            }

        }
    }

    private static void Collect(Collider item)
    {
        Destroy(item.gameObject);
    }
}
