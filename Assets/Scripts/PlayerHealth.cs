using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 15;
    private int currentHealth;

    private bool isDead = false;

public GameObject deathVFX; // Assign your VFX prefab here
    public float destroyDelay = 0.1f; // Small delay before destroying player
    
    [Header("Healing Ability")]
    [SerializeField] private int healAmount = 5;
    [SerializeField] private float healCooldown = 20f;

    private float healTimer = 0f;

    public bool IsHealOnCooldown => healTimer > 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        // cooldown
        if (healTimer > 0f)
        {
            healTimer -= Time.deltaTime;
        }

        // input (new input system)
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame &&
            healTimer <= 0f)
        {
            Heal(healAmount);
            healTimer = healCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // clamp immediately
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Player Died");

                // Spawn VFX if assigned
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, transform.rotation);
        }
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOver();
        }

        // Destroy player after short delay (lets VFX spawn properly)
        Destroy(gameObject, destroyDelay);

       
    }

    public int GetHPValue()
    {
        return currentHealth;
    }
}