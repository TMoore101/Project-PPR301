using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // General variables
    public float health;

    //== Take Damage
    public void TakeDamage(float damage)
    {
        // Start chasing player
        GetComponent<GuardRobot>().currentState = EnemyState.Chasing;

        // Find all nearby enemies
        Collider[] hits = Physics.OverlapSphere(transform.position, 20);
        foreach (Collider col in hits)
        {
            // Get guard robot
            GuardRobot robot = col.GetComponentInParent<GuardRobot>();
            if (robot != null && robot.currentState != EnemyState.Deactivated)
            {
                // Set robot state to chasing player
                robot.currentState = EnemyState.Chasing;
            }
        }

        // Take damage out of health
        health -= damage;

        // If health is now less than 0, destroy object
        if (health <= 0)
            Destroy(gameObject);
    }
}
