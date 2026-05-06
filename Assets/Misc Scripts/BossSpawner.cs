using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.Shapes;

public class BossSpawner : MonoBehaviour
{
    // General variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float openDistance;
    [SerializeField] private Transform platform;
    [SerializeField] private Transform bossHealthTransform;
    [SerializeField] private Door doorToLock;
    // State variables
    private Vector3 loweredPos;
    private Vector3 raisedPos;
    [HideInInspector] public bool isActivated;
    // Boss variables
    [SerializeField] private BossController bossController;

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

            // If platform is at the raised position & the boss hasn't been activated yet, activate boss
            if (platform.position == raisedPos && !bossController.isActivated)
            {
                bossController.isActivated = true;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(bossController.transform.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    bossController.agent.Warp(hit.position);
                    bossController.agent.enabled = true;
                }
            }
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

            // Lock door to lock
            doorToLock.isLocked = true;
            doorToLock.isOpening = false;
        }
    }
}
