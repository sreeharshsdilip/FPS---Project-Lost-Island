using System;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image expBar;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject itemPickUpPanel;
    [SerializeField] private NotificationPopUp notificationPopUp;

    [SerializeField] private Image starFragmentBar;

    private float range = 4f;

    private const float HEALTH_MEDIUM = 0.30f; // 30%
    private const float HEALTH_LOW = 0.10f; // 10%

    private PlayerLevel playerLevel;

    private void Start()
    {
        itemPickUpPanel.SetActive(false);

        playerLevel = FindFirstObjectByType<PlayerLevel>();
        if (playerLevel != null)
        {
            playerLevel.onXPChanged.AddListener(UpdateExpBar);
            playerLevel.onLevelUp.AddListener(OnPlayerLevelUp);
        }
    }

    private void Update()
    {
        TryPickUpItems();
    }

    public void UpdateHealthBar(float currentHealth, float maxHP)
    {
        if (healthBar != null)
        {

            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }

            float healthRatio = currentHealth / maxHP;

            healthBar.fillAmount = healthRatio;
            hpText.text = $"{currentHealth:F0} / {maxHP}";

            UpdateHealthBarColor(healthRatio);
        }
    }

    private void UpdateHealthBarColor(float healthRatio)
    {
        if (healthRatio <= HEALTH_LOW)
        {
            healthBar.color = Color.red;
        }
        else if (healthRatio <= HEALTH_MEDIUM)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.green;
        }
    }

    private void TryPickUpItems()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
        {
            if (hit.collider.CompareTag("Item") || hit.collider.CompareTag("Gun"))
            {
                itemPickUpPanel.SetActive(true);
                PickUpItem(hit);
            }
            else
            {
                itemPickUpPanel.SetActive(false);
            }
        }
    }

    private void PickUpItem(RaycastHit hit)
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (hit.collider.CompareTag("Gun"))
            {
                int weaponIndex = -1;
                string name = hit.transform.gameObject.name;

                for (int i = 0; i < 10; i++)
                {
                    if (name.Contains(i.ToString()))
                    {
                        weaponIndex = i;
                        break;
                    }
                }

                if (weaponIndex >= 0)
                {
                    WeaponSwitcher weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
                    if (weaponSwitcher != null)
                    {
                        weaponSwitcher.UnlockWeapon(weaponIndex);
                    }
                }

                notificationPopUp.ShowNotification($"Picked Up {name}");
            }
            else
            {
                AmmoPickUp ammo = FindFirstObjectByType<AmmoPickUp>();
                ammo.CollectAmmo();
            }
            Destroy(hit.transform.gameObject);
            itemPickUpPanel.SetActive(false);
        }
    }

    private void UpdateExpBar(float currentXP, float expToNextLevel)
    {
        if (expBar != null)
        {
            if (currentXP < 0)
            {
                currentXP = 0;
            }

            float expRatio = currentXP / expToNextLevel;
            expBar.fillAmount = expRatio;
        }
    }

    private void OnPlayerLevelUp(int level)
    {
        Debug.Log("Player leveled up!");
    }

    public void UpdateStarFragmentBar(float starFragment)
    {
        float starFragmentRatio = starFragment / 100f;
        starFragmentBar.fillAmount += starFragmentRatio;

        if (starFragmentBar.fillAmount >= 1f)
        {
            notificationPopUp.ShowNotification("<color=green>Main Quest Complete</color> Star Fragment collected!");
        }
    }
}
