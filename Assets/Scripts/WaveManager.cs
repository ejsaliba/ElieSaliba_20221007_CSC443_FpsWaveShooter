using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Spawners")]
    [SerializeField] private EnemySpawner spawnerA;
    [SerializeField] private EnemySpawner spawnerB;

    [Header("Barrier")]
    [SerializeField] private GameObject barrier;

    private EnemySpawner currentSpawner;

    private int currentWave = 0;
    private int enemiesAlive = 0;

    private bool waitingForPlayer = false;
    private bool waveInProgress = false;
    private bool waveEnded = false;

    private int activeZone = 0; // 0 = A, 1 = B

    public int CurrentWave => currentWave;
    public bool IsWaveInProgress => waveInProgress;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (spawnerA == null || spawnerB == null)
        {
            Debug.LogError("WaveManager: Missing spawners!");
            return;
        }

        if (barrier == null)
        {
            Debug.LogError("WaveManager: Missing barrier reference!");
            return;
        }

        StartNextWave();
    }

    // ---------------- WAVE START ----------------

    public void StartNextWave()
    {
        // 🔴 BLOCK START IF SHOP IS STILL OPEN
        if (ShopMenuKeyboard.Instance != null && ShopMenuKeyboard.Instance.IsOpen)
            return;

        currentWave++;

        waveEnded = false;
        waitingForPlayer = false;
        waveInProgress = true;

        // switch zones
        activeZone = 1 - activeZone;
        currentSpawner = (activeZone == 0) ? spawnerA : spawnerB;

        if (barrier != null)
            barrier.SetActive(true);

        SpawnWave();
    }

    // ---------------- SPAWN LOGIC ----------------

    void SpawnWave()
    {
        int melee = 0;
        int ranged = 0;
        int explosive = 0;

        if (currentWave == 1)
        {
            melee = 5;
        }
        else if (currentWave == 2)
        {
            melee = 5;
            ranged = 5;
        }
        else if (currentWave == 3)
        {
            melee = 5;
            ranged = 5;
            explosive = 2;
        }
        else if (currentWave == 4)
        {
            melee = 10;
            ranged = 5;
            explosive = 2;
        }
        else
        {
            int extra = (currentWave - 4) * 2;

            melee = 10 + extra;
            ranged = 5 + extra;
            explosive = 2 + extra;
        }

        enemiesAlive = melee + ranged + explosive;

        if (currentSpawner != null)
            currentSpawner.StartWave(melee, ranged, explosive);
        else
            Debug.LogError("WaveManager: Current spawner is NULL");
    }

    // ---------------- ENEMY TRACKING ----------------

    public void OnEnemyDied()
    {
        if (waveEnded) return;

        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            WaveCompleted();
        }
    }

    // ---------------- WAVE COMPLETE ----------------

    void WaveCompleted()
    {
        if (waveEnded) return;
        waveEnded = true;

        waveInProgress = false;
        waitingForPlayer = true;

        if (barrier != null)
            barrier.SetActive(false);

        // 🔥 OPEN SHOP CLEANLY (NO COROUTINE HACKS)
        if (ShopMenuKeyboard.Instance != null)
        {
            ShopMenuKeyboard.Instance.OpenShop();
        }
        else
        {
            Debug.LogError("ShopMenuKeyboard is NULL — cannot open shop");
        }
    }

    // ---------------- ZONE SYSTEM ----------------

    public void PlayerEnteredZone(int zoneIndex)
    {
        if (!waitingForPlayer) return;

        if (zoneIndex == activeZone)
        {
            StartNextWave();
        }
    }
}