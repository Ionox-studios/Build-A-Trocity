using UnityEngine;
using System.Collections.Generic;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private List<ItemSO> itemsToLoad = new List<ItemSO>();
    
    [Header("Testing Options")]
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private KeyCode addItemsKey = KeyCode.L;
    [SerializeField] private bool clearInventoryBeforeLoading = true;

    private void Start()
    {
        if (loadOnStart)
        {
            LoadItemsIntoInventory();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(addItemsKey))
        {
            LoadItemsIntoInventory();
        }
    }

    public void LoadItemsIntoInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager instance not found!");
            return;
        }

        if (clearInventoryBeforeLoading)
        {
            InventoryManager.Instance.inventory.Clear();
        }

        foreach (ItemSO item in itemsToLoad)
        {
            if (item != null)
            {
                InventoryManager.Instance.AddItem(item);
            }
        }
    }

    // Helper method to add a single item for testing
    public void AddSingleItem(ItemSO item)
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager instance not found!");
            return;
        }

        if (item != null)
        {
            InventoryManager.Instance.AddItem(item);
        }
    }
}