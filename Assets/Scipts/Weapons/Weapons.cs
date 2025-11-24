using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject zombieHitFX;
    [SerializeField] private GameObject groundHitFX;
    [SerializeField] private NotificationPopUp notificationPopUp;

    private bool canShoot = true;
    private float delay = 0.5f;

    public Ammo ammo;
    public AmmoType ammoType;

    private WeaponShootingSFX weaponSFX;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        weaponSFX = GetComponent<WeaponShootingSFX>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && canShoot)
        {
            StartCoroutine(Shoot());
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weaponSFX.StopShootingSFX();
        }
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        if (ammo.GetCurrentAmmo(ammoType) > 0)
        {
            HandleRaycast();
            MuzzleFlash();
            weaponSFX.PlayShootingSFX();
            ammo.RemoveAmmo(ammoType);
            Debug.Log("Ammo left: " + ammo.GetCurrentAmmo(ammoType));
        } else
        {
            Debug.Log("Out of ammo!");
            notificationPopUp.ShowNotification("<color=red>Out of Ammo!");
        }
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    private void MuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    private void HandleRaycast()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            Debug.Log("Hit: " + hit.collider.name);
            ShowHitImpact(hit);

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void ShowHitImpact(RaycastHit hit)
    {
        if (zombieHitFX != null && hit.transform.CompareTag("Monster"))
        {
            GameObject impact = Instantiate(zombieHitFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, delay);
        }
        else if (groundHitFX != null)
        {
            GameObject impact = Instantiate(groundHitFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, delay);
        }
    }
}
