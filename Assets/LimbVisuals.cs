using UnityEngine;


public class LimbVisuals : MonoBehaviour
{
    public ItemSO itemSO;
    public SpriteRenderer spriteRenderer;
    public ItemSO.BodySide side; // Add this to set which side this limb represents

    void Start()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (spriteRenderer != null && itemSO != null)
        {
            spriteRenderer.sprite = itemSO.icon;
                        
            // Flip the sprite if the itemSO side doesn't match the limb's side
            if (itemSO.side != side)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void SetItemSO(ItemSO newItemSO)
    {
        itemSO = newItemSO;
        UpdateSprite();
    }

    // Optional: Method to set the side if you need to change it at runtime
    public void SetSide(ItemSO.BodySide newSide)
    {
        side = newSide;
        UpdateSprite();
    }
}