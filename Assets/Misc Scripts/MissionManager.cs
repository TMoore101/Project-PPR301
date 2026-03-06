using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    // General variables
    [SerializeField] private MissionMarker[] missionMarkers;
    private int currentMarker;
    // UI variables
    [SerializeField] private Image markerIcon;
    [SerializeField] private TextMeshProUGUI taskTitle;
    [SerializeField] private GameObject alertPopup;
    [SerializeField] private float alertTime;

    //== On Start
    private void Start()
    {
        currentMarker = 0;
        taskTitle.text = missionMarkers[currentMarker].taskName;
    }

    //== On Update
    private void Update()
    {
        // Get marker position in world space
        Vector3 worldPos = missionMarkers[currentMarker].transform.position;
        // Convert marker position into screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        // If marker is behind layer, invert it
        if (screenPos.z < 0)
            screenPos *= -1;

        // Set marker icon position to screen position
        markerIcon.GetComponent<RectTransform>().position = screenPos;
    }

    //== Next Marker
    public void NextMarker()
    {
        // Move to next marker
        if (currentMarker <= missionMarkers.Length)
        {
            // Set new current marker
            currentMarker++;
            
            // Set task title
            taskTitle.text = missionMarkers[currentMarker].taskName;

            // If task has an alert message, make alert popup
            if (missionMarkers[currentMarker].taskAlert != "")
            {
                // Activate alert popup
                alertPopup.SetActive(true);
                // Get alert message object
                TextMeshProUGUI alertMessageObject = alertPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                // Set alert message
                alertMessageObject.text = missionMarkers[currentMarker].taskAlert;

                // Hide alert after seconds
                StartCoroutine(HideAlertAfterSeconds(alertTime));
            }
        }
    }

    //== Hide Alert After Seconds
    private IEnumerator HideAlertAfterSeconds(float seconds)
    {
        // Wait time
        yield return new WaitForSeconds(seconds);
        // Hide alert popup
        alertPopup.SetActive(false);
    }
}
