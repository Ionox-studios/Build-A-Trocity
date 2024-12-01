using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.Profiling;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("UI References")]
    public Image[] hearts;
    public Image redOverlay;  // Reference to red screen flash image
    public GameObject hitEffectSprite;  // Reference to hit effect sprite overlay
    public Vector2 powOffset = new Vector2(0.5f, 0.5f); // Offset from player position
    

    
[Header("Hit Effect Settings")]
public float invulnerabilityTime = 2f;
public float knockbackForce = 20f; // Increased from 10
public float bounceForce = 5f;
public float pulseMinScale = 0.8f;  // New minimum scale



public float pulseMaxScale = 1.5f;
public float pulseDuration = 0.2f;
public movementIgor movementScript; // Reference to your movement script
public float screenFlashDuration = 0.2f;
public string gameOverSceneName = "GameOver";
[Header("Audio")]
public AudioSource audioSource;  // Reference to the AudioSource component
public AudioClip hitSound;       // The sound to play when hit
    private bool isInvulnerable = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hitEffectSprite.SetActive(false);
        
        if (GameManager.Instance.playerHealth != 0)
        {
            currentHealth = GameManager.Instance.playerHealth;
        }
        else
        {
            currentHealth = maxHealth;
            GameManager.Instance.playerHealth = currentHealth;
        }
        UpdateHealthUI();
    }

    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        if (isInvulnerable) return;
            // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        currentHealth -= damage;
        GameManager.Instance.playerHealth = currentHealth;
        UpdateHealthUI();
        
        // Apply all hit effects
        StartCoroutine(HitEffects(attackerPosition));
        
        if (currentHealth <= 0)
        {
           GameOver();
        }
    }
private IEnumerator HitEffects(Vector2 attackerPosition)
{
    isInvulnerable = true;
    
    // Disable player movement temporarily
    if (movementScript != null)
        movementScript.enabled = false;
    
    // Calculate knockback direction
    Vector2 knockbackDir = ((Vector2)transform.position - attackerPosition).normalized;
    
        // Screen flash effect
    redOverlay.color = new Color(1, 0, 0, 0.3f);  // Set initial flash opacity
    
    // Show hit effect sprite
    hitEffectSprite.SetActive(true);
    hitEffectSprite.transform.localScale = Vector3.zero; // Start at zero scale
        // Apply knockback
    rb.linearVelocity = Vector2.zero;  // Reset current velocity
    rb.AddForce(knockbackDir * knockbackForce + Vector2.up * bounceForce, ForceMode2D.Impulse);
    // Pulse animation
    float elapsedTime = 0;
        while (elapsedTime < pulseDuration)
        {
            float scaleProgress = elapsedTime / pulseDuration;
            // Lerp between min and max scale using sine wave
            float currentScale = Mathf.Lerp(pulseMinScale, pulseMaxScale, 
                Mathf.Sin(scaleProgress * Mathf.PI));
            hitEffectSprite.transform.localScale = new Vector3(currentScale, currentScale, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


    
    // Wait a short time before re-enabling movement
    //yield return new WaitForSeconds(0.2f);
    
    // Re-enable player movement
    if (movementScript != null)
        movementScript.enabled = true;
    hitEffectSprite.SetActive(false);
            // Wait for flash duration
        yield return new WaitForSeconds(screenFlashDuration);
        
        // Fade out red overlay
        elapsedTime = 0;
        while (elapsedTime < screenFlashDuration)
        {
            float alpha = Mathf.Lerp(0.3f, 0, elapsedTime / screenFlashDuration);
            redOverlay.color = new Color(1, 0, 0, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        redOverlay.color = new Color(1, 0, 0, 0);  // Ensure it's fully transparent
    
    yield return new WaitForSeconds(invulnerabilityTime - 0.2f);
    isInvulnerable = false;
}

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        GameManager.Instance.playerHealth = currentHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHealth;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        GameManager.Instance.playerHealth = maxHealth;
        SceneManager.LoadScene(gameOverSceneName);
    }
}