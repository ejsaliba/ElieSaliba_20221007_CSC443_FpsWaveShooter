using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Types (ORDER MATTERS)")]
    [SerializeField] private EnemyHealth meleePrefab;
    [SerializeField] private EnemyHealth rangedPrefab;
    [SerializeField] private EnemyHealth explosivePrefab;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private int maxActiveEnemies = 15;

    private ObjectPool<EnemyHealth> meleePool;
    private ObjectPool<EnemyHealth> rangedPool;
    private ObjectPool<EnemyHealth> explosivePool;

    private int totalToSpawn;
    private int spawned;

    private void Awake()
    {
        meleePool = new ObjectPool<EnemyHealth>(meleePrefab, transform, 5);
        rangedPool = new ObjectPool<EnemyHealth>(rangedPrefab, transform, 5);
        explosivePool = new ObjectPool<EnemyHealth>(explosivePrefab, transform, 5);
    }

    public void StartWave(int melee, int ranged, int explosive)
    {
        StopAllCoroutines();

        totalToSpawn = melee + ranged + explosive;
        spawned = 0;

        StartCoroutine(SpawnRoutine(melee, ranged, explosive));
    }

    IEnumerator SpawnRoutine(int melee, int ranged, int explosive)
    {
        int m = 0, r = 0, e = 0;

        while (spawned < totalToSpawn)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GetActiveCount() >= maxActiveEnemies)
                continue;

            if (m < melee)
            {
                Spawn(meleePool);
                m++;
            }
            else if (r < ranged)
            {
                Spawn(rangedPool);
                r++;
            }
            else if (e < explosive)
            {
                Spawn(explosivePool);
                e++;
            }

            spawned++;
        }
    }

    void Spawn(ObjectPool<EnemyHealth> pool)
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyHealth enemy = pool.Get(point.position, point.rotation);

        enemy.OnDied += (e) =>
        {
            pool.Return(e);
            WaveManager.Instance.OnEnemyDied();
        };
    }

    int GetActiveCount()
    {
        return meleePool.CountActive + rangedPool.CountActive + explosivePool.CountActive;
    }
}