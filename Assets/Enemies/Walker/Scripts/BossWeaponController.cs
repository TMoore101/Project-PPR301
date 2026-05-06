using UnityEngine;

public class BossWeaponController : MonoBehaviour
{
    // General variables
    [SerializeField] private float angleToStartFiring;
    [SerializeField] private float rateOfFire;
    private float fireRateTimer;
    private Transform player;
    private BossController controller;
    // Bullet variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private float damage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private LayerMask ignoreLayer;
    // Animation variables
    private Animator animator;
    // Audio variables
    private AudioSource audioSource;
    [SerializeField] private AudioClip firingClip;

    //== On Start
    private void Start()
    {
        // Get components
        controller = GetComponent<BossController>();

        // Get player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get animator
        animator = GetComponent<Animator>();

        // Get audio source
        audioSource = GetComponent<AudioSource>();
    }

    //== On Update
    private void Update()
    {
        // If not activated, don't do anything
        if (!controller.isActivated) return;

        // Get direction to player
        Vector3 playerDir = player.position - transform.position;
        // Clear y direction
        playerDir.y = 0;

        // If angle to player is within the firing angle, start firing
        if (Vector3.Angle(transform.forward, playerDir) <= angleToStartFiring)
        {
            // If fire rate timer is greater than 0, decrease fire rate timer over time
            if (fireRateTimer > 0) fireRateTimer -= Time.deltaTime;
            // Else, fire a blast
            else
            {
                // Play shoot animation
                animator.SetTrigger("Shoot");

                // Play firing sound
                audioSource.clip = firingClip;
                audioSource.Play();

                // Reset fire rate timer
                fireRateTimer = rateOfFire;

                // Instantiate bullet prefab and set data
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                bullet.GetComponent<ExplosiveBullet>().damage = damage;

                // Get the distance to the player
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                // Get estimated player position
                Vector3 estimatedPlayerPosition = transform.position + (transform.forward * distanceToPlayer);
                // Aim bullet towards estimated player position
                bullet.transform.LookAt(estimatedPlayerPosition);

                // Add force to the bullet
                bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletSpeed, ForceMode.Impulse);
            }
        }
    }
}
