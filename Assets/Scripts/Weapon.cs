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

    // Audio Manager
    public AudioManager audioManager;

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
    private Quaternion gunReloadRotation;
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
    public LayerMask enemyLayerMask;
    private RaycastHit hit;
    public GameObject bulletEffect;
    public GameObject pistolPrefab;
    public GameObject machineGunPrefab;
    public GameObject shotgunPrefab;
    public Transform weaponAttachmentPoint;
    public TextMeshProUGUI HUDAmmo;

    public UserData activePlayer;

    public float impactSoundRadius = 5f;
    public float firingSoundRadius = 10f;

    // Methods

    private void Start()
    {
        activePlayer = GameObject.Find("Menu Controller").GetComponent<UIController>().activePlayer;

        setWeaponType(weapon); // Default to pistol

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentMagazineAmmo = magazineSize;
        canShoot = true;

        audioManager = GameObject.Find("Audio Controller").GetComponent<AudioManager>();

        enemyLayerMask = LayerMask.GetMask("Enemies");
        canBeHitLayerMask = ~LayerMask.GetMask("Ignore Raycast");

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
            StopCoroutine("moveWeapon");
            StopCoroutine("rotateWeapon");
            currentWeapon.transform.localRotation = gunDefaultRotation;
            currentWeapon.transform.localPosition = gunDefaultPosition;

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
                gunReloadRotation = Quaternion.Euler(90f, -30f, 0f);
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
                currentWeapon.transform.localPosition = new Vector3 (-0.1f, -0.4f, 0.4f);
                currentWeapon.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                gunReloadRotation = Quaternion.Euler(-35f, 0f, 0f);
                break;
            case WeaponType.Shotgun:
                weaponName = "Shotgun";
                damage = 70;
                fireRate = 0.0f;
                bulletSpread = 0.3f;
                bulletRange = 50;
                magazineSize = 10;
                bulletsPerShot = magazineSize / 2;
                reloadTime = 4;
                isAutomatic = false;
                currentWeapon = Instantiate(shotgunPrefab, weaponAttachmentPoint);
                currentWeapon.transform.localPosition = new Vector3(-0.2f, -0.2f, 1f);
                currentWeapon.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                gunReloadRotation = Quaternion.Euler(70f, 0f, 0f);
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
            else
            {
                // Get enemies to investigate the bullet impact
                foreach (Collider enemy in Physics.OverlapSphere(hit.point, impactSoundRadius, enemyLayerMask))
                {
                    enemy.GetComponent<EnemyUI>().setCustomGoal(hit.point);
                }
            }
        }

        // Instantiate special effect for bullet hit
        Instantiate(bulletEffect, hit.point, Quaternion.Euler(0, 0, 0));

        currentMagazineAmmo--; // Reduce current magazines ammo
        bulletsShot--; // Count down the number of times the gun has shot per shot

        Invoke("ResetShootTime", fireRate); // Allow shooting again after delay of firerate

        // Play sound effect
        switch (weapon)
        {
            case WeaponType.Pistol:
                audioManager.PlaySFX("pistolFire", transform.position, 0.3f);
                break;
            case WeaponType.MachineGun:
                audioManager.PlaySFX("machineGunFire", transform.position);
                break;
            case WeaponType.Shotgun:
                if (bulletsShot == 0)
                {
                    audioManager.PlaySFX("shotgunFire", transform.position);
                }
                break;
        }
          
        if (bulletsShot > 0 && currentMagazineAmmo > 0)
        {
            Invoke("RayCastShoot", fireRate);
        }

        // Get enemies to investigate the sound of the gun firing, takes precedence over bullet impact
        foreach (Collider enemy in Physics.OverlapSphere(transform.position, firingSoundRadius, enemyLayerMask))
        {
            enemy.GetComponent<EnemyUI>().setCustomGoal(transform.position);
        }

    }

    public void Reload()
    {
        if (isReloading) return; // Prevent multiple reload attempts
        isReloading = true;

        StopAllCoroutines();

        StartCoroutine(ReloadRoutine());


    }

    private IEnumerator ReloadRoutine()
    {
        var routines = new []
        {
            StartCoroutine(rotateWeapon(gunReloadRotation)),
            //StartCoroutine(moveWeapon(gunReloadPosition))
        };

        // Play sound effect
        switch (weapon)
        {
            case WeaponType.Pistol:
                audioManager.Play2DSFX("pistolReload");
                break;
            case WeaponType.MachineGun:
                audioManager.Play2DSFX("machineGunReload");
                break;
            case WeaponType.Shotgun:
                audioManager.Play2DSFX("shotgunReload");
                break;
        }
        
        foreach (var routine in routines)
        {
            yield return routine;
        }

        currentWeapon.transform.localRotation = gunReloadRotation;
        currentWeapon.transform.localPosition = gunReloadPosition;

        FinishReload();
    }
    public void FinishReload()
    {

        currentWeapon.transform.localRotation = gunDefaultRotation;
        currentWeapon.transform.localPosition = gunDefaultPosition;

        currentMagazineAmmo = magazineSize;
        isReloading = false;
    }

    public void ResetShootTime()
    {
        canShoot = true;
    }

    // public void upgradeDamage(float change)
    // {
    //     damage = damage + (damage * change);
    // }

    // public void upgradeReloadSpeed(float change)
    // {
    //     reloadTime = reloadTime - (reloadTime * change);
    // }

    // public void upgradeMagazineSize(float change)
    // {
    //     magazineSize = magazineSize + (int)(magazineSize * change);
    // }
    // public void upgradeAccuracy(float change)
    // {
    //     bulletSpread = bulletSpread - (bulletSpread * change);
    // }

    IEnumerator moveWeapon(Vector3 targetPos)
    {
        Vector3 startPosition = currentWeapon.transform.localPosition;
        float elapsedTime = 0f;
        float duration = reloadTime * 0.5f; // Duration proportional to reload time

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentWeapon.transform.localPosition = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);

            if (Vector3.Distance(currentWeapon.transform.localPosition, targetPos) < 0.001f)
            {
                break; // Prevent endless loop if already at the target
            }

            yield return null;
        }

        currentWeapon.transform.localPosition = targetPos; // Snap to target position
    }
    IEnumerator rotateWeapon(Quaternion targetRot)
    {
        Quaternion startRotation = currentWeapon.transform.localRotation;
        Quaternion targetRotation = targetRot;

        float elapsedTime = 0f;
        float duration = reloadTime * 0.75f; // Duration proportional to reload time

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            currentWeapon.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);

            if (Quaternion.Angle(currentWeapon.transform.localRotation, targetRotation) < 0.1f)
            {
                break; // Prevent endless loop if already at the target
            }

            yield return null;
        }

        currentWeapon.transform.localRotation = targetRotation; // Snap to target rotation
    }

    public void applyUserData()
    {
        Debug.Log("Applying upgrades to: " + activePlayer.ToString());


        if (activePlayer == null || activePlayer.playerName == "")
        {
            activePlayer = GameObject.Find("Menu Controller").GetComponent<UIController>().activePlayer;
        }
        switch (weapon)
        {
            case WeaponType.Pistol:
                damage *= activePlayer.getPistolDamage();
                fireRate /= activePlayer.getPistolFireRate();
                bulletSpread /= activePlayer.getPistolAccuracy();
                magazineSize = (int)(magazineSize * activePlayer.getPistolMag());
                reloadTime /= activePlayer.getPistolReload();
                break;

            case WeaponType.MachineGun:
                damage *= activePlayer.getMGDamage();
                fireRate /= activePlayer.getMGFireRate();
                bulletSpread /= activePlayer.getMGAccuracy();
                magazineSize = (int)(magazineSize * activePlayer.getMGMag());
                reloadTime /= activePlayer.getMGReload();
                break;

            case WeaponType.Shotgun:
                damage *= activePlayer.getShotgunDamage();
                fireRate /= activePlayer.getShotgunFireRate();
                bulletSpread /= activePlayer.getShotgunAccuracy();
                magazineSize = (int)(magazineSize * activePlayer.getShotgunMag());
                reloadTime /= activePlayer.getShotgunReload();

                bulletsPerShot = magazineSize / 2;
                break;
        }
    }

}



