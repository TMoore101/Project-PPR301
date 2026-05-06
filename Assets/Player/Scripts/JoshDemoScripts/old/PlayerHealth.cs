using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100, currentHealth;
    [SerializeField] private float timeToHeal;
    [SerializeField] private float healRate;
    [SerializeField] private Slider healthSlider;

    public delegate void PlayerDeathEvent();
    public static event PlayerDeathEvent OnPlayerDeath;
    private float timeSinceCombat;

    [SerializeField] private Volume postProcessVolume;
    private Vignette healthVignette;
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private AudioClip hurtAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        // Get health vignette
        if (postProcessVolume.profile.TryGet<Vignette>(out healthVignette)) { }
    }

    // Update is called once per frame
    void Update()
    {
        // Increase time since combat
        timeSinceCombat += Time.deltaTime;

        // If time since combat is greater than the time to heal, heal player
        if (timeSinceCombat >= timeToHeal)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += healRate * Time.deltaTime;
            }
        }

        // Set health slider value
        healthSlider.value = currentHealth / maxHealth;

        // Set health vignette intensity
        healthVignette.intensity.value = Mathf.Clamp01(Mathf.InverseLerp(1, 0.3f, currentHealth / maxHealth)) * maxVignetteIntensity;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        timeSinceCombat = 0;

        if (Random.Range(0, 5) == 0)
        {
            AudioSource.PlayClipAtPoint(hurtAudio, transform.position);
        }

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        OnPlayerDeath?.Invoke();

        GetComponent<Player_MovementController>().enabled = false;

        // Load demo complete scene
        SceneManager.LoadScene(3);
    }
}
