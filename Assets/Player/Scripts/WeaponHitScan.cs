using UnityEngine;

public class WeaponHitscan : WeaponBase
{
    [Header("Hitscan Settings")]
    public float range = 100f;
    public LayerMask hitMask;

    public override void Fire()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out RaycastHit hit, range, hitMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
        }
    }
}