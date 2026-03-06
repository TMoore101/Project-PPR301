using UnityEngine;

public class GeneratorBattery : MonoBehaviour
{
    // General variables
    public float health = 100;

    //== On Update
    private void Update()
    {
        // If battery is destroyed, hide battery object
        if (health <= 0)
            gameObject.SetActive(false);
    }
}
