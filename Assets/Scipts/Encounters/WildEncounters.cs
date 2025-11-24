using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class WildEncounters : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform player;

    [SerializeField] private Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);
    [SerializeField] private float triggerDistance = 15f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private int maxEnemies = 3;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isPlayerInRange = false;

    private const float RaycastStartHeight = 10f;
    private const float RaycastDistance = 20f;
    private const float GroundOffset = 0.1f;
    private const float NavMeshSampleMaxDistance = 1.0f;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Update()
    {       
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool wasInRange = isPlayerInRange;
        isPlayerInRange = distanceToPlayer <= triggerDistance;

        // Clean up destroyed enemies from the list
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (isPlayerInRange && !wasInRange)
        {
            TrySpawnEnemies();
        }
    }

    private void TrySpawnEnemies()
    {
        int aliveEnemies = spawnedEnemies.Count;

        // If all enemies are dead, spawn a new batch
        if (aliveEnemies == 0)
        {
            SpawnEnemies(maxEnemies);
        }
        // If we can afford, spawn more
        else if (aliveEnemies < maxEnemies)
        {
            SpawnEnemies(maxEnemies - aliveEnemies);
        }
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = transform.position + new Vector3(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2), 0f, Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2));

            Vector3 rayStart = randomPosition + Vector3.up * RaycastStartHeight;
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, RaycastDistance))
            {
                Vector3 spawnPosition = hit.point + Vector3.up * GroundOffset;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(spawnPosition, out navHit, NavMeshSampleMaxDistance, NavMesh.AllAreas))
                {
                    int prefabIndex = Random.Range(0, enemyPrefabs.Length);
                    GameObject enemy = Instantiate(enemyPrefabs[prefabIndex], navHit.position, Quaternion.identity);

                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    
                    if (enemyAI != null)
                    {
                        enemyAI.SetTarget(player);
                    }

                    spawnedEnemies.Add(enemy);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
