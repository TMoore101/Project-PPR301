using UnityEngine;

public class BulletProjectile : MonoBehaviour
{


    public int damageAmount = 10;

    public float lifettime = 5f;
    public bool destoryOnImpact = true;
    public float lifetime = 5f;

    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //PLAYER HIT BY ENEMY
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }

            if (destoryOnImpact)
                Destroy(gameObject);

            return;
        }

        //ENEMY HIT BY PLAYER BULLET
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }

            if (destoryOnImpact)
                Destroy(gameObject);

            return;
        }


    }


   

    // Update is called once per frame
    void Update()
    {
        
    }
}
