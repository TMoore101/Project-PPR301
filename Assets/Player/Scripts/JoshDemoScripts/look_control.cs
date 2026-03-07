using UnityEngine;
using UnityEngine.InputSystem;

public class Look_Control : MonoBehaviour
{
    public Transform player;
    float xRot;
    float yRot;
    public float sensitivity = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouse = Mouse.current.delta.ReadValue() * sensitivity * Time.deltaTime;

        // Camera pitch
        xRot -= mouse.y;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        // Player yaw
        yRot += mouse.x;
        player.rotation = Quaternion.Euler(0, yRot, 0);
    }
}