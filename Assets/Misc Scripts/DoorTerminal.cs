using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.Shapes;

public class DoorTerminal : MonoBehaviour
{
    // General variables
    private bool isActivated;
    private bool playerInRange = false;
    [SerializeField] private Door[] doorsToUnlock;
    [SerializeField] private Door[] doorsToLock;
    [SerializeField] private float unlockTime;
    private float timeTillUnlocked;
    [SerializeField] private UnityEvent onTerminalActivated;
    // Spawner variables
    [SerializeField] private bool spawnWaves;
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
        if (other.gameObject.layer != 6) return;

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
        // If the player presses interact and the terminal is not already activated, activate terminal
        if (playerInRange && input.Player.Interact.WasPressedThisFrame() && !isActivated)
        {
            // Activate terminal
            isActivated = true;

            // Lock each door in doors to lock
            foreach (Door door in doorsToLock)
            {
                door.isLocked = true;
                door.isOpening = false;
            }

            // Activated on terminal activated event
            if (onTerminalActivated != null)
                onTerminalActivated?.Invoke();

            // Play interact sound
            GetComponent<AudioSource>().Play();
        }

        // If terminal is activated, count up
        if (isActivated && timeTillUnlocked < unlockTime)
            timeTillUnlocked += Time.deltaTime;

        // If time till unlocked has reached the unlock time, unlock door
        if (isActivated && timeTillUnlocked >= unlockTime)
        {
            // Unlock doors
            foreach (Door door in doorsToUnlock)
                door.isLocked = false;
        }
    }
}
