using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    [Header("References")] 
    public Transform vCam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public TextMeshProUGUI count;
    private InputManager _inputManager;

    [Header("Configurations")] 
    public int totalThrows;
    public float throwCooldown;
    [HideInInspector]public int maxThrows;

    [Header("Throw Stats")]
    public float throwForce;
    public float throwUpwardForce;
    
    public bool readyToThrow;
    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        _inputManager = InputManager.instance();
        count.text = "Grenades Left: " + totalThrows;
        maxThrows = totalThrows;
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputManager.pauseGame())
        {
            return;
        }
        if (_inputManager.Throwable() && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = false;
        
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, vCam.rotation);
        
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = vCam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(vCam.position, vCam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;
        count.text = "Grenades Left: " + totalThrows;
        
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    public void AddGrenade()
    {
        totalThrows++;
        count.text = "Grenades Left: " + totalThrows;
    }

    public int getGrenadeCount()
    {
        return totalThrows;
    }
}
