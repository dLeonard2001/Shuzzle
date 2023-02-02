using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissolve_script : MonoBehaviour
{
    public Transform player;
    private Material material;
    private Vector3 parentPosition;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        
    }

    // Update is called once per frame
    void Update()
    {
        // toFrom 
        // from - to
        material.SetFloat("_distanceFromPlayer", (player.position - transform.position).magnitude);
    }
}
