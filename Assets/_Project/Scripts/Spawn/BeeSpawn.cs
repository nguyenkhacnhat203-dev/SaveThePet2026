
using System.Collections.Generic;
using UnityEngine;

public class BeeSpawn : MonoBehaviour
{
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private List<Transform> targetDogs = new List<Transform>();

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
            return;
        }

        if (spawnPoint == null)
        {
            return;
        }

        if (targetDogs == null || targetDogs.Count == 0)
        {
            return;
        }

        targetDogs.RemoveAll(dog => dog == null);

        if (targetDogs.Count == 0)
        {
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
            Destroy(beeObject);
            return;
        }

        BeeManager.Instance.RegisterBee(
            bee,
            spawnPoint.position,
            targetDogs
        );
    }
}
