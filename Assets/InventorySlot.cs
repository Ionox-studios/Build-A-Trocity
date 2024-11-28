using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void SetItem(ItemSO item) // Changed from Item to ItemSO
    {
        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            iconImage.preserveAspect = true; //AL
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }
}