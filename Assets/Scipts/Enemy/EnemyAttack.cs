using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float baseDamage = 5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.5f;

    private EnemyAnimations enemyAnimations;
    private Transform target;
    private float nextAttackTime;
    private EnemyHealth enemyHealth;
    private float damage;

    private void Start()
    {
        enemyAnimations = GetComponent<EnemyAnimations>();
        enemyHealth = GetComponent<EnemyHealth>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Scale damage based on level
        int level = enemyHealth != null ? enemyHealth.GetLevel() : 2;
        damage = baseDamage * (1 + (level - 1) * 0.2f);

        if (target == null)
        {
            Debug.LogError($"[{gameObject.name}] Player not found!");
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
        {
            AttackTarget();
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (distanceToTarget > attackRange)
        {
            enemyAnimations?.StopAttackAnimation();
        }
    }

    private void AttackTarget()
    {
        if (enemyAnimations == null)
        {
            Debug.LogError($"[{gameObject.name}] EnemyAnimations component is missing!");
            return;
        }

        FindFirstObjectByType<MusicController>()?.PlayCombatMusic();

        enemyAnimations.SetChasing(false);
        enemyAnimations.TriggerAttackAnimation();

        if (target.gameObject.tag == "Player")
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else
        {
            EnemyHealth TargetHealth = target.GetComponent<EnemyHealth>();
            if (TargetHealth != null)
            {
                TargetHealth.TakeDamage(damage);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
