using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public float fireRate = 0.2f;
    public int damage = 10;
    public Transform muzzlePoint;

    protected float nextFireTime = 0f;

    public abstract void Fire();

}
