using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class BuildInventoryManager : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryContent;
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private List<ItemSO> availableParts;
    
    [Header("Layout Settings")]
    [SerializeField] private GridLayoutGroup gridLayout;


    private void Start()
    {
        SetupPanelAndGrid();
        InitializeInventory();
    }

    private void SetupPanelAndGrid()
    {
        // Set up the panel to have fixed size
        if (inventoryPanel != null)
        {

            // Set fixed position and size
            //inventoryPanel.anchorMin = new Vector2(0, 0);
            //inventoryPanel.anchorMax = new Vector2(0, 0);
            //inventoryPanel.pivot = new Vector2(0, 0);
            
            // Set your desired size
  //          inventoryPanel.sizeDelta = new Vector2(400, 700); // Adjust these values
            
            // Position it where you want
            //inventoryPanel.anchoredPosition = new Vector2(50, 50); // Adjust these values
        }

        // Set up the content to fill the panel
        if (inventoryContent != null)
        {
            inventoryContent.anchorMin = Vector2.zero;
            inventoryContent.anchorMax = Vector2.one;
            inventoryContent.sizeDelta = Vector2.zero;
            inventoryContent.anchoredPosition = Vector2.zero;
            
        }

        if (gridLayout == null)
        {
            gridLayout = inventoryContent.GetComponent<GridLayoutGroup>();
        }

        if (gridLayout != null)
        {
//            gridLayout.cellSize = new Vector2(100f, 100f); //AL
   //         gridLayout.spacing = new Vector2(10f, 10f);
    //        gridLayout.padding = new RectOffset(10, 10, 10, 10);
     //       gridLayout.childAlignment = TextAnchor.UpperLeft;
      //      gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;
        }
    }

    private void InitializeInventory()
    {
        // Clear existing items
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // Use GameManager's inventory if it exists, otherwise fall back to availableParts
        List<ItemSO> itemsToDisplay = GameManager.Instance != null 
            ? GameManager.Instance.playerInventory 
            : availableParts;

        foreach (ItemSO part in itemsToDisplay)
        {
            GameObject itemObj = Instantiate(inventoryItemPrefab, inventoryContent);
            RectTransform itemRect = itemObj.GetComponent<RectTransform>();
            
            if (itemRect != null)
            {
 //               itemRect.anchorMin = new Vector2(0.5f, 0.5f);
  //              itemRect.anchorMax = new Vector2(0.5f, 0.5f);
  //              itemRect.pivot = new Vector2(0.5f, 0.5f);
                
            }

            BuildInventoryItem itemComponent = itemObj.GetComponent<BuildInventoryItem>();
            if (itemComponent != null)
            {
                itemComponent.Initialize(part);
            }
        }
    }
    
}