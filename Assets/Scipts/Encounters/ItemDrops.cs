using UnityEngine;

public class ItemDrops : MonoBehaviour
{
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab;
        [Range(0, 100)]
        public float dropChance = 20f;
    }

    [SerializeField] private ItemDrop[] possibleDrops;
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float dropSpread = 1f;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            float enemyDestroyDelay = 0.5f;
            StartCoroutine(WatchHealth(enemyDestroyDelay));
        }
    }

    private System.Collections.IEnumerator WatchHealth(float delay)
    {
        while (enemyHealth != null && enemyHealth.GetCurrentHealth() > 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (enemyHealth != null)
        {
            DropRandomItems();
            yield return new WaitForSeconds(delay);
        }
    }

    private void DropRandomItems()
    {
        if (possibleDrops == null || possibleDrops.Length == 0) return;

        foreach (var drop in possibleDrops)
        {
            if (Random.Range(0f, 100f) <= drop.dropChance)
            {
                Vector3 randomOffset = Random.insideUnitSphere * dropSpread;
                randomOffset.y = Mathf.Abs(randomOffset.y); // Ensure items drop upward

                GameObject droppedItem = Instantiate(drop.itemPrefab, transform.position + randomOffset, Quaternion.identity);

                if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.AddForce((Vector3.up + randomOffset).normalized * dropForce, ForceMode.Impulse);
                }
            }
        }
    }
}
