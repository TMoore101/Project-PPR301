using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int maxHealth = 100, 
    currentHealth;

    public delegate void PlayerDeathEvent();
    public static event PlayerDeathEvent OnPlayerDeath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took damage; " + amount + " | Current Health: " + currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        OnPlayerDeath?.Invoke();

        GetComponent<Player_MovementController>().enabled = false;


        //Disablemovement?
        //show UI?
        
    }
}
