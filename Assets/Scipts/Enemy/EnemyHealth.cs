using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int level = 2;
    [SerializeField] private float baseHitPoint = 20f;
    [SerializeField] private float expPoint = 1f;
    
    [Range(10f, 50f)]
    [SerializeField] private float baseExpPoint = 10f;

    [SerializeField] private float starFragmentPoints = 0.5f;
    
    [SerializeField] private Image healthBar;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private string enemyName = "UNKOWN";
    
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private NotificationPopUp notificationPopUp;

    private float currentMaxHitPoints;
    private float hitPoints;
    public UnityEvent<float> onDamageTaken;

    private const int MAX_LEVEL = 20;
    private const int MIN_LEVEL = 2;
    private const float HP_SCALE = 0.2f;
    private const float MAX_EXP_POINT = 50f;

    private float delay = 0.5f;

    private PlayerLevel playerLevel;
    private HUDController ui;
    private CatchEncounters catchEncounters;

    private void Awake()
    {
        onDamageTaken = new UnityEvent<float>();
        SetLevel(level);
        
        playerLevel = FindFirstObjectByType<PlayerLevel>();
        SetExpPoint();

        ui = FindAnyObjectByType<HUDController>();
        catchEncounters = FindFirstObjectByType<CatchEncounters>();
    }

    public void SetLevel(int newLevel)
    {
        level = Random.Range(MIN_LEVEL, MAX_LEVEL);
        Debug.Log(level);

        currentMaxHitPoints = baseHitPoint + level + HP_SCALE;
        hitPoints = currentMaxHitPoints;
        Debug.Log(hitPoints);

        UpdateEnmeyData();
    }

    private void UpdateEnmeyData()
    {
        levelText.text = "Lvl: " + level;
        nameText.text = enemyName;

        UpdateHealthBar();
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetCurrentHealth()
    {
        return Mathf.RoundToInt(hitPoints);
    }

    public int GetMaxHealth()
    {
        return Mathf.RoundToInt(currentMaxHitPoints);
    }

    private void SetExpPoint()
    {
        expPoint += Random.Range(baseExpPoint, MAX_EXP_POINT); 
    }

    public void RestoreHealth()
    {
        hitPoints = currentMaxHitPoints;
        UpdateHealthBar();
        Debug.Log($"{level}: health restored, HP: {GetCurrentHealth()} / {GetMaxHealth()}");
    }

    public void Heal(float healAmount)
    {
        hitPoints += healAmount;
        if (hitPoints > currentMaxHitPoints)
        {
            hitPoints = currentMaxHitPoints;
        }
        UpdateHealthBar();
        Debug.Log($"{level}: health restored by {healAmount}, HP: {GetCurrentHealth()} / {GetMaxHealth()}");
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        onDamageTaken?.Invoke(damage);
        Debug.Log($"{level}: got damaged {damage}, HP left: {GetCurrentHealth()}");
        UpdateHealthBar();
        catchEncounters.SetTargetEnemy(this);

        if (hitPoints <= 0)
        {
            FindFirstObjectByType<MusicController>()?.PlayVictoryMusic();
            
            playerLevel.AddExpPoints(expPoint);
            notificationPopUp.ShowNotification($"<color=red>Enemy Defeated!</color> +{expPoint:F0} XP");
            ui.UpdateStarFragmentBar(starFragmentPoints);
            Destroy(gameObject, delay);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            int currentHealth = GetCurrentHealth();
            float maxHealth = GetMaxHealth();

            float healthRatio = currentHealth / maxHealth;
            
            if (currentHealth <= 0)
            {
                healthRatio = 0;
                currentHealth = 0;
            }
            
            healthBar.fillAmount = healthRatio;

            hpText.text = $"HP: {currentHealth} / {maxHealth}";
        }
    }
}
