using Unity.VisualScripting;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    // General variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float openDistance;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform bossHealthTransform;
    // State variables
    private Vector3 loweredPos;
    private Vector3 raisedPos;
    [HideInInspector] public bool isActivated;

    //== On Start
    private void Start()
    {
        // Set closed and open positions
        loweredPos = platform.position;
        raisedPos = loweredPos + Vector3.up * openDistance;
    }

    //== On Update
    private void Update()
    {
        // If activated, move towards raised position
        if (isActivated)
        {
            platform.position = Vector3.MoveTowards(
                platform.position,
                raisedPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    //== On Trigger Enter
    private void OnTriggerEnter(Collider other)
    {
        // If other object is not the player, return
        if (other.gameObject.tag != "Player") return;

        if (!isActivated)
        {
            // Get mission manager
            MissionManager missionManager = GameObject.FindGameObjectWithTag("MissionManager").GetComponent<MissionManager>();
            // Move to next marker
            missionManager.NextMarker();

            // Enable boss health transform
            bossHealthTransform.gameObject.SetActive(true);

            // Activate platform
            isActivated = true;
        }
    }
}
