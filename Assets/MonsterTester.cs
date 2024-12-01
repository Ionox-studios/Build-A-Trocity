using UnityEngine;

public class MonsterTester : MonoBehaviour
{
    public MonsterControllerSimple monsterController;
    public ItemSO testLeftArm;
    public ItemSO testRightArm;
    public ItemSO testLeftLeg;
    public ItemSO testRightLeg;
    public ItemSO testHead;
    public ItemSO testTorso;

    void Start()
    {
                // Check if BuildTransfer exists
        if (BuildTransfer.Instance != null)
        {
            // Disable this component if BuildTransfer is present
            this.enabled = false;
            return;
        }
        // Create test monster data
        MonsterData testMonster = ScriptableObject.CreateInstance<MonsterData>();
        testMonster.leftArm = testLeftArm;
        testMonster.rightArm = testRightArm;
        testMonster.leftLeg = testLeftLeg;
        testMonster.rightLeg = testRightLeg;
        testMonster.head = testHead;
        testMonster.torso = testTorso;

        // Apply to monster controller
        if (monsterController != null)
        {
            monsterController.SetupMonsterData(testMonster);
        }
    }
}