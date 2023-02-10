using TMPro;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    [Header("References")] 
    public Transform vCam;
    public Transform attackPoint;
    public GameObject objectToThrow;
    public Transform player;
    public TextMeshProUGUI count;
    public Camera cam;
    public GameObject kunai;
    public GameObject grenade;
    private InputManager inputManager;

    [Header("Configurations")] 
    public int totalThrows;
    public float throwCooldown;
    public float pickUpRange;
    [HideInInspector]public int maxThrows;
    private RaycastHit hit;
    private bool isKunai;
    private bool isGrenade;
    private Vector3 rangeFromPlayer;

    [Header("Throw Stats")]
    public float throwForce;
    public float throwUpwardForce;
    
    public bool readyToThrow;
    // Start is called before the first frame update
    void Start()
    {
        readyToThrow = true;
        inputManager = InputManager.instance();
        maxThrows = totalThrows;

        isKunai = objectToThrow.CompareTag("kunai");
        isGrenade = objectToThrow.CompareTag("grenade");
        
        if (isKunai)
        {
            count.text = "Kunai: " + totalThrows;
            SwapForces();
        }else if (isGrenade)
        {
            count.text = "Grenades: " + totalThrows;
            SwapForces();
        }
        else
        {
            count.text = "No throwable Equipped";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.pauseGame())
        {
            return;
        }
        
        if (inputManager.Throwable() && readyToThrow && totalThrows > 0)
        {
            Throw();
        }
        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Physics.Raycast(ray, out hit);
        
        // swapping throwables
        if (hit.transform != null)
        {
            if (hit.transform.CompareTag("weapon"))
            {
                return;
            }
            
            rangeFromPlayer = player.position - hit.transform.position;
            
            if (inputManager.PickupItem() && rangeFromPlayer.magnitude <= pickUpRange)
            {
                if (!isKunai && hit.transform.CompareTag("loot_kunai"))
                {
                    isKunai = true;
                    isGrenade = false;
                    throwForce = 60f;
                    throwUpwardForce = 0f;
                    objectToThrow = kunai;
                    count.text = "Kunai: " + totalThrows;
                    SwapForces();
                }else if (!isGrenade && hit.transform.CompareTag("loot_grenade"))
                {
                    isKunai = false;
                    isGrenade = true;
                    throwForce = 30f;
                    throwUpwardForce = 10f;
                    objectToThrow = grenade;
                    count.text = "Grenades: " + totalThrows;
                    SwapForces();
                }
            }
        }
    }

    private void Throw()
    {
        readyToThrow = false;
        
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, vCam.rotation);

        if (isKunai)
        {
            projectile.transform.rotation = Quaternion.Euler(0,player.transform.eulerAngles.y + 90, 90);
        }else if (isGrenade)
        {
            projectile.transform.rotation = Quaternion.Euler(-121, -80, 56);
        }
        
        Rigidbody projectileRB = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = vCam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(vCam.position, vCam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        
        Vector3 forceToAdd = player.GetComponent<Rigidbody>().velocity;
        forceToAdd += forceDirection * throwForce + transform.up * throwUpwardForce;
        
        projectileRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;
        if (isKunai)
        {
            count.text = "Kunai: " + totalThrows;
        }else if (isGrenade)
        {
            count.text = "Grenades: " + totalThrows;
        }
        
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void SwapForces()
    {
        if (isGrenade)
        {
            throwForce = 20;
            throwUpwardForce = 10;
            objectToThrow.GetComponent<Rigidbody>().useGravity = true;
        }else if (isKunai)
        {
            throwForce = 50;
            throwUpwardForce = 0;
            objectToThrow.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    // helper functions
    public void AddThrowable()
    {
        totalThrows++;
        if (isKunai)
        {
            count.text = "Kunai: " + totalThrows;
        }else if (isGrenade)
        {
            count.text = "Grenades: " + totalThrows;
        }
        
    }

    public bool WithinRange()
    {
        return rangeFromPlayer.magnitude <= pickUpRange;
    }
    
    public int GetThrowableCount()
    {
        return totalThrows;
    }

    public bool IsGrenadeEquipped()
    {
        return isGrenade;
    }
    
    public bool IsKunaiEquipped()
    {
        return isKunai;
    }
}
