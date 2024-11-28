using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RainLighting : MonoBehaviour 
{
    [Header("Lighting")]
    public Light2D globalLight;
    
    [Header("Lightning")]
    [SerializeField] private float flashIntensity = 1.5f;
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private float minTimeBetweenFlashes = 5f;
    [SerializeField] private float maxTimeBetweenFlashes = 15f;
    
    [Header("Rain")]
    public ParticleSystem rainParticles;
    [SerializeField] private float baseRainIntensity = 500f;
    [SerializeField] private float maxRainMultiplier = 3f; // Maximum multiplier for rain intensity
    
    [Header("Light Intensities")]
    [SerializeField] private float dayLightIntensity = 0.7f;
    [SerializeField] private float nightLightIntensity = 0.3f;
    [SerializeField] private float rainDimFactor = 0.7f;
    
    public GameTimer gameTimer;
    [Header("Thunder")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] thunderSounds; // Array of different thunder sounds
    [SerializeField] private float minThunderDelay = 0.1f; // Minimum delay between lightning and thunder
    [SerializeField] private float maxThunderDelay = 0.5f; // Maximum delay between lightning and thunder
    [SerializeField] [Range(0f, 1f)] private float thunderVolume = 1f;
        
    private float currentRainIntensity;
    private float normalLightIntensity;
    private bool isFlashing = false;
    
    void Start()
    {
        if (gameTimer == null)
        {
            Debug.LogError("GameTimer not found!");
            return;
        }
        
        GameTimer.OnHourChanged += OnHourChanged;
        
        // Initial rain setup
        UpdateRainIntensity();
        
        UpdateLighting();
        
        // Start lightning system
        StartCoroutine(LightningSystem());
    }
    
    void UpdateRainIntensity()
    {
        if (rainParticles == null) return;
        
        // Random multiplier between 1 and maxRainMultiplier
        float randomMultiplier = Random.Range(1f, maxRainMultiplier);
        currentRainIntensity = baseRainIntensity * randomMultiplier;
        
        var emission = rainParticles.emission;
        emission.rateOverTime = currentRainIntensity;
    }
    
    IEnumerator LightningSystem()
    {
        while (true)
        {
            // Wait random time before next flash
            float waitTime = Random.Range(minTimeBetweenFlashes, maxTimeBetweenFlashes);
            yield return new WaitForSeconds(waitTime);
            
            // Trigger lightning flash
            StartCoroutine(LightningFlash());
        }
    }
    
    IEnumerator LightningFlash()
    {
        if (isFlashing || globalLight == null) yield break;
        
        isFlashing = true;
        normalLightIntensity = globalLight.intensity;
        
        // Flash on
        globalLight.intensity = flashIntensity;

            // Start thunder with a slight delay
    if (audioSource != null && thunderSounds != null && thunderSounds.Length > 0)
    {
        float thunderDelay = Random.Range(minThunderDelay, maxThunderDelay);
        StartCoroutine(PlayThunderWithDelay(thunderDelay));
    }
        
        yield return new WaitForSeconds(flashDuration);
        
        // Flash off
        globalLight.intensity = normalLightIntensity;
        
        isFlashing = false;
    }

    IEnumerator PlayThunderWithDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    
    // Pick a random thunder sound from the array
    AudioClip thunderClip = thunderSounds[Random.Range(0, thunderSounds.Length)];
    
    // Set the volume and play the thunder
    audioSource.volume = thunderVolume;
    audioSource.PlayOneShot(thunderClip);
}
    
    void OnHourChanged(int hour)
    {
        UpdateLighting();
        UpdateRainIntensity(); // Update rain intensity every hour
    }
    
    void UpdateLighting()
    {
        if (globalLight == null || gameTimer == null) return;
        
        float baseIntensity;
        int currentHour = gameTimer.GetCurrentHour();
        
        if (currentHour >= 6 && currentHour < 18) // Day
        {
            baseIntensity = dayLightIntensity;
        }
        else // Night
        {
            baseIntensity = nightLightIntensity;
        }
        
        float finalIntensity = baseIntensity * rainDimFactor;
        
        // Only lerp if not currently flashing
        if (!isFlashing)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, finalIntensity, Time.deltaTime * 2f);
            normalLightIntensity = globalLight.intensity;
        }
    }
    
    void OnDestroy()
    {
        if (gameTimer != null)
        {
            GameTimer.OnHourChanged -= OnHourChanged;
        }
        StopAllCoroutines();
    }
}