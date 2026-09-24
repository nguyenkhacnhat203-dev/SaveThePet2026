using UnityEngine;

public class BeeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Transform targetDog;

    [SerializeField] private int beeCount = 10;
    [SerializeField] private float totalSpawnTime = 1f;

    private int spawnedCount;
    private float spawnTimer;
    private float spawnInterval;
    private bool isSpawning;

    public void StartSpawn()
    {
        if (beePrefab == null)
        {
            Debug.LogError("BeeSpawn: Bee Prefab chưa được gán.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("BeeSpawn: Spawn Point chưa được gán.");
            return;
        }

        if (targetDog == null)
        {
            Debug.LogError("BeeSpawn: Target Dog chưa được gán.");
            return;
        }

        if (beeCount <= 0)
            return;

        spawnedCount = 0;
        spawnTimer = 0f;
        spawnInterval = totalSpawnTime / beeCount;

        isSpawning = true;
    }

    public void Tick(float deltaTime)
    {
        if (!isSpawning)
            return;

        spawnTimer += deltaTime;

        while (spawnedCount < beeCount && spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            SpawnBee();
            spawnedCount++;
        }

        if (spawnedCount >= beeCount)
        {
            isSpawning = false;
        }
    }

    private void SpawnBee()
    {
        GameObject beeObject = Instantiate(
            beePrefab,
            spawnPoint.position,
            Quaternion.identity,
            transform 
        );

        Bee bee = beeObject.GetComponent<Bee>();

        if (bee == null)
        {
            Debug.LogError("Bee Prefab không có component Bee.");
            return;
        }

        BeeManager.Instance.RegisterBee(
            bee,
            spawnPoint.position,
            targetDog
        );
    }
}