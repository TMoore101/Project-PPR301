using UnityEngine;
using UnityEngine.Windows;

public class InputHandler : MonoBehaviour
{
    // Instance variables
    [HideInInspector] public static InputHandler instance { get; private set; }
    // Input variables
    [HideInInspector] public Player_InputActions input { get; private set; }

    //== On Awake
    private void Awake()
    {
        // If the public instance does not already exist, set this version as the public instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Else, destroy this version
        else
            Destroy(gameObject);

        
        // Enable default input actions
        input = new Player_InputActions();
        input.Player.Enable();
        input.UI.Disable();
    }
    //== On Destroy
    private void OnDestroy()
    {
        // If this version is the public instance, destroy this version
        if (instance == this)
        {
            input.Dispose();
            instance = null;
        }
    }
}
