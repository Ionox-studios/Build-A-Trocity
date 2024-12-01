// BuildSpot.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildSpot : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ItemSO.ItemType acceptedType;
    [SerializeField] private ItemSO.BodySide spotSide;  // Left or Right
    [SerializeField] private string stationName;
    [SerializeField] private string description;
    [SerializeField] private bool isRequired = true;
    [SerializeField] private ItemSO defaultItem;
    [SerializeField] private Image placeholderImage;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip placementSound;
    [SerializeField] private AudioClip rejectSound;  // Add this line


    private BuildInventoryItem currentItem;
    private Vector3 originalScale;

    private void Start()
    {
        BuildManager.Instance.RegisterBuildSpot(this);

        if (placeholderImage != null)
        {
            originalScale = placeholderImage.transform.localScale;
        }
    }

    public bool CanAcceptItem(ItemSO item)
    {
        bool canAccept = item.itemType == acceptedType;
        if (!canAccept)
        {
            if (audioSource != null && rejectSound != null)
            {
                audioSource.PlayOneShot(rejectSound);
            }
            Debug.Log($"Cannot accept item of type {item.itemType}");
        }
        
        return canAccept;
    }

    public void PlaceItem(BuildInventoryItem item)
    {
        Debug.Log($"Attempting to place item of type {item.Item.itemType}");

        if (!CanAcceptItem(item.Item))
        {
            // Play reject sound when item type isn't accepted

            Debug.Log("Item type not accepted");
            Debug.Log("Item type not accepted");
            return;
        }

        if (currentItem != null)
        {
            RemoveItem();
        }
        if (audioSource != null && placementSound != null)
            {
            audioSource.PlayOneShot(placementSound);
            }

        currentItem = item;

        if (placeholderImage != null && item.Item.icon != null)
        {
            Debug.Log("Setting placeholder image");
            placeholderImage.sprite = item.Item.icon;
            placeholderImage.preserveAspect = true;

            // Flip the image if necessary
            if ((spotSide == ItemSO.BodySide.Left && item.Item.side == ItemSO.BodySide.Right) ||
                (spotSide == ItemSO.BodySide.Right && item.Item.side == ItemSO.BodySide.Left))
            {
                placeholderImage.transform.localScale = new Vector3(
                    -Mathf.Abs(originalScale.x),
                    originalScale.y,
                    originalScale.z
                );
            }
            else
            {
                placeholderImage.transform.localScale = originalScale;
            }
        }
        else
        {
            Debug.Log($"PlaceholderImage null? {placeholderImage == null}, Item icon null? {item.Item.icon == null}");
        }

        // Hide the dragged item's visuals
        var itemImage = item.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.enabled = false;
        }

        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        BuildManager.Instance.PlacePart(item.Item, this);
    }

    public void RemoveItem()
    {
        if (currentItem != null)
        {
            // Restore the dragged item's visuals
            var itemImage = currentItem.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.enabled = true;
            }

            // Reset placeholder
            if (placeholderImage != null)
            {
                placeholderImage.sprite = null;
                placeholderImage.transform.localScale = originalScale;  // Reset scale when removing
            }

            // Return item to inventory
            currentItem.transform.SetParent(currentItem.originalParent);
            currentItem.ReturnToOriginalPosition();

            BuildManager.Instance.RemovePart(acceptedType);
            currentItem = null;
        }
    }

    public ItemSO GetCurrentItem()
    {
        return currentItem != null ? currentItem.Item : null;
    }

    public bool HasItem() => currentItem != null;
    public bool IsRequired() => isRequired;
    public ItemSO.ItemType GetAcceptedType() => acceptedType;
    public ItemSO GetDefaultItem() => defaultItem;
    public ItemSO.BodySide GetSpotSide() => spotSide;
}
