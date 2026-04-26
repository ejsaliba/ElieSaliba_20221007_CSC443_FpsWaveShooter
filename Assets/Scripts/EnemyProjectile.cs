using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 40f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

   private void OnTriggerEnter(Collider other)
{
    Debug.Log("HIT OBJECT: " + other.name);

    PlayerHealth player = other.GetComponent<PlayerHealth>();

    Debug.Log("PLAYER HEALTH FOUND? " + (player != null));

    if (player != null)
    {
        player.TakeDamage(damage);
        Debug.Log("DAMAGE APPLIED");
    }

    Destroy(gameObject);
}
}



   
