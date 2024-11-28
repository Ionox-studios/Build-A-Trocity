using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
public class HeartTimer : MonoBehaviour
{
    [Header("Heart Settings")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private HorizontalLayoutGroup heartContainer;
    [SerializeField] public float timerDuration = 60f; // Duration per heart in seconds
    [SerializeField] private float bleedRate = 0.5f; // How often blood particles spawn
    
    [Header("Particle Settings")]
    [SerializeField] private Color bloodColor = Color.red;
    [SerializeField] private float bloodParticleSize = 0.2f;

   [Header("Game Over Effects")]
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float lightIntensityIncrease = 2f;
    [SerializeField] private float lightChangeDuration = 1f;
    [SerializeField] private string badEndingSceneName = "BadEnding";
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    public Light2D globalLight;
    private Color originalLightColor;
    private float originalLightIntensity;




    private HeartIndicator[] hearts;
    public int maxHealth;
    private int currentHealth;
    private float currentHeartTimer;
    
    void Start()
    {   
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.position;
        // Set up the red overlay
        if (globalLight != null)
        {
            originalLightColor = globalLight.color;
            originalLightIntensity = globalLight.intensity;
        }
        maxHealth = 4;
        currentHealth = maxHealth;
        currentHeartTimer = timerDuration;
        SetupHearts();
        StartCoroutine(StartHeartTimer());
    }
    
    private void SetupHearts()
    {
        hearts = new HeartIndicator[maxHealth];
        
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer.transform);
            hearts[i] = heart.GetComponent<HeartIndicator>();
            hearts[i].Setup();
            hearts[i].StopBleeding(); // Ensure all hearts start without bleeding
        }
    }
    
    private IEnumerator StartHeartTimer()
    {
        while (currentHealth > 0)
        {
            // Stop bleeding on all hearts
            for (int i = 0; i < maxHealth; i++)
            {
                hearts[i].StopBleeding();
            }
            
            // Start bleeding only on the current active heart
            if (currentHealth > 0)
            {
                hearts[currentHealth - 1].StartBleeding();
            }
            
            // Timer countdown logic
            currentHeartTimer = timerDuration;
            while (currentHeartTimer > 0)
            {
                currentHeartTimer -= Time.deltaTime;
                
                // Update the current heart's fade based on timer
                if (currentHealth > 0)
                {
                    float fadeProgress = currentHeartTimer / timerDuration;
                    hearts[currentHealth - 1].UpdateFade(fadeProgress);
                }
                
                yield return null;
            }
            
            RemoveHeart();
        }
        StartCoroutine(GameOverSequence());
        yield break;
    }
    
    private void RemoveHeart()
    {
        if (currentHealth > 0)
        {
            hearts[currentHealth - 1].StopBleeding();
            currentHealth--;
        }
    }
    private IEnumerator GameOverSequence()
    {
        // Start screen shake
        StartCoroutine(ScreenShake());
        
        // Change light color and intensity
        float elapsedTime = 0f;
        while (elapsedTime < lightChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lightChangeDuration;
            
            // Lerp the light color to red
            globalLight.color = Color.Lerp(originalLightColor, Color.red, t);
            
            // Increase light intensity
            globalLight.intensity = Mathf.Lerp(originalLightIntensity, 
                                             originalLightIntensity + lightIntensityIncrease, 
                                             t);
            
            yield return null;
        }
        
        // Wait for shake to finish
        yield return new WaitForSeconds(shakeDuration);
        
        // Load bad ending scene
        SceneManager.LoadScene(badEndingSceneName);
    }
    
    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Reset camera position
        mainCamera.transform.position = originalCameraPosition;
    }
}