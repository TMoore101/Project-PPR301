using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

//== Enemy State
public enum EnemyState
{
    Patrolling,
    Waiting,
    Chasing,
    Attacking,
    Deactivated
}

public class GuardRobot : MonoBehaviour
{
    // General variables
    [SerializeField] private GameObject model;
    [SerializeField] public EnemyState currentState;
    private Vector3 spawnPosition;
    private NavMeshAgent agent;
    [SerializeField] private float rotationSpeed;
    // Patrol variables
    [SerializeField] private float patrolRadius;
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxwaitTime;
    private float waitTimer;
    // Attack variables
    [SerializeField] private float detectionRadius;
    [SerializeField] private float stopDistance;
    [SerializeField] private float loseTargetTime;
    private Transform player;
    private float timeSincePlayerVisible;
    // Deactivated variables
    [SerializeField] private float activationDistance;
    [SerializeField] private Material activatedMat;
    [SerializeField] private Material deactivatedMat;
    private bool deactivationComplete;
    public bool isActive;
    // Audio variables
    [SerializeField] private AudioClip[] passiveAudioClips;
    [SerializeField] private AudioSource voiceBox;
    private float voiceTimer;
    // Animation variables
    [SerializeField] public Animator animator;

    //== On Start
    private void Start()
    {
        // Get spawn position
        spawnPosition = transform.position;

        // Get agent
        agent = GetComponent<NavMeshAgent>();
        // Disable rotation
        agent.updateRotation = false;

        // Set initial patrol point
        if (currentState == EnemyState.Patrolling)
            SetNewPatrolPoint();

        // If current state is deactivated, disable agent
        if (currentState == EnemyState.Deactivated)
            agent.enabled = false;

        // Get player
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Play voices
        if (voiceBox != null && isActive)
            StartCoroutine(PlayVoice());
    }

