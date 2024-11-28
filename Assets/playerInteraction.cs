using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private PlayerInput playerInput;
    
    private void Awake()
    {
        // Get reference to PlayerInput if not assigned in inspector
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
            
        // Subscribe to the interact action
        playerInput.actions["Interact"].performed += OnInteract;
    }
    
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        
        foreach (Collider2D collider in colliders)
        {
            Debug.Log("Checking collider: " + collider.name);
            Chest chest = collider.GetComponent<Chest>();
            if (chest != null)
            {
                chest.Interact();
                // break;
            }
        }
    }
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (playerInput != null)
            playerInput.actions["Interact"].performed -= OnInteract;
    }
}