using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 0f;
    public float magnitude = 0.1f;

    private Vector3 originalPos;

    void Update()
    {
        if (duration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;
            duration -= Time.deltaTime;
        }
        else
        {
            duration = 0f;
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        this.duration = duration;
        this.magnitude = magnitude;
        originalPos = transform.localPosition;
    }
}