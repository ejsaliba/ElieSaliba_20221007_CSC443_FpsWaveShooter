using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;

    public event Action<EnemyHealth> OnDied;

    private Robot robot;

    private void Awake()
    {
        currentHealth = startingHealth;
        robot = GetComponent<Robot>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // --- SCORE SYSTEM ---
        if (ScoreManager.Instance != null && robot != null)
        {
            bool exploded = robot.enemyType == Robot.EnemyType.Exploder && robot.HasExploded;
            ScoreManager.Instance.RegisterKill(robot.enemyType, exploded);
        }

        OnDied?.Invoke(this);
    }

    public void OnGetFromPool()
    {
        currentHealth = startingHealth;
    }

    public void OnReturnFromPool()
    {
        OnDied = null;
    }
}