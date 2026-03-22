using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneratorController : MonoBehaviour
{
    // General variables
    [SerializeField] private GeneratorBattery[] batteries;
    [SerializeField] private Door nextRoomDoor;
    private bool isDestroyed;
    // UI variables
    [SerializeField] private TextMeshProUGUI selfDestructTimerText;
    [SerializeField] private float selfDestructTime;
    private float currentSelfDestructTime;
    [SerializeField] private GameObject escapeHatch;

    //== On Start
    private void Start()
    {
        // Set current self destruct time
        currentSelfDestructTime = selfDestructTime;
    }

    //== On Update
    private void Update()
    {
        // Check if all batteries are destroyed
        bool generatorDestroyed = true;
        foreach (GeneratorBattery battery in batteries)
        {
            // If battery is not destroyed, mark generator destroyed as false
            if (battery.health > 0)
                generatorDestroyed = false;
        }

        // If all batteries are destroyed, unlock next room
        if (generatorDestroyed && !isDestroyed)
        {
            isDestroyed = true;
            nextRoomDoor.isLocked = false;

            // Get mission manager
            MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
            // Move to next marker
            missionManager.NextMarker();

            // Make self destruct timer text active
            selfDestructTimerText.gameObject.SetActive(true);
            // Start self destruct coroutine
            StartCoroutine(SelfDestruct());

            // Enable escape hatch
            escapeHatch.SetActive(true);
        } 
    }

    //== Self Destruct Coroutine
    private IEnumerator SelfDestruct()
    {
        while (currentSelfDestructTime > 0)
        {
            // Decrease self destruct time over time
            currentSelfDestructTime -= Time.deltaTime;

            // Get current self destruct time in minutes and seconds
            int minutes = Mathf.FloorToInt(currentSelfDestructTime / 60);
            int seconds = Mathf.FloorToInt(currentSelfDestructTime % 60);

            // Set self destruct timer text
            selfDestructTimerText.text = string.Format("{0}:{1:00}", minutes, seconds);

            yield return null;
        }

        // Load demo failed scene
        SceneManager.LoadScene(3);
    }
}