    //== On Update
    private void Update()
    {
        // Rotate towards target
        RotateTowardsTarget();

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        // If the distance to player is less than the detection radius, start chasing the player
        if (currentState == EnemyState.Patrolling && distanceToPlayer <= detectionRadius || currentState == EnemyState.Waiting && distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Chasing;
            timeSincePlayerVisible = 0;
        }

        // Check current state
        switch (currentState)
        {
            // Patrolling
            case EnemyState.Patrolling:
                // Set stopping distance
                agent.stoppingDistance = 0;

                // If the agent has reached the end of their patrol point, start waiting
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Waiting;
                    waitTimer = Random.Range(minWaitTime, maxwaitTime);
                }

                animator.SetBool("isMoving", true);
                break;
            // Waiting
            case EnemyState.Waiting:
                // Decrease wait timer over time
                waitTimer -= Time.deltaTime;

                animator.SetBool("isMoving", false);

                // If wait timer has reached the end, set a new patrol point
                if (waitTimer <= 0)
                    SetNewPatrolPoint();
                break;
            // Chasing
            case EnemyState.Chasing:
                // Set stopping distance
                agent.stoppingDistance = stopDistance;

                // Get shooting component
                EnemyShooting enemyShooting = GetComponent<EnemyShooting>();

                // If the distance to the player is greater than the detection radius, wait until lose target time to start patrolling again
                if (distanceToPlayer > detectionRadius)
                {
                    // Increase time since player visible over time
                    timeSincePlayerVisible += Time.deltaTime;

                    // If the time since player visible has reached the lose target time, start waiting
                    if (timeSincePlayerVisible >= loseTargetTime)
                    {
                        // Set current state to waiting
                        currentState = EnemyState.Waiting;
                        waitTimer = Random.Range(minWaitTime, maxwaitTime);

                        // Stop shooting
                        enemyShooting.Shoot = false;
                    }
                }

                if (distanceToPlayer <= stopDistance)
                {
                    animator.SetBool("isMoving", false);
                }

                // Raycast forward
                RaycastHit hit;
                if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 100f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Start shooting
                        enemyShooting.Shoot = true;
                    }
                }
                 
                // Move to player
                if (agent.isOnNavMesh)
                    agent.SetDestination(player.position);
                break;
            // Deactivated
            case EnemyState.Deactivated:
                // If the robot is not yet fully deactivated, deactivate robot
                if (!deactivationComplete)
                {
                    // Mark deactivation as complete
                    deactivationComplete = true;
                    // Set headset material to deactivated material
                    model.GetComponent<MeshRenderer>().materials[2] = deactivatedMat;
                }

                // Get distance to player
                float distToPlayer = Vector3.Distance(player.position, transform.position);
                // If distance to player is less than the activation distance, activate robot
                if (distToPlayer < activationDistance && !isActive)
                {
                    isActive = true;
                    // Activate agent
                    MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();
                    Material[] materials = meshRenderer.materials;
                    model.GetComponent<Rigidbody>().isKinematic = false;
                    // Set headset material to activated material
                    materials[2] = activatedMat;
                    meshRenderer.materials = materials;
                    StartCoroutine(ActivateAgent());
                }
                break;
        }
    }

    //== Play Voice
    private IEnumerator PlayVoice()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 6));

            // Pick a random voice clip
            AudioClip voiceClip = passiveAudioClips[UnityEngine.Random.Range(0, passiveAudioClips.Length)];
            voiceBox.clip = voiceClip;

            // Choose a random start time
            float maxStartTime = Mathf.Max(0, voiceClip.length - 2);
            float startTime = UnityEngine.Random.Range(0, maxStartTime);

            // Set voice clip start time
            voiceBox.time = startTime;
            // Play voice clip
            voiceBox.Play();

            // Decide how long to play clip
            float playDuration = UnityEngine.Random.Range(2, 5);
            // Clamp duration so it doesn't exceed clip length
            playDuration = Mathf.Min(playDuration, voiceClip.length - startTime);

            yield return new WaitForSeconds(playDuration);

            // Stop clip after the duration
            voiceBox.Stop();
        }
    }

    //== Activate Agent
    private IEnumerator ActivateAgent()
    {
        // Step 1: Move forward 1 meter manually
        float moved = 0f;
        float distanceToMove = 1f;
        float speed = 0.65f;

        while (moved < distanceToMove)
        {
            float step = speed * Time.deltaTime;
            if (moved + step > distanceToMove) step = distanceToMove - moved;

            transform.position += transform.forward * step;
            moved += step;

            yield return null;
        }

        // Step 2: Snap to nearest NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        // Step 3: Enable agent safely
        agent.enabled = true;
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = false;

        // Step 4: Start patrol
        currentState = EnemyState.Waiting;
        waitTimer = Random.Range(minWaitTime, maxwaitTime);
        SetNewPatrolPoint();

        if (voiceBox != null)
            StartCoroutine(PlayVoice());
    }

    //== Set new Patrol Point
    private void SetNewPatrolPoint()
    {
        // Get a random point inside patrol radius
        Vector3 randomPoint = spawnPosition + Random.insideUnitSphere * patrolRadius;

        // Set destination to nearest valid position from random point
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            // Set empty nav mesh path
            NavMeshPath path = new NavMeshPath();

            // If the path is complete, set agent destination and start patrolling
            if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                currentState = EnemyState.Patrolling;
                return;
            }
        }

        // If patrol position is invalid, start waiting and try again
        currentState = EnemyState.Waiting;
        waitTimer = 2;
    }

    //== Rotate Towards Target
    private void RotateTowardsTarget()
    {
        // Set empty direction
        Vector3 direction = Vector3.zero;

        // If current state is chasing, set direction to the face the player
        if (currentState == EnemyState.Chasing || currentState == EnemyState.Attacking)
            direction = player.position - transform.position;
        // Else, set direction to face agent path
        else if (agent.velocity.sqrMagnitude > 0.01f)
            direction = agent.velocity;
        
        // Clear y direction
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            // Get target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Rotate towards direction
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
