using UnityEngine;

public class LegController : MonoBehaviour 
{
    private ItemSO _legItem;
    public float length { get; private set; } = 1f;
    public float speed { get; private set; } = 1f;
    
    private Transform parentTransform;

    private void Start()
    {
        parentTransform = transform.parent;
    }

    private void LateUpdate()
    {
        if(parentTransform != null)
        {
            // Keep the leg's position relative to parent but let it keep its rotation
            Vector3 currentPos = transform.position;
            currentPos.x = parentTransform.position.x;
            currentPos.y = parentTransform.position.y - length; // Offset by leg length
            transform.position = currentPos;
        }
    }

    public void SetItem(ItemSO newItem)
    {
        if (newItem == null || newItem.itemType != ItemSO.ItemType.Leg)
        {
            Debug.LogError($"Invalid item assigned to LegController on {gameObject.name}");
            return;
        }

        _legItem = newItem;
        UpdateItemProperties();
    }

    private void UpdateItemProperties()
    {
        if (_legItem == null) return;

        length = Mathf.Max(_legItem.length, 0.1f);
        speed = Mathf.Max(_legItem.speed, 0.1f);
        
        // Update physical properties
        Vector3 currentScale = transform.localScale;
        currentScale.y = length;
        transform.localScale = currentScale;
    }
}