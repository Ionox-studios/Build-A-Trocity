using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public List<ItemSO> inventory = new List<ItemSO>();
    public int maxInventorySize = 10;
    
    [SerializeField] private Transform inventoryUIParent;
    [SerializeField] private GameObject inventorySlotPrefab;
    
    public bool isInventoryOpen = false;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private BodyPartsUI bodyPartsUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load inventory from GameManager
        inventory = new List<ItemSO>(GameManager.Instance.playerInventory);
        UpdateInventoryUI();
        
        // Initialize inventory panel state
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    public void AddItem(ItemSO item)
    {
        bodyPartsUI.RemoveBodyPartSquare(item.itemType);
        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(item);
            GameManager.Instance.playerInventory = new List<ItemSO>(inventory);
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public void RemoveItem(ItemSO item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            GameManager.Instance.playerInventory = new List<ItemSO>(inventory);
            UpdateInventoryUI();
        }
    }

    public bool HasItem(ItemSO item)
    {
        return inventory.Contains(item);
    }

    private void UpdateInventoryUI()
    {

        // Clear existing UI
        foreach (Transform child in inventoryUIParent)
        {
            Destroy(child.gameObject);
        }

        // Create new slots
        for (int i = 0; i < maxInventorySize; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
            if (i < inventory.Count)
            {
                slot.GetComponent<InventorySlot>().SetItem(inventory[i]);
            }
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    private void Update()
    {
        // Optional: Add inventory toggle key
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
}