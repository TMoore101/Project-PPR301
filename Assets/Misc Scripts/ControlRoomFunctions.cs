using System.Collections;
using UnityEngine;

public class ControlRoomFunctions : MonoBehaviour
{
    // General variables
    private Light[] lights;
    private float[] baseIntensities;
    // Light pulse variables
    [SerializeField] private float pulseAmount = 0.65f;
    [SerializeField] private float pulseSpeed = 3;
    // Audio variables
    [SerializeField] private AudioSource alarmAudioSource;

    //== Control Room Terminal Activated
    public void ControlRoomTerminalActivated()
    {
        // Get mission manager
        MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        // Move to next marker
        missionManager.NextMarker();

        // Get all building lights
        GameObject[] buildingLights = GameObject.FindGameObjectsWithTag("Building Light");

        // Setup building lights and base intensities
        lights = new Light[buildingLights.Length];
        baseIntensities = new float[buildingLights.Length];
        for (int i = 0; i < buildingLights.Length; i++)
        {
            // Get light component
            Light lightComponent = buildingLights[i].GetComponent<Light>();
            // Add building light to lights array
            lights[i] = lightComponent;
            baseIntensities[i] = lightComponent.intensity;
            // Set light color
            lights[i].color = Color.red;
        }

        // Start pulsing lights
        StartCoroutine(PulseLights());

        // Start alarm
        alarmAudioSource.Play();
    }

    //== Pulse Lights Coroutine
    private IEnumerator PulseLights()
    {
        while (true)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                {
                    lights[i].intensity = baseIntensities[i] * (1f + pulse);
                }
            }

            yield return null;
        }

    }
}
