using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    private Animator animator;
    private EnemyHealth health;

    private readonly string IS_CHASING = "IsChasing";
    private readonly string IS_ATTACKING = "IsAttacking";
    private readonly string HIT = "Hit";
    private readonly string DIE = "Die";
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        if (health != null)
        {
            // Subscribe to damage event to trigger hit and death animations
            health.onDamageTaken.AddListener((damage) => {
                if (health.GetCurrentHealth() <= 0)
                {
                    TriggerDeathAnimation();
                }
                else
                {
                    TriggerHitAnimation();
                }
            });
        }
    }

    public void SetChasing(bool isChasing)
    {
        if (animator != null)
        {
            animator.SetBool(IS_CHASING, isChasing);
        }
    }

    public void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IS_ATTACKING, true);
        }
    }

    public void StopAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(IS_ATTACKING, false);
        }
    }

    private void TriggerHitAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(HIT);
        }
    }

    private void TriggerDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(DIE);
            
            // Disable other animations
            animator.SetBool(IS_CHASING, false);
            animator.SetBool(IS_ATTACKING, false);
        }
    }
}
