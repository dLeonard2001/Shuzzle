using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health_pickup : MonoBehaviour
{
    [Header("Pickup Stats")]
    public float health_per_pickup;
    
    [Header("Object Display")]
    public float yawDegreesPerSecond;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 
                yawDegreesPerSecond * Time.deltaTime, 0f));
    }
}
