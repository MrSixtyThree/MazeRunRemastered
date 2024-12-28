using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType
    {
        Pistol,
        MachineGun,
        Shotgun
    }

    public WeaponType weapon;

    // Gun attributes
    public string weaponName;
    public float damage;
    public float fireRate; // Time between shooting
    public float bulletSpread;
    public float bulletRange;
    public int bulletsPerShot;
    public int bulletsShot;
    public int currentMagazineAmmo;
    public int magazineSize;
    public float reloadTime;
    private Vector3 gunDefaultPosition;
    private Quaternion gunDefaultRotation;
    private Vector3 gunReloadPosition = new Vector3(0.5f, -0.5f, 1.2f);
    private Vector3 gunReloadRotation;
    private float shakeIntensity;
    private Vector3 camOriginPosition;
    private Quaternion camOriginRotation;
    private float timeSinceHit;

    // Booleans
    public bool isReloading;
    public bool isShooting;
    public bool canShoot;
    public bool isAutomatic; // If false, shoot key cannot be held

    // References
    public Camera cam;
    private GameObject currentWeapon;
    public LayerMask canBeHitLayerMask;
    private RaycastHit hit;
    public GameObject bulletEffect;
    public GameObject pistolPrefab;
    public GameObject machineGunPrefab;
    public GameObject shotgunPrefab;
    public Transform weaponAttachmentPoint;
    public TextMeshProUGUI HUDAmmo;

    public UserData activePlayer;

    // Methods

    private void Start()
    {
        setWeaponType(weapon); // Default to pistol

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentMagazineAmmo = magazineSize;
        canShoot = true;
        activePlayer = GameObject.Find("Menu Controller").GetComponent<UIController>().activePlayer;
    }

    void Update()
    {
        // Get whether shooting key is being held or tapped

        if (isAutomatic) // Machine gun and being fired rapidly
        {
            isShooting = Input.GetKey(KeyCode.Mouse0); // Holding
        }
        else
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0); // Tapped
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && currentMagazineAmmo < magazineSize && !isReloading)
        {
            Reload();
        }

        // Shoot
        if (canShoot && isShooting && !isReloading && currentMagazineAmmo > 0)
        {
            bulletsShot = bulletsPerShot;
            RayCastShoot();
        }

        // Camera Shake
        if (shakeIntensity > 0)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            cam.transform.position = camOriginPosition + shakeOffset;
            shakeIntensity -= 0.02f;
        }
        else
        {
            cam.transform.localPosition = new Vector3(0f, 0.23f, 0f); // Reset camera in case camera shake throws it off
        }

        HUDAmmo.text = currentMagazineAmmo + " / " + magazineSize;

    }

    void OnGUI()
    {
        if (isReloading)
        {
            int size = 20;

            // Offset the X position to center the text
            float offsetX = 40;

            // centre of screen and caters for font size
            float posX = cam.pixelWidth / 2 - size / 4 - offsetX;
            float posY = cam.pixelHeight / 2 - size / 2;

            size = 100;
            GUI.color = Color.red;
            GUI.Label(new Rect(posX, posY, size, size), "RELOADING");
        }
        else if (!isReloading && currentMagazineAmmo == 0)
        {
            int size = 20;

            // Offset the X position to center the text
            float offsetX = 40;

            // centre of screen and caters for font size
            float posX = cam.pixelWidth / 2 - size / 4 - offsetX;
            float posY = cam.pixelHeight / 2 - size / 2;

            size = 100;
            GUI.color = Color.white;
            GUI.Label(new Rect(posX, posY, size, size), "NO AMMO");
        }
        else
        {
            int size = 30;

            // centre of screen and caters for font size
            float posX = cam.pixelWidth / 2 - size / 4;
            float posY = cam.pixelHeight / 2 - size / 2;

            // displays "*" in the crentre of screen
            if (Time.time < timeSinceHit + 0.5f)
            {
                GUI.color = Color.red;
                GUI.Label(new Rect(posX, posY, size, size), "*");
            }
            else
            {
                GUI.color = Color.white;
                GUI.Label(new Rect(posX, posY, size, size), "*");
            }
        }
    }

    public void setWeaponType(WeaponType type)
    {
        weapon = type;

        // Destory existing weapon
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        switch (weapon)
        {
            case WeaponType.Pistol:
                weaponName = "Pistol";
                damage = 40;
                fireRate = 0.8f;
                bulletSpread = 0.06f;
                bulletRange = 100;
                bulletsPerShot = 1;
                magazineSize = 13;
                reloadTime = 1;
                isAutomatic = false;
                currentWeapon = Instantiate(pistolPrefab, weaponAttachmentPoint);
                currentWeapon.transform.localPosition = new Vector3(-0.1f, -0.4f, -0.1f);
                currentWeapon.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                gunReloadRotation = new Vector3(90f, -30f, 0f);
                break;
            case WeaponType.MachineGun:
                weaponName = "Machine Gun";
                damage = 20;
                fireRate = 0.1f;
                bulletSpread = 0.13f;
                bulletRange = 100;
                bulletsPerShot = 1;
                magazineSize = 32;
                reloadTime = 3;
                isAutomatic = true;
                currentWeapon = Instantiate(machineGunPrefab, weaponAttachmentPoint);
                currentWeapon.transform.localPosition = new Vector3 (-0.1f, -0.3f, 0.4f);
                currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                gunReloadRotation = new Vector3(-20f, 0f, 0f);
                break;
            case WeaponType.Shotgun:
                weaponName = "Shotgun";
                damage = 70;
                fireRate = 0f;
                bulletSpread = 0.3f;
                bulletRange = 50;
                bulletsPerShot = 10;
                magazineSize = 10;
                reloadTime = 4;
                isAutomatic = false;
                currentWeapon = Instantiate(shotgunPrefab, weaponAttachmentPoint);
                currentWeapon.transform.localPosition = new Vector3(-0.2f, -0.2f, 1f);
                currentWeapon.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                gunReloadRotation = new Vector3(70f, 0f, 0f);
                break;
        }

        applyUserData(); // Apply upgrades

        gunDefaultPosition = currentWeapon.transform.localPosition;
        gunDefaultRotation = currentWeapon.transform.localRotation;
    }

    protected void RayCastShoot()
    {
        canShoot = false;

        // Bullet spread
        float xSpread = Random.Range(-bulletSpread, bulletSpread);
        float ySpread = Random.Range(-bulletSpread, bulletSpread);
        float zSpread = Random.Range(-bulletSpread, bulletSpread);

        // Direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(xSpread, ySpread, zSpread);

        // Store camera position for shake
        camOriginPosition = cam.transform.position;

        // Apply camera shake
        shakeIntensity = 0.2f;

        // Cast Ray
        if (Physics.Raycast(cam.transform.position, direction, out hit, bulletRange, canBeHitLayerMask))
        {
            Debug.DrawLine(cam.transform.position, hit.point, Color.red, 5);
    
            if (hit.collider.CompareTag("Enemy")){
                Debug.Log("Hit Enemy");
                hit.collider.GetComponent<EnemyUI>().takeDamage((int)damage); // Implement enemies taking damage
                timeSinceHit = Time.time;
            }
        }

        // Instantiate special effect for bullet hit
        Instantiate(bulletEffect, hit.point, Quaternion.Euler(0, 0, 0));

        currentMagazineAmmo--; // Reduce current magazines ammo
        bulletsShot--; // Count down the number of times the gun has shot per shot

        Invoke("ResetShootTime", fireRate); // Allow shooting again after delay of firerate
          
        if (bulletsShot > 0 && currentMagazineAmmo > 0)
        {
            Invoke("RayCastShoot", fireRate);
        }
        
    }

    public void Reload()
    {
        isReloading = true;
        StopCoroutine("moveWeapon");
        StartCoroutine(moveWeapon(gunReloadPosition));
        StartCoroutine(rotateWeapon(gunReloadRotation));
        Invoke("FinishReload", reloadTime);
    }

    public void FinishReload()
    {
        StopCoroutine("moveWeapon");
        currentWeapon.transform.localRotation = gunDefaultRotation;
        currentWeapon.transform.localPosition = gunDefaultPosition;
        StartCoroutine(moveWeapon(gunDefaultPosition));
        currentMagazineAmmo = magazineSize;
        isReloading = false;
    }

    public void ResetShootTime()
    {
        canShoot = true;
    }

    public void upgradeDamage(float change)
    {
        damage = damage + (damage * change);
    }

    public void upgradeReloadSpeed(float change)
    {
        reloadTime = reloadTime - (reloadTime * change);
    }

    public void upgradeMagazineSize(float change)
    {
        magazineSize = magazineSize + (int)(magazineSize * change);
    }
    public void upgradeAccuracy(float change)
    {
        bulletSpread = bulletSpread - (bulletSpread * change);
    }

    IEnumerator moveWeapon(Vector3 targetPos)
    {
        while(currentWeapon.transform.localPosition != targetPos)
        {
            currentWeapon.transform.localPosition = Vector3.MoveTowards(currentWeapon.transform.localPosition, targetPos, 0.1f* Time.deltaTime); // Move to target position
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
    IEnumerator rotateWeapon(Vector3 targetRot)
    {
        Quaternion startRotation = currentWeapon.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(targetRot);

        float elapsedTime = 0f;
        float rotationDuration = 0.7f;

        while (elapsedTime < rotationDuration)
        {
            currentWeapon.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentWeapon.transform.localRotation = targetRotation;
    }

    public void applyUserData()
    {
        switch (weapon)
        {
            case WeaponType.Pistol:
                damage = damage + damage * activePlayer.getPistolDamage();
                fireRate = fireRate + fireRate * activePlayer.getPistolFireRate();
                bulletSpread = bulletSpread - bulletSpread * activePlayer.getPistolAccuracy();
                magazineSize = magazineSize + (int)(magazineSize * activePlayer.getPistolMag());
                reloadTime = reloadTime - reloadTime * activePlayer.getPistolReload();
                break;

            case WeaponType.MachineGun:
                damage = damage + damage * activePlayer.getMGDamage();
                fireRate = fireRate + fireRate * activePlayer.getMGFireRate();
                bulletSpread = bulletSpread - bulletSpread * activePlayer.getMGAccuracy();
                magazineSize = magazineSize + (int)(magazineSize * activePlayer.getMGMag());
                reloadTime = reloadTime - reloadTime * activePlayer.getMGReload();
                break;

            case WeaponType.Shotgun:
                damage = damage + damage * activePlayer.getShotgunDamage();
                fireRate = fireRate + fireRate * activePlayer.getShotgunFireRate();
                bulletSpread = bulletSpread - bulletSpread * activePlayer.getShotgunAccuracy();
                magazineSize = magazineSize + (int)(magazineSize * activePlayer.getShotgunMag());
                reloadTime = reloadTime - reloadTime * activePlayer.getShotgunReload();
                break;
        }
    }

}



