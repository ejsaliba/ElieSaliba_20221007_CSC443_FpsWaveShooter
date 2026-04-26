using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class Robot : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        Exploder
    }

    [Header("Type")]
    public EnemyType enemyType;

    [Header("Combat")]
    public int damage = 1;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    [Header("Ranged")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileForce = 20f;

    [Header("Exploder")]
    public float explosionRadius = 5f;
    public GameObject explosionVFX;

    private float lastAttackTime;
    private bool hasExploded = false;

    public bool HasExploded => hasExploded;

    NavMeshAgent agent;
    FirstPersonController player;
    PlayerHealth playerHealth;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (agent != null)
        {
            agent.enabled = false;
            agent.enabled = true;
            agent.Warp(transform.position);
        }

        if (player == null)
            player = FindAnyObjectByType<FirstPersonController>();

        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        hasExploded = false;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (agent.isOnNavMesh)
            agent.SetDestination(player.transform.position);

        switch (enemyType)
        {
            case EnemyType.Melee:
                HandleMelee(distance);
                break;

            case EnemyType.Ranged:
                HandleRanged(distance);
                break;

            case EnemyType.Exploder:
                HandleExploder(distance);
                break;
        }
    }

    void HandleMelee(float distance)
    {
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            playerHealth.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }

    void HandleRanged(float distance)
    {
        float stopDistance = Mathf.Max(attackRange - 2f, 2f);

        if (distance <= stopDistance)
        {
            agent.ResetPath();

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                ShootProjectile();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 spawnPos = firePoint.position + firePoint.forward * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        Vector3 direction = (player.transform.position - spawnPos).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileForce;
        }
    }

    void HandleExploder(float distance)
    {
        if (distance <= attackRange)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (playerHealth != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist <= explosionRadius)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null && eh.gameObject != gameObject)
            {
                eh.TakeDamage(damage);
            }
        }

        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        GetComponent<EnemyHealth>().TakeDamage(999);
    }
}