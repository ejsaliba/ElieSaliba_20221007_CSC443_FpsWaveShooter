using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public int damage = 50;
    public float radius = 5f;
    public GameObject vfx;

    public float lifetime = 5f;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // prevent instant explosion on spawn
        if (Time.time < spawnTime + 0.1f) return;

        Explode();
    }

    void Explode()
    {
        // damage enemies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // spawn VFX
        if (vfx != null)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}