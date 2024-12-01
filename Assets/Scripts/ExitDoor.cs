using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;

public class ExitDoor : MonoBehaviour
{
    public string sceneToLoad;
    public string secretEnding; // New scene for secret ending

    public Vector2 playerPosition;
    public VectorValue playerStorage;
    
    // Dialogue components
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    private bool playerInRange;
    private bool showingExitDialog;
    
    // Reference to InventoryManager
    public InventoryManager inventoryManager;
    
    private void Start()
    {
        dialogBox.SetActive(false);
        showingExitDialog = false;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && showingExitDialog)
        {
            ExitLevel();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerInRange = true;
            ShowExitDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            playerInRange = false;
            dialogBox.SetActive(false);
            showingExitDialog = false;
        }
    }

    private void ShowExitDialog()
    {
        string message;
        int bodyPartCount = inventoryManager.inventory.Count;
        var (hasCompleteBody, missingParts) = CheckForCompleteBody();
        int totalParts = 29; // Total number of collectible parts

        int remainingParts = totalParts - bodyPartCount;

        if (bodyPartCount == 0)
        {
            message = "You haven't collected anything yet? Are you sure you want to leave? (Press E to leave)";
        }
        else if (!hasCompleteBody)
        {
            message = $"You have an incomplete body. Victor will be angry! You're missing: {missingParts}. Are you sure you want to leave? (Press E to leave)";
        }
        else if (bodyPartCount == 29)
        {
            message = "Congrats you collected everything! Are you ready to build? (Press E to leave)";
        }
        else
        {
            message = $"You have what's needed to Build-A-Trocity but {remainingParts} parts remain. Are you sure you want to leave? (Press E to leave)";
        }

        dialogText.text = message;
        dialogBox.SetActive(true);
        showingExitDialog = true;
    }

    private (bool complete, string missing) CheckForCompleteBody()
    {
        int arms = 0;
        int legs = 0;
        int torso = 0;
        int head = 0;

        foreach (ItemSO item in inventoryManager.inventory)
        {
            switch (item.itemType)
            {
                case ItemSO.ItemType.Arm:
                    arms++;
                    break;
                case ItemSO.ItemType.Leg:
                    legs++;
                    break;
                case ItemSO.ItemType.Torso:
                    torso++;
                    break;
                case ItemSO.ItemType.Head:
                    head++;
                    break;
            }
        }

        StringBuilder missingParts = new StringBuilder();
        if (arms < 2) missingParts.Append($"{2 - arms} arm{(2 - arms > 1 ? "s" : "")}, ");
        if (legs < 2) missingParts.Append($"{2 - legs} leg{(2 - legs > 1 ? "s" : "")}, ");
        if (torso < 1) missingParts.Append("a torso, ");
        if (head < 1) missingParts.Append("a head, ");

        string missingString = missingParts.ToString().TrimEnd(' ', ',');
        
        bool isComplete = arms >= 2 && legs >= 2 && torso >= 1 && head >= 1;
        return (isComplete, missingString);
    }

    private void ExitLevel()
    {
        if (inventoryManager.inventory.Count == 0)
        {
            SceneManager.LoadScene(secretEnding);
        }
        else
        {
            //playerStorage.initialValue = playerPosition;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}