using UnityEngine;

public class NextMarkerTrigger : MonoBehaviour
{
    // General variables
    private bool isActivated;

    //== On Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        // If other object is not the player, return
        if (other.gameObject.tag != "Player") return;

        // If already activated, return
        if (isActivated == true) return;

        // Get mission manager
        MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        // Move to next marker
        missionManager.NextMarker();

        // Activate trigger
        isActivated = true;
    }
}
