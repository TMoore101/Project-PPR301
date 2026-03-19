using UnityEngine;
using UnityEngine.InputSystem;

public class Player_CameraController : MonoBehaviour
{
    // Camera variables
    [Header("Camera")]
    [SerializeField, Tooltip("Mouse movement sensitivity"), Range(0.01f, 0.2f)]
    private float mouseSensitivity = 2;
    [SerializeField, Tooltip("Minimum pitch rotation")]
    private float pitchClampMin = 90;
    [SerializeField, Tooltip("Maximum pitch rotation")]
    private float pitchClampMax = -85;
    [HideInInspector] public bool inControl = true;
    // Player variables
    [Header("Player")]
    [SerializeField, Tooltip("Player transform")]
    private Transform playerTransform;
    // Input variables
    private Player_InputActions input;
    // Data variables
    private float yaw;
    private float pitch;
    [HideInInspector] public float addedPitch;

    //== On Start
    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;

        // Lock mouse to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }

    //== On Update
    private void Update()
    {
        // If the player is not in control, return
        if (!inControl) return;

        // Get look delta
        Vector2 lookDelta = input.Player.Look.ReadValue<Vector2>();
        // Apply sensitivity
        yaw += lookDelta.x * mouseSensitivity;
        pitch -= lookDelta.y * mouseSensitivity;
        pitch += addedPitch * Time.deltaTime * 25;

        // Clamp pitch
        pitch = Mathf.Clamp(pitch, pitchClampMin, pitchClampMax);

        // Rotate camera & player
        playerTransform.localRotation = Quaternion.Euler(0, yaw, 0);
        transform.GetChild(0).localRotation = Quaternion.Euler(pitch, 0, 0);

        if (addedPitch < 0) addedPitch -= addedPitch * Time.deltaTime * 25;
    }
}
