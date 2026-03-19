using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    //Lifetime variables
    public float projectileLife;
    private float timer;

    //Damage variable
    public float damage;

    public bool isEnemyBullet;


    public GameObject HitWall;
    public GameObject HitEnemy;
    public GameObject HitShip;

    [SerializeField]
    private AudioClip[] wallHitNoises;
    [SerializeField]
    private AudioClip[] playerHitNoises;
    [SerializeField]
    private AudioClip[] enemyHitNoises;
    [SerializeField]
    private AudioClip[] shipHitNoises;
    [SerializeField]
    private AudioSource audioSource;

    private Vector3 previousPos;
    private Rigidbody rb;

    //== On Start
    private void Start()
    {
        // Get data
        rb = GetComponent<Rigidbody>();
        previousPos = transform.position;
    }

    //Wait until the timer runs out, then destroy the bullet
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= projectileLife)
            Destroy(gameObject);

        Vector3 currentPosition = transform.position;
        Vector3 movement = currentPosition - previousPos;
        float distance = movement.magnitude;

        if (distance > 0f)
        {
            // Raycast forward
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                HandleHit(hit.collider, hit.point, hit.normal);
                return;
            }
        }

        previousPos = currentPosition;
    }

    // If the bullet hits an object, destroy the bullet
    private void HandleHit(Collider other, Vector3 hitPos, Vector3 hitNormal) {
        if (other.isTrigger) return;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

        GameObject audio = new GameObject("SFX", typeof(AudioSource));
        audio.GetComponent<AudioSource>().spatialBlend = 1;
        audio.GetComponent<AudioSource>().outputAudioMixerGroup = audioSource.outputAudioMixerGroup;

        if (isEnemyBullet)
        {
            //If the bullet hits the player, decrease their health by the bullet's damage
            if (other.gameObject.tag == "Player") {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
                audio.GetComponent<AudioSource>().clip = playerHitNoises[Random.Range(0, playerHitNoises.Length)];
            }
            else {
                GameObject hitParticle = Instantiate(HitWall);
                hitParticle.transform.position = hitPos;
                hitParticle.transform.rotation = rotation;

                audio.GetComponent<AudioSource>().clip = wallHitNoises[Random.Range(0, wallHitNoises.Length)];
            }
        }
        else
        {
            if (other.gameObject.tag == "Enemy") {
                other.transform.parent.GetComponent<EnemyHealth>().TakeDamage(damage);
                GameObject hitParticle = Instantiate(HitEnemy);
                hitParticle.transform.position = hitPos;
                hitParticle.transform.rotation = rotation;

                audio.GetComponent<AudioSource>().clip = enemyHitNoises[Random.Range(0, enemyHitNoises.Length)];
            }

            else if (other.gameObject.tag == "Generator")
            {
                other.GetComponent<GeneratorBattery>().health -= damage;
                GameObject hitParticle = Instantiate(HitWall);
                hitParticle.transform.position = hitPos;
                hitParticle.transform.rotation = rotation;

                audio.GetComponent<AudioSource>().clip = wallHitNoises[Random.Range(0, wallHitNoises.Length)];
            }

            else {
                GameObject hitParticle = Instantiate(HitWall);
                hitParticle.transform.position = hitPos;
                hitParticle.transform.rotation = rotation;

                audio.GetComponent<AudioSource>().clip = wallHitNoises[Random.Range(0, wallHitNoises.Length)];
            }
        }

        audio.GetComponent<AudioSource>().Play();

        //Destroy bullet
        Destroy(audio, audio.GetComponent<AudioSource>().clip.length);
        Destroy(gameObject);
    }
}
