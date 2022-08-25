using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class Lootables : MonoBehaviour
{
    [Header("Stats")] public float pickUpRange;

    [Header("References")] 
    public Transform playerTransform;
    public Transform gunContainer;
    public Grenade grdScript;
    public gunScript gunScript;
    private Vector3 rangeFromPlayer;
    private int ammoToRestore;
    
    [Header("Loot Type")]
    public bool isGrenade;
    public bool isAmmo;
    public bool isKunai;

    private void Update()
    {
        rangeFromPlayer = playerTransform.position - transform.position;

        if (isAmmo)
        {
            AmmoResupply(rangeFromPlayer);
        }else if (isGrenade && grdScript.IsGrenadeEquipped())
        {
            ThrowableResupply(rangeFromPlayer);
        }else if (isKunai && grdScript.IsKunaiEquipped())
        {
            ThrowableResupply(rangeFromPlayer);
        }
    }

    private void AmmoResupply(Vector3 rangeFromPlayer)
    {
        if (gunContainer.childCount == 1)
        {
            gunScript = gunContainer.GetComponentInChildren<gunScript>();
            ammoToRestore = gunScript.magazineSize;
            if (rangeFromPlayer.magnitude <= pickUpRange && gunScript.ammoSupply < gunScript.GetMaxAmmoSupply())
            {
                if (ammoToRestore + gunScript.ammoSupply > gunScript.GetMaxAmmoSupply())
                {
                    gunScript.ammoSupply = gunScript.GetMaxAmmoSupply();
                    Destroy(gameObject);
                }
                else
                {
                    gunScript.ammoSupply += ammoToRestore;
                    Destroy(gameObject);
                }
            }
            else if(gunScript.ammoSupply == gunScript.GetMaxAmmoSupply() && rangeFromPlayer.magnitude <= pickUpRange)
            {
                Debug.Log("ammo is full");
            }
        }
    }

    private void ThrowableResupply(Vector3 rangeFromPlayer)
    {
        if (rangeFromPlayer.magnitude <= pickUpRange && grdScript.GetThrowableCount() < grdScript.maxThrows && (isGrenade || isKunai))
        {
            grdScript.AddThrowable();
            Destroy(gameObject);
        }else if(grdScript.GetThrowableCount() == grdScript.maxThrows && rangeFromPlayer.magnitude <= pickUpRange)
        {
            // will say grenade bag is full
            Debug.Log("Grenade bag is full");
        }
    }

    public bool WithinRange()
    {
        return rangeFromPlayer.magnitude <= pickUpRange;
    }
}
