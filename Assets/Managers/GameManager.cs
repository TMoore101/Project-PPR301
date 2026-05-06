using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Input variables
    private Player_InputActions input;
    // UI variables
    [SerializeField] private GameObject alertPopup;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;
    }

    //== On Update
    private void Update()
    {
        // If player presses the open alert keybind, open alert popup
        if (input.Player.OpenAlert.WasPressedThisFrame())
            alertPopup.SetActive(true);
        // If player releases the open alert keybind, close alert popup
        else if (input.Player.OpenAlert.WasReleasedThisFrame())
            alertPopup.SetActive(false);
    }
}
