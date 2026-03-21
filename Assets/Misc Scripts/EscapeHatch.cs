using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeHatch : MonoBehaviour
{
    // General variables
    private bool playerInRange = false;
    // UI variables
    [SerializeField] private GameObject interactMessage;

    // Input variables
    private Player_InputActions input;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;
    }
    
    //== On Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        // If collision object is not the player, return
        if (other.gameObject.tag != "Player") return;

        // Set player in range to true
        playerInRange = true;

        // Display interact message
        interactMessage.SetActive(true);
    }
    //== On Trigger Exit
    private void OnTriggerExit(Collider other)
    {
        // If collision object is not the player, return
        if (other.gameObject.tag != "Player") return;

        // Set player in range to false
        playerInRange = false;

        // Hide interact message
        interactMessage.SetActive(false);
    }

    //== On Update
    private void Update()
    {
        // If the player presses interact, escape facility
        if (playerInRange && input.Player.Interact.WasPressedThisFrame())
        {
            // Load demo complete scene
            SceneManager.LoadScene(1);
        }
    }
}
