using UnityEngine;
using UnityEngine.Events;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private float startingXP = 0f;
    [SerializeField] private float expToNextLevel = 100f;
    [SerializeField] private float expMultiplierPerLevel = 1.5f; // 50%
    [SerializeField] private NotificationPopUp notificationPopUp;
    
    public UnityEvent<int> onLevelUp;
    public UnityEvent<float, float> onXPChanged;

    private int currentLevel;
    private float currentXP;

    private PlayerHealth playerHealth;

    private void Start()
    {
        currentLevel = startingLevel;
        currentXP = startingXP;
        playerHealth = GetComponent<PlayerHealth>();
        
        if (onLevelUp == null)
            onLevelUp = new UnityEvent<int>();
        if (onXPChanged == null)
            onXPChanged = new UnityEvent<float, float>();
    }

    public void AddExpPoints(float xpAmount)
    {
        currentXP += xpAmount;
        onXPChanged?.Invoke(currentXP, expToNextLevel);
        notificationPopUp.ShowNotification($"Gained <color=green>+{xpAmount:F0} XP</color>");

        while (currentXP >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= expToNextLevel;
        currentLevel++;
        
        // Increase Exp required for next level
        expToNextLevel *= expMultiplierPerLevel;
        
        // Heal player on level up
        if (playerHealth != null)
        {
            playerHealth.HealPlayer(playerHealth.GetMaxHealth());
        }

        onLevelUp?.Invoke(currentLevel);
        onXPChanged?.Invoke(currentXP, expToNextLevel);
        notificationPopUp.ShowNotification($"Level Up! Lvl: <color=green>{currentLevel}");
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public (float current, float max) GetXP()
    {
        return (currentXP, expToNextLevel);
    }
}
