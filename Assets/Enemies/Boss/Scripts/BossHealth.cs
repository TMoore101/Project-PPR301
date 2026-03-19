using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    // General variables
    [SerializeField] private string bossName;
    [SerializeField] private float maxHealth;
    private float health;

    // UI variables
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private Slider healthSlider;

    //== On Start
    private void Start()
    {
        // Set name
        nameField.text = bossName;

        // Set health to max health
        health = maxHealth;
    }

    //== On Update
    private void Update()
    {
        // Set health slider value
        healthSlider.value = health / maxHealth;
    }

    //== Take Damage
    private void TakeDamage(float damage)
    {
        // Take damage
        health -= damage;

        // If health reached zero, destroy boss
        if (health <= 0)
            Destroy(gameObject);
    }
}
