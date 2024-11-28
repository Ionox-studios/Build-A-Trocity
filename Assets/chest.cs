using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemToGive;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;  // Added closed sprite
    [SerializeField] private string chestID; // Unique identifier for this chest
    public AudioSource audioSource; //AL
    public AudioClip openSound; //AL
    private SpriteRenderer spriteRenderer;
    private bool isOpen = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set initial sprite state
        if (GameManager.Instance.openedChests.Contains(chestID))
        {
            isOpen = true;
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;  // Set closed sprite by default
        }
    }

    public void Interact()
    {
        if (!isOpen)
        {
            isOpen = true;
            spriteRenderer.sprite = openSprite;
            GameManager.Instance.openedChests.Add(chestID);
            InventoryManager.Instance.AddItem(itemToGive);
            audioSource.PlayOneShot(openSound); //AL
        }
    }
}