using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dissolve_script : MonoBehaviour
{
    public Transform player;
    private Material material;
    public bool useSharedMaterial;

    // Start is called before the first frame update
    void Start()
    {
        if(useSharedMaterial)
            material = GetComponent<MeshRenderer>().sharedMaterial;
        else
            material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_DistanceFromPlayer", player.position.magnitude - transform.position.magnitude);
    }
}
