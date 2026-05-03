using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    // General variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float openDistance;
    [SerializeField] private LayerMask interactableLayers;
    // Lock variables
    public bool isLocked;
    [SerializeField] private GameObject lockLightModel;
    [SerializeField] private Material unlockedMat;
    [SerializeField] private Material lockedMat;
    // State variables
    private Vector3 closedPos;
    private Vector3 openPos;
    [HideInInspector] public bool isOpening = false;
    public bool isForcedOpen;

    //== On Start
    private void Start()
    {
        // If door is not forced open, set closed and open positions using current position as closed
        if (!isForcedOpen)
        {
            // Set closed and open positions
            closedPos = transform.GetChild(0).Find("doorModel").position;
            openPos = closedPos + Vector3.up * openDistance;
        }
        // Else, set closed and open positions using current position as open
        else
        {
            // Set closed and open positions
            openPos = transform.GetChild(0).Find("doorModel").position;
            closedPos = openPos + Vector3.down * openDistance;
            // Mark as opening
            isOpening = true;
        }
    }

    //== On Update
    private void Update()
    {
        // Get door model
        Transform doorModel = transform.GetChild(0).Find("doorModel");

        // Update door obstacle for navmesh
        doorModel.GetComponent<NavMeshObstacle>().carving = isLocked;

        // If the lock materials and lock light model exists, set lock material
        if (lockLightModel != null && lockedMat != null && unlockedMat != null)
            lockLightModel.GetComponent<MeshRenderer>().material = isLocked ? lockedMat : unlockedMat;

        // Get target position based on if door is opening or not
        Vector3 targetPos = isOpening ? openPos : closedPos;

        // Move door towards target position
        doorModel.position = Vector3.MoveTowards(
            doorModel.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );
    }

    //== On Trigger Enter
    private void OnTriggerEnter(Collider collider)
    {
        // If door is locked, prevent opening
        if (isLocked) return;

        // If collider's layer is in the interactable layers, open door
        if ((interactableLayers & (1 << collider.gameObject.layer)) != 0)
        {
            isOpening = true;

            GetComponent<AudioSource>().Play();
        }
    }
    //== On Trigger Exit
    private void OnTriggerExit(Collider collider)
    {
        // If door is locked, prevent closing
        if (isLocked) return;

        // If collider's layer is in the interactable layers, close door
        if ((interactableLayers & (1 << collider.gameObject.layer)) != 0)
            isOpening = false;
    }
}
