using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class gunScript : MonoBehaviour
{
    [Header("Bullet Forces")] 
    public float shootForce;
    public float upwardForce;

    [Header("Gun Stats")] 
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime; 
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public int gunDamage;
    public int gunZoom;
    public int ammoSupply;
    public bool allowButtonHold;
    private int maxAmmo;

    private int bulletsLeft, bulletsShot;

    //Recoil
    // public CharacterController playerRB;
    // public float recoilForce;

    //bools
    private bool shooting, readyToShoot, reloading;
    
    [Header("References")]
    public GameObject bullet;
    public Camera fpsCam;
    public Transform attackPoint;
    public Transform gunContainer;
    public playerController player;
    private InputManager inputManager;
    private bool equipped;

    [Header("Graphics")]
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    //bug fixing :D
    public bool allowInvoke = true;

    private bool pause;

    private void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        bulletsShot = 0;
        pause = false;
        inputManager = InputManager.instance();
        maxAmmo = ammoSupply;
    }

    private void Update()
    {
        if (inputManager.pauseGame())
        {
            if (pause)
            {
                pause = false;
            }
            else
            {
                pause = true;
            }
        }else if (pause)
        {
            return;
        }
        MyInput();

        //Set ammo display, if it exists :D
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " - " + ammoSupply + " :" + transform.name);
    }
    private void MyInput()
    {
        player.setGunZoom(gunZoom, equipped);
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = inputManager.Automatic();
        else shooting = inputManager.NotAutomatic();

        //Reloading 
        if (inputManager.Reload() && bulletsLeft < magazineSize && !reloading) 
            Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && ammoSupply > 0)
            Reload();
        
        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        //Just a ray through the middle of your current view
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        bullet.GetComponent<playerProjectile>().gun = this;
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            // Add recoil to player (should only be called once)
            // playerRB.attachedRigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished()
    {
        //Fill magazine

        if (ammoSupply - bulletsShot < 0)
        {
            bulletsLeft = ammoSupply + bulletsLeft;
            ammoSupply = 0;
        }
        else
        {
            ammoSupply -= bulletsShot;
            bulletsShot = 0;
            bulletsLeft = magazineSize;
        }
        reloading = false;
    }

    public void setEquipStatus(bool b)
    {
        equipped = b;
    }
    
    public int GetMaxAmmoSupply()
    {
        return maxAmmo;
    }
}
