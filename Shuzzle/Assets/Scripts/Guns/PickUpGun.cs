using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGun : MonoBehaviour
{
    public gunScript gun;
    public Rigidbody rb;
    public CapsuleCollider coll;
    public Transform player, gunContainer, fpsCam;
    public Transform gunTransform;
    private InputManager inputManager;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        inputManager = InputManager.instance();
        if (!equipped)
        {
            gun.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        if (equipped)
        {
            gun.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }
    private void Update()
    {
        Vector3 distaceToPlayer = player.position - transform.position;
        if (!equipped && distaceToPlayer.magnitude <= pickUpRange && inputManager.PickupItem() && !slotFull)
        {
            PickUp();
        }

        if (equipped && inputManager.DropItem()) Drop();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 90,90);
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        rb.isKinematic = true;
        coll.isTrigger = true;

        gun.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;
        
        rb.velocity = GameObject.Find("Player").GetComponent<CharacterController>().velocity;
        
        
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse); 
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);


        gunTransform = gameObject.transform;
        gun.enabled = false;
    }
}