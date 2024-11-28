using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Changed to Collider2D
public class DoorRattleEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rattleSound;

    private void Start()
    {
        // Changed to Collider2D
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        
        Debug.Log("DoorRattleEffect initialized on " + gameObject.name);
        if (audioSource == null)
            Debug.LogWarning("No AudioSource assigned to DoorRattleEffect on " + gameObject.name);
        if (rattleSound == null)
            Debug.LogWarning("No rattle sound assigned to DoorRattleEffect on " + gameObject.name);
    }

    // Changed to OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            if (audioSource != null && rattleSound != null)
            {
                audioSource.PlayOneShot(rattleSound);
                Debug.Log("Playing rattle sound");
            }
        }
    }
}