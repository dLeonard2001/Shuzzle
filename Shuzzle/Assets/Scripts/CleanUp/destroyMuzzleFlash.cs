using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyMuzzleFlash : MonoBehaviour
{

    public Transform gunContainer;
    // Start is called before the first frame update
    void Start()
    {
        gunContainer = GameObject.Find("GunContainer").transform;
        transform.SetParent(gunContainer);
        Destroy(gameObject, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
