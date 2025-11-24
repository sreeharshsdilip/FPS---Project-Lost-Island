using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    HUDController hudUI;
    UiController ui;
    HitEffect hitEffect;

    private void Start()
    {
        currentHealth = maxHealth;
        hudUI = FindFirstObjectByType<HUDController>();
        ui = FindFirstObjectByType<UiController>();
        hitEffect = FindFirstObjectByType<HitEffect>();
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        hudUI.UpdateHealthBar(currentHealth, maxHealth);
        hitEffect.TriggerHitEffect();
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Player has died.");
        
        transform.GetComponent<MonoBehaviour>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        ui.ShowGameOverMenu();
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void HealPlayer(float healAmount)
    {
        currentHealth += healAmount;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        hudUI.UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log($"Player healed for {healAmount}. Current health: {currentHealth}");
    }
}
