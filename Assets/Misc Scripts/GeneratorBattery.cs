using UnityEngine;

public class GeneratorBattery : MonoBehaviour
{
    // General variables
    public float health = 100;
    [SerializeField] private MeshFilter generatorMesh;
    [SerializeField] private Mesh destroyedMesh;
    [SerializeField] private ParticleSystem explosionFX;

    //== On Update
    private void Update()
    {
        // If battery health is 0, destroy battery
        if (health <= 0)
        {
            // Hide game object
            gameObject.SetActive(false);
            // Swap to destroyed mesh
            generatorMesh.mesh = destroyedMesh;

            // Player explosion
            explosionFX.Play();
        }
    }
}
