using UnityEngine;

public class StorageRoomFunctions : MonoBehaviour
{
    //== Attempt Hanger Open
    public void AttemptHangerOpen()
    {
        // Get mission manager
        MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
        // Move to next marker
        missionManager.NextMarker();
    }
}
