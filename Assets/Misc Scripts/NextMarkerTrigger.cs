using UnityEngine;

public class NextMarkerTrigger : MonoBehaviour
{
    //== On Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        // If other object is not the player, return
        if (other.gameObject.tag != "Player") return;

        // Get mission manager
        MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        // Move to next marker
        missionManager.NextMarker();
    }
}
