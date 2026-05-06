using UnityEngine;

public class MedStation : MonoBehaviour
{
    // General variables
    [SerializeField] private float healthGain = 1;
    private PlayerHealth playerHealth;
    private bool playerInRange = false;
    private bool hasActivated;
    // UI variables
    [SerializeField] private GameObject interactMessage;
    // Input variables
    private Player_InputActions input;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;

        // Get player health
        playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    //== On Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        // If collision object is not the player, return
        if (other.gameObject.layer != 6) return;

        // If already active, return
        if (hasActivated) return;

        // Set player in range to true
        playerInRange = true;

        // Display interact message
        interactMessage.SetActive(true);
    }
    //== On Trigger Exit
    private void OnTriggerExit(Collider other)
    {
        // If collision object is not the player, return
        if (other.gameObject.layer != 6) return;

        // Set palyer in range to false
        playerInRange = false;

        // Hide interact message
        interactMessage.SetActive(false);
    }

    //== On Update
    private void Update()
    {
        // If the player holds interact, increase player health
        if (playerInRange && input.Player.Interact.WasPressedThisFrame() && !hasActivated)
        {
            // Increase player health
            playerHealth.currentHealth += healthGain;
            playerHealth.currentHealth = Mathf.Clamp(playerHealth.currentHealth, 0, playerHealth.maxHealth);

            // Hide interact message
            interactMessage.SetActive(false);

            // Player activate sound effect
            GetComponent<AudioSource>().Play();
        }
    }
}
