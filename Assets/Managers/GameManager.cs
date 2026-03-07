using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Input variables
    private Player_InputActions input;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;
    }

    //== On Update
    private void Update()
    {
        // If player presses pause key, quit application
        if (input.Player.Pause.WasPressedThisFrame())
            Application.Quit();
    }
}
