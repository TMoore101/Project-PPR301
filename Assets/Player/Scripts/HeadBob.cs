using UnityEngine;

public class HeadBob : MonoBehaviour
{
    // Bob speed
    public float frequency = 10f;   // speed of bob
    // Bob height
    public float amplitude = 0.05f; // height of bob
    // Smooth factor
    public float smooth = 10f;
    // Max move speed
    public float maxMoveSpeed = 3.25f;

    // Controller variables
    private Vector3 startPos;
    private float timer;

    // Components
    private Rigidbody playerRb;

    //== On Start
    void Start()
    {
        // Get start position
        startPos = transform.localPosition;

        // Get components
        playerRb = GetComponentInParent<Rigidbody>(); // adjust if needed
    }

    //== On Update
    void Update()
    {
        // Get horizontal velocity
        Vector3 horziontalVelocity = playerRb.linearVelocity;
        horziontalVelocity.y = 0f;
        // Get player's current speed
        float speed = horziontalVelocity.magnitude;

        // Normalize speed
        float speedMultiplier = Mathf.Pow(speed / maxMoveSpeed, 1.5f);

        // Scale bob frequency and amplitude by the speed multiplier
        float currentFrequency = frequency * speedMultiplier;
        float currentAmplitude = amplitude * speedMultiplier;

        // If player is moving, bob up and down
        if (speed > 0.1f)
        {
            // Increase timer by frequency
            timer += Time.deltaTime * currentFrequency;

            // Calculate bob changes
            float bobY = Mathf.Sin(timer) * currentAmplitude;
            float bobX = Mathf.Cos(timer * 0.5f) * currentAmplitude * 0.5f;

            // Calculate new bob position
            Vector3 targetPos = startPos + new Vector3(bobX, bobY, 0f);
            // Lerp to new bob position
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smooth);
        }
        // Else, return to start position
        else
        {
            // Clear timer
            timer = 0f;

            // Lerp to start position
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * smooth);
        }
    }
}