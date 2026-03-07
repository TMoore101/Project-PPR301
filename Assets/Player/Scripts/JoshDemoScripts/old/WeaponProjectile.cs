using UnityEngine;

public class WeaponProjectile : WeaponBase
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        GameObject proj = Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = muzzlePoint.forward * projectileSpeed;
    }
}