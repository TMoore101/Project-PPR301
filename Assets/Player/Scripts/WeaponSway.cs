using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    // Max rotation variables
    public float maxRotationX = 5f;   // vertical (up/down tilt)
    public float maxRotationY = 5f;   // horizontal (left/right)
    // Smoothing variables
    public float inputScale = 0.5f;
    public float smoothTime = 0.08f;

    // Data variables
    private float currentX;
    private float currentY;
    private float velX;
    private float velY;

    // General variables
    private Quaternion startRot;
    private Player_InputActions input;

    //== On Start
    void Start()
    {
        // Get input
        input = InputHandler.instance.input;
        
        // Get start rotation
        startRot = transform.localRotation;
    }

    //== On Update
    void Update()
    {
        // Get look delta
        Vector2 lookDelta = input.Player.Look.ReadValue<Vector2>();

        // Get target rotations
        float targetY = Mathf.Clamp(-lookDelta.x * inputScale, -maxRotationY, maxRotationY);
        float targetX = Mathf.Clamp(lookDelta.y * inputScale, -maxRotationX, maxRotationX);

        // Apply smoothness to target rotations
        currentY = Mathf.SmoothDamp(currentY, targetY, ref velY, smoothTime);
        currentX = Mathf.SmoothDamp(currentX, targetX, ref velX, smoothTime);

        // Apply rotation
        transform.localRotation = startRot * Quaternion.Euler(0f, currentY, currentX);
    }
}