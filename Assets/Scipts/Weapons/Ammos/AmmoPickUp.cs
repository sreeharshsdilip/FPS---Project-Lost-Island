using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{ 
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private AmmoType ammoType;

    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private NotificationPopUp notificationPopUp;

    Ammo ammo;

    private void Start()
    {
        ammo = FindFirstObjectByType<Ammo>();
    }

    public void CollectAmmo()
    {
        if (ammo != null)
        {
            notificationPopUp.ShowNotification($"Picked Up {ammoType} <color=green>x{ammoAmount}</color>");
            ammo.AddAmmo(ammoType, ammoAmount);
            
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            Debug.Log($"Picked up {ammoAmount} ammo of type {ammoType}. Current amount: {ammo.GetCurrentAmmo(ammoType)}");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Ammo component not found in the scene.");
        }
    }
}
