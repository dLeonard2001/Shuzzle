using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootables : MonoBehaviour
{
    [Header("Stats")] 
    public float pickUpRange;

    [Header("References")] 
    public Transform player;
    public Grenade grdScript;

    private void Update()
    {
        Vector3 rangeFromPlayer = player.position - transform.position;
        
        Debug.Log(rangeFromPlayer.magnitude);

        if (rangeFromPlayer.magnitude <= pickUpRange && grdScript.getGrenadeCount() < grdScript.maxThrows)
        {
            grdScript.AddGrenade();
            Destroy(gameObject);
        }else if (grdScript.getGrenadeCount() == grdScript.maxThrows)
        {
            Debug.Log("Grenade Pouch is Full");
        }
    }
}
