using UnityEngine;
using UnityEngine.Audio;

public class ExplosiveBullet : MonoBehaviour
{
    // General variables
    [HideInInspector] public float damage;
    [SerializeField] private float projectileLife;
    [SerializeField] private AudioClip[] hitSounds;
    private float lifeTimer;
    private Vector3 previousPos;
    private Rigidbody rb;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float explosionRadius;
    [SerializeField] private GameObject explosionFX;

    //== On Start
    private void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody>();
        previousPos = transform.position;
    }

    //== On Update
    private void Update()
    {
        // Increase life timer over time
        lifeTimer += Time.deltaTime;
        // If life timer has reached the projectile life
        if (lifeTimer >= projectileLife) 
            Destroy(gameObject);

        // Get current position
        Vector3 currentPos = transform.position;
        // Get current movement
        Vector3 currentMovement = currentPos - previousPos;
        // Get distance for current movement
        float movementDistance = currentMovement.magnitude;

        // If the bullet is moving, try to hit target
        if (movementDistance > 0)
        {
            // Raycast forward
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, movementDistance))
            {
                // Handle hit
                HandleHit(hit.collider, hit.point, hit.normal);
                return;
            }
        }

        // Update previous position
        previousPos = currentPos;
    }

    //== Handle Hit
    private void HandleHit(Collider hitCollider, Vector3 hitPos, Vector3 hitNormal)
    {
        // If hit collider is a trigger, ignore hit
        if (hitCollider.isTrigger) return;

        // Create new audio source
        GameObject audio = new GameObject("SFX", typeof(AudioSource));
        audio.GetComponent<AudioSource>().spatialBlend = 1;
        audio.GetComponent<AudioSource>().outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        // Set audio source clip
        audio.GetComponent<AudioSource>().clip = hitSounds[Random.Range(0, hitSounds.Length)];
        // Play audio source
        audio.GetComponent<AudioSource>().Play();

        // Create explosion fx
        GameObject explosion = GameObject.Instantiate(explosionFX, transform.position, Quaternion.identity);

        // Get all objects in explosion range
        Collider[] explosionHits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in explosionHits)
        {
            // If explosion hit the player, damage player
            if (hit.CompareTag("Player"))
                hit.GetComponentInParent<PlayerHealth>().TakeDamage(damage);
        }

        // Destroy audio source
        Destroy(audio, audio.GetComponent<AudioSource>().clip.length);
        // Destroy bullet
        Destroy(gameObject);
    }
}
