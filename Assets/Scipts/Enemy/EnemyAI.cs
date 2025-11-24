using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform FollowTraget;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float extendedChaseRange = 15f;
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float minWanderWaitTime = 2f;
    [SerializeField] private float maxWanderWaitTime = 5f;

    private float distanceToPlayer = Mathf.Infinity;
    private bool wasShot = false;
    private float nextWanderTime;
    private Vector3 startingPosition;
    private bool returningToStart = false;

    private NavMeshAgent agent;
    private EnemyHealth health;
    private EnemyAnimations enemyAnimations;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        enemyAnimations = GetComponent<EnemyAnimations>();
        startingPosition = transform.position;
        SetNextWanderTime();
        
        if (health != null)
        {
            health.onDamageTaken.AddListener((damage) => {
                wasShot = true;
            });
        }
    }

    private bool CheckIfInMainMenu()
    {
        return SceneManager.GetActiveScene().buildIndex == 0;
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(FollowTraget.position, transform.position);
        float currentChaseRange = wasShot ? extendedChaseRange : chaseRange;

        // If the scene is the main menu, just wander
        if (CheckIfInMainMenu())
        {
            Wander();
            return;
        }

        if (distanceToPlayer <= currentChaseRange)
        {
            returningToStart = false;
            if (distanceToPlayer <= agent.stoppingDistance)
            {
                StopChasingPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            if (!returningToStart && Vector3.Distance(transform.position, startingPosition) > 1f)
            {
                // Return to starting position after chase
                agent.SetDestination(startingPosition);

                // Only set chasing if actually moving
                enemyAnimations?.SetChasing(agent.remainingDistance > agent.stoppingDistance);
                returningToStart = true;
            }
            else if (returningToStart)
            {
                // Check if reached starting position
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    returningToStart = false;
                    StopChasingPlayer();
                }
            }
            else
            {
                Wander();
            }
        }
    }

    private void Wander()
    {
        // Only pick a new wander point if close to current destination
        if (Time.time >= nextWanderTime && (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath))
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startingPosition;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                
                if (CheckIfInMainMenu())
                {
                    enemyAnimations?.SetChasing(true);
                }
                else
                {
                    enemyAnimations?.SetChasing(agent.remainingDistance > agent.stoppingDistance);
                }
                
                SetNextWanderTime();
            }
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)
        {
            enemyAnimations?.SetChasing(false);
        }
    }

    private void SetNextWanderTime()
    {
        nextWanderTime = Time.time + Random.Range(minWanderWaitTime, maxWanderWaitTime);
    }

    private void ChasePlayer()
    {
        agent.SetDestination(FollowTraget.position);
        enemyAnimations?.SetChasing(true);
    }

    private void StopChasingPlayer()
    {
        agent.ResetPath();
        wasShot = false; // Reset the shot state when stopping chase
        enemyAnimations?.SetChasing(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, extendedChaseRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public void SetTarget(Transform traget)
    {
        FollowTraget = traget;
    }

    public float GetChaseRange()
    {
        return chaseRange;
    }

    public float GetExtendedChaseRange()
    {
        return extendedChaseRange;
    }

    public void SetChaseRange(float newChaseRange)
    {
        chaseRange = newChaseRange;
    }

    public void SetExtendedChaseRange(float newExtendedChaseRange)
    {
        extendedChaseRange = newExtendedChaseRange;
    }
}
