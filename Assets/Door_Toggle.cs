using UnityEngine;

public class Door_Toggle : MonoBehaviour
{
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite; 
    private SpriteRenderer spriteRenderer;
       
    private bool isOpen = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        if (!isOpen)
        {
            isOpen = true;
            spriteRenderer.sprite = openSprite;
        }
    }
}