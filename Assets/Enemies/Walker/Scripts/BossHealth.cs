using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    // General variables
    [SerializeField] private string bossName;
    [SerializeField] private float maxHealth;
    private float health;

    // Mission variables
    [SerializeField] private DoorTerminal nextMissionTerminal;

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
    public void TakeDamage(float damage)
    {
        // Take damage
        health -= damage;

        // If health reached zero, destroy boss
        if (health <= 0)
        {
            // Destroy boss
            Destroy(gameObject);

            // Get mission manager
            MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
            // Move to next marker
            missionManager.NextMarker();
    
            // Enable next mission terminal
            nextMissionTerminal.enabled = true;

            // Disable health slider
            healthSlider.enabled = false;
        }
    }
}
