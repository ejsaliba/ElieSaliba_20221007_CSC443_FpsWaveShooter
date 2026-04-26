using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponData weaponData;
    private Weapon currentWeapon;
    StarterAssetsInputs inputs;
    [SerializeField] Animator animator;

    InputAction shootAction;
    FirstPersonController controller;

    const string SHOOT_ANIMATION_TRIGGER = "Shoot";
    float nextFireTime = 0f;

    public Weapon CurrentWeapon => currentWeapon;

    private void Awake()
    {
        currentWeapon = null;
        inputs = GetComponentInParent<StarterAssetsInputs>();
        shootAction = GetComponentInParent<PlayerInput>().actions["Shoot"];
        controller = GetComponentInParent<FirstPersonController>();
    }

    private void Update()
    {
        HandleShoot();
    }

    private void HandleShoot()
    {
        if (ShopMenuKeyboard.Instance != null && ShopMenuKeyboard.Instance.IsOpen)
    return;
        bool canFire = Time.time >= nextFireTime;

        if (!canFire || currentWeapon == null || !currentWeapon.HasAmmo)
            return;

        if (weaponData.isAutomatic)
        {
            if (!shootAction.IsPressed()) return;
        }
        else
        {
            if (!inputs.shoot) return;
            inputs.ShootInput(false);
        }

        nextFireTime = Time.time + (1.0f / weaponData.fireRate);

        animator.Play(SHOOT_ANIMATION_TRIGGER, 0, 0f);
        currentWeapon.Shoot();

        float yawKick = Random.Range(-weaponData.recoilX, weaponData.recoilX);
        controller.ApplyRecoil(weaponData.recoilY, yawKick);

        if (weaponData.isProjectileWeapon)
        {
            ShootProjectile();
        }
        else
        {
            ShootHitscan();
        }
    }

    void ShootHitscan()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position,
            Camera.main.transform.forward,
            out hit,
            weaponData.range))
        {
            EnemyHealth health = hit.collider.GetComponentInParent<EnemyHealth>();
            health?.TakeDamage(weaponData.damage);

            if (weaponData.hitVFXPrefab != null)
                Instantiate(weaponData.hitVFXPrefab, hit.point, Quaternion.identity);
        }
    }

    void ShootProjectile()
    {
        if (weaponData.projectilePrefab == null) return;

        Transform cam = Camera.main.transform;

        // spawn in front of camera (prevents instant collision)
        Vector3 spawnPos = cam.position + cam.forward * 1.0f;

        GameObject proj = Instantiate(
            weaponData.projectilePrefab,
            spawnPos,
            Quaternion.LookRotation(cam.forward)
        );

        // move projectile
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = cam.forward * weaponData.projectileForce;
        }

        // 🔥 IGNORE PLAYER COLLISION (CharacterController safe)
        Collider projCol = proj.GetComponent<Collider>();
        CharacterController cc = GetComponentInParent<CharacterController>();

        if (projCol != null && cc != null)
        {
            Physics.IgnoreCollision(projCol, cc);
        }

        // pass weapon data to projectile
        RocketProjectile rocket = proj.GetComponent<RocketProjectile>();
        if (rocket != null)
        {
            rocket.damage = weaponData.damage;
            rocket.radius = weaponData.explosionRadius;
            rocket.vfx = weaponData.hitVFXPrefab;
        }
    }

    public void SwitchWeapon(Weapon newWeapon)
    {
        if (newWeapon == null) return;

        currentWeapon = newWeapon;
        weaponData = newWeapon.Data;
        nextFireTime = 0;

        inputs.ShootInput(false);
    }
}