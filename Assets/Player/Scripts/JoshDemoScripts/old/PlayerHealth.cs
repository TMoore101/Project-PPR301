using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100, currentHealth;
    [SerializeField] private float timeToHeal;
    [SerializeField] private float healRate;
    [SerializeField] private Slider healthSlider;

    public delegate void PlayerDeathEvent();
    public static event PlayerDeathEvent OnPlayerDeath;
    private float timeSinceCombat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Increase time since combat
        timeSinceCombat += Time.deltaTime;

        // If time since combat is greater than the time to heal, heal player
        if (timeSinceCombat >= timeToHeal)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += healRate * Time.deltaTime;
            }
        }

        healthSlider.value = currentHealth / maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player took damage; " + amount + " | Current Health: " + currentHealth);

        timeSinceCombat = 0;

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

        // Load demo complete scene
        SceneManager.LoadScene(2);
    }
}
