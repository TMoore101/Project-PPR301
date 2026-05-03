using UnityEngine;

public class DestructableProp : MonoBehaviour
{
    // General variables
    [SerializeField] private float health = 30;

    //== On Update
    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    //== Take Damage
    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
