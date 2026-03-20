using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    // General variables
    [HideInInspector] public bool isActivated;
    [HideInInspector] public NavMeshAgent agent;
    private Transform player;
    // Movement variables
    [SerializeField] private float rotationSpeed;
    // Attack variables
    [SerializeField] private float stopDistance;

    //== On Start
    private void Start()
    {
        // Get components
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        // Disable rotation
        agent.updateRotation = false;

        // Get player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set agent stopping distance
        agent.stoppingDistance = stopDistance;
    }

    //== On Update
    private void Update()
    {
        // If not activated, don't do anything
        if (!isActivated) return;

        // Rotate towards target
        RotateTowardsTarget();

        // Move towards player
        agent.SetDestination(player.position);
    }

    //== Rotate Towards Target
    private void RotateTowardsTarget()
    {
        // Set target direction
        Vector3 targetDir = player.position - transform.position;
        // Clear y direction
        targetDir.y = 0;

        // If not facing the target, rotate towards target
        if (targetDir.sqrMagnitude > 0.001f)
        {
            // Get target rotation
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            // Rotate towards direction
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
