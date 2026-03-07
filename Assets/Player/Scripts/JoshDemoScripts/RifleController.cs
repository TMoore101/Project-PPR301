using UnityEngine;

public class RifleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Camera cam;
    [SerializeField] GameObject bulletHole;

    [Header("Settings")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float fireRange = 300f;
    [SerializeField] LayerMask hitMask;

    bool canFire = true;
    FPS_Control input;

    void Awake()
    {
        input = new FPS_Control();
        input.Player_map.Enable();

        // Bind fire input
        input.Player_map.Fire.performed += ctx => TryFire();
    }

    void TryFire()
    {
        if (!canFire)
            return;

        Fire();
        canFire = false;
        Invoke(nameof(ResetFire), fireRate);
    }

    void Fire()
    {
        // Effects
        audioSource.Play();
        muzzleFlash.Play();

        // Raycast
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, fireRange, hitMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
            }
            else
            {
                Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
        }
    }

    void ResetFire()
    {
        muzzleFlash.Stop();
        canFire = true;
    }
}