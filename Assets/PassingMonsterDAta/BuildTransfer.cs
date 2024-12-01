// BuildTransfer.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BuildTransfer : MonoBehaviour
{
    public static BuildTransfer Instance { get; private set; }

        // Add reference to BuildInventoryManager
    public BuildInventoryManager inventoryManager;

    // Public references to BuildSpots
    public BuildSpot leftArmSpot;
    public BuildSpot rightArmSpot;
    public BuildSpot leftLegSpot;
    public BuildSpot rightLegSpot;
    public BuildSpot headSpot;
    public BuildSpot torsoSpot;

    public MonsterData currentMonster;
    
        // Add bool for good ending
    public bool isGoodEnding { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // No need to create MonsterData here
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectPartsFromBuildSpots()
    {
        // Create MonsterData when starting gameplay
        currentMonster = ScriptableObject.CreateInstance<MonsterData>();

        // Assign parts from BuildSpots
        currentMonster.leftArm = leftArmSpot.GetCurrentItem();
        currentMonster.rightArm = rightArmSpot.GetCurrentItem();
        currentMonster.leftLeg = leftLegSpot.GetCurrentItem();
        currentMonster.rightLeg = rightLegSpot.GetCurrentItem();
        currentMonster.head = headSpot.GetCurrentItem();
        currentMonster.torso = torsoSpot.GetCurrentItem();
    }
    // Add method to check inventory count
    public void CheckInventoryCompletion()
    {
        if (inventoryManager != null)
        {
            int itemCount = 0;
            
            // If using GameManager's inventory
            if (GameManager.Instance != null)
            {
                itemCount = GameManager.Instance.playerInventory.Count;
            }
            // If using availableParts list
            else if (inventoryManager.availableParts != null)
            {
                itemCount = inventoryManager.availableParts.Count;
            }

            isGoodEnding = (itemCount == 29);
        }
    }
    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("monsterSpec");
    }

    public void StartGameplay()
    {
        // Start the coroutine for delayed scene loading
        StartCoroutine(LoadSceneWithDelay());
    }
}
