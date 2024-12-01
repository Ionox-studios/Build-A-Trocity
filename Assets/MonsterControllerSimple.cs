using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MonsterControllerSimple : MonoBehaviour
{
    public ArmControllerSimple leftArm;
    public ArmControllerSimple rightArm;
    public Transform leftShoulderPoint;
    public Transform rightShoulderPoint;

    public Transform headPoint;
    public Transform torsoPoint;
    public SpriteRenderer headRenderer;
    public SpriteRenderer torsoRenderer;

    // Add references to legs
    public LegControllerSimple leftLeg;
    public LegControllerSimple rightLeg;
    public Transform leftHipPoint;
    public Transform rightHipPoint;

    public float moveSpeed = 1f;
    public float armInfluence = 0.5f;
    public float influenceMultiplier = 4f; // Existing variable

    private Rigidbody2D rb;
    // Add these near your other public variables
    [SerializeField] private TMP_Text shootPromptText;
    private float promptFadeTime = 5f;
    private float promptTimer = 0f;
    private bool promptShown = false;
    [Header("Game State")]
    public bool isBadEnding = false;
    public bool reduceEnemyFear = false;
    public bool isGreatEnding = false;

    public HeartTimer heartTimer;
    [Header("Debug Info")]
    public bool showDebugInfo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (leftArm != null && leftShoulderPoint != null)
        {
            leftArm.ConnectToShoulder(leftShoulderPoint);
        }

        if (rightArm != null && rightShoulderPoint != null)
        {
            rightArm.ConnectToShoulder(rightShoulderPoint);
        }

        if (leftLeg != null && rightLeg != null)
        {
            rightLeg.SetPhaseOffset(Mathf.PI); // Offset by 180 degrees
        }
        if (BuildTransfer.Instance != null && BuildTransfer.Instance.currentMonster != null)
        {
            ApplyMonsterConfiguration(BuildTransfer.Instance.currentMonster);
            isGreatEnding  = BuildTransfer.Instance.isGoodEnding;
        }

    }

    void Update()
    {
        // Existing Update code...
        // Handle arm swings and key holds
        if (Input.GetKeyDown(KeyCode.O))
        {
            leftArm.TriggerSwing();
        }
        leftArm.SetKeyHeld(Input.GetKey(KeyCode.O)); // Track if Q is held

        if (Input.GetKeyDown(KeyCode.P))
        {
            rightArm.TriggerSwing();
        }
        rightArm.SetKeyHeld(Input.GetKey(KeyCode.P)); // Track if E is held

        if (showDebugInfo)
        {
            DebugArmStatus();
        }

        if (promptShown && Time.time > promptTimer + promptFadeTime)
        {
            Debug.Log("Fading out prompt");
            float alpha = 1f - ((Time.time - (promptTimer + promptFadeTime)) / 0.5f);
            if (alpha <= 0)
            {
                shootPromptText.gameObject.SetActive(false);
            }
            else
            {
                shootPromptText.color = new Color(1, 1, 1, alpha);
            }
        }
    }

    void FixedUpdate()
    {
        // Get input
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Calculate base movement direction
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

        // Get leg speeds
        float leftLegSpeed = leftLeg != null ? leftLeg.GetSpeed() : 1f;
        float rightLegSpeed = rightLeg != null ? rightLeg.GetSpeed() : 1f;
        float rightLegLength = rightLeg != null ? rightLeg.GetLength() : 1f;
        float leftLegLength = leftLeg != null ? leftLeg.GetLength() : 1f;
        Debug.Log($"Left Leg Speed: {leftLegSpeed} | Right Leg Speed: {rightLegSpeed}");
        Debug.Log($"Left Leg Length: {leftLegLength} | Right Leg Length: {rightLegLength}");
        // Calculate x speed based on leg speeds and input direction
        float xSpeed = moveSpeed;
        if (horizontalInput > 0)
        {
            xSpeed *= rightLegSpeed;
        }
        else if (horizontalInput < 0)
        {
            xSpeed *= leftLegSpeed;
        }
        else
        {
            xSpeed *= (leftLegSpeed + rightLegSpeed) / 2f;
        }

        // Calculate y speed based on average leg speed
        float ySpeed = moveSpeed * (leftLegSpeed + rightLegSpeed) / 2f;

        // Apply speeds to movement
        movement.x *= xSpeed;
        movement.y *= ySpeed;

        // Vertical arm influence
        float verticalInfluence = 0f;

        // Existing code to calculate verticalInfluence
        if (leftArm.IsSwingingUp())
        {
            float leftInfluence = armInfluence*leftLegLength ;
            if (leftArm.IsKeyHeld())
            {
                leftInfluence *= influenceMultiplier;
            }
            verticalInfluence += leftInfluence;
        }
        if (leftArm.IsSwingingDown())
        {
            float leftInfluence = armInfluence*leftLegLength;
            if (leftArm.IsKeyHeld())
            {
                leftInfluence *= influenceMultiplier;
            }
            verticalInfluence -= leftInfluence;
        }

        if (rightArm.IsSwingingUp())
        {
            float rightInfluence = armInfluence*rightLegLength;
            if (rightArm.IsKeyHeld())
            {
                rightInfluence *= influenceMultiplier;
            }
            verticalInfluence += rightInfluence;
        }
        if (rightArm.IsSwingingDown())
        {
            float rightInfluence = armInfluence*rightLegLength;
            if (rightArm.IsKeyHeld())
            {
                rightInfluence *= influenceMultiplier;
            }
            verticalInfluence -= rightInfluence;
        }

        // Horizontal arm influence
        float horizontalInfluence = 0f;

        if (rightArm.IsSwingingForward())
        {
            float rightInfluence = armInfluence*rightLegLength;
            if (rightArm.IsKeyHeld())
            {
                rightInfluence *= influenceMultiplier;
            }
            horizontalInfluence += rightInfluence;
        }

        if (leftArm.IsSwingingForward())
        {
            float leftInfluence = armInfluence*leftLegLength;
            if (leftArm.IsKeyHeld())
            {
                leftInfluence *= influenceMultiplier;
            }
            horizontalInfluence -= leftInfluence;
        }
        Debug.Log($"Vertical Influence: {verticalInfluence} | Horizontal Influence: {horizontalInfluence}");

        // Adjust arm influence based on average leg length
        float avgLegLength = ((leftLeg != null ? leftLeg.GetLength() : 1f) + (rightLeg != null ? rightLeg.GetLength() : 1f)) / 2f;

        //verticalInfluence *= avgLegLength;
        //horizontalInfluence *= avgLegLength;

        // Add arm influences to movement
        movement.x += horizontalInfluence;
        movement.y += verticalInfluence;

        // Apply movement to the Rigidbody2D
        rb.linearVelocity = movement;
    }

    void DebugArmStatus()
    {
        string leftStatus = leftArm.IsSwingingForward() ? "Forward" : "Back";
        string rightStatus = rightArm.IsSwingingForward() ? "Forward" : "Back";
        Debug.Log($"Left Arm: {leftStatus} | Right Arm: {rightStatus}");
    }
private void ApplyMonsterConfiguration(MonsterData monster)
{
    bool hasMissingParts = false;
    
    // Check for missing parts and apply defaults if needed
    if (monster.leftArm == null && DefaultMonsterParts.Instance != null)
    {
        monster.leftArm = DefaultMonsterParts.Instance.defaultLeftArm;
        hasMissingParts = true;

    }
    
    if (monster.rightArm == null && DefaultMonsterParts.Instance != null)
    {
        monster.rightArm = DefaultMonsterParts.Instance.defaultRightArm;
        hasMissingParts = true;
    }
    
    if (monster.leftLeg == null && DefaultMonsterParts.Instance != null)
    {
        monster.leftLeg = DefaultMonsterParts.Instance.defaultLeftLeg;
        hasMissingParts = true;
    }
    
    if (monster.rightLeg == null && DefaultMonsterParts.Instance != null)
    {
        monster.rightLeg = DefaultMonsterParts.Instance.defaultRightLeg;
        hasMissingParts = true;
    }
    
    if (monster.head == null && DefaultMonsterParts.Instance != null)
    {
        monster.head = DefaultMonsterParts.Instance.defaultHead;
        hasMissingParts = true;
    }
    
    if (monster.torso == null && DefaultMonsterParts.Instance != null)
    {
        monster.torso = DefaultMonsterParts.Instance.defaultTorso;
        hasMissingParts = true;
    }
    
    // Set the bad ending flag if any parts were missing
    isBadEnding = hasMissingParts;
    
    // Apply configurations as before
    if (leftArm != null && monster.leftArm != null)
    {
        leftArm.Configure(monster.leftArm);
        // Tag based on damage value
        if (monster.leftArm.damage == 1)
        {
            leftArm.gameObject.tag = "Monster";
            // Tag all children
            foreach (Transform child in leftArm.transform)
            {
                child.gameObject.tag = "Monster";
            }
        }
        else if (monster.leftArm.damage > 1)
        {
            leftArm.gameObject.tag = "MonsterInstaKill";
            // Tag all children
            foreach (Transform child in leftArm.transform)
            {
                child.gameObject.tag = "MonsterInstaKill";
            }
        }
    }
    
    if (rightArm != null && monster.rightArm != null)
    {
        rightArm.Configure(monster.rightArm);
                // Tag based on damage value
        if (monster.rightArm.damage == 1)
        {
            rightArm.gameObject.tag = "Monster";
            // Tag all children
            foreach (Transform child in rightArm.transform)
            {
                child.gameObject.tag = "Monster";
            }
        }
        else if (monster.rightArm.damage == 2)
        {
            rightArm.gameObject.tag = "MonsterInstaKill";
            // Tag all children
            foreach (Transform child in rightArm.transform)
            {
                child.gameObject.tag = "MonsterInstaKill";
            }
        }
    }
    
        if (leftLeg != null && monster.leftLeg != null)
        {
            leftLeg.Configure(monster.leftLeg);
            // Add damage tag handling for legs
            if (monster.leftLeg.damage > 1)
            {
                leftLeg.gameObject.tag = "MonsterInstaKill";
                foreach (Transform child in leftLeg.transform)
                {
                    child.gameObject.tag = "MonsterInstaKill";
                }
            }
        }

        if (rightLeg != null && monster.rightLeg != null)
        {
            rightLeg.Configure(monster.rightLeg);
            // Add damage tag handling for legs
            if (monster.rightLeg.damage > 1)
            {
                rightLeg.gameObject.tag = "MonsterInstaKill";
                foreach (Transform child in rightLeg.transform)
                {
                    child.gameObject.tag = "MonsterInstaKill";
                }
            }
        }
    
    if (headRenderer != null && monster.head != null)
    {
        headRenderer.sprite = monster.head.icon;
        HeadController headController = headPoint.GetComponent<HeadController>();
        headController.headItem = monster.head;
        headController.Setup(); // Call Setup instead of letting Start handle it

    }
    
    if (torsoRenderer != null && monster.torso != null)
    {
        torsoRenderer.sprite = monster.torso.icon;
        torsoRenderer.sprite = monster.torso.icon;
        
        // Apply torso speed multiplier
        moveSpeed *= monster.torso.torsoSpeed;
        
        // Set health in HeartTimer
 
        if (heartTimer != null)
        {
            heartTimer.SetMaxHealth(monster.torso.torsoHealth);
        }
        
    }
            if (shootPromptText != null)
        {
            Debug.Log("Checking for shooters");
            bool hasShooter = (leftArm.isShooter ==  true) || 
                             (rightArm.isShooter == true) ||
                             (leftLeg.isShooter == true) ||
                             (rightLeg.isShooter== true);
            
            if (hasShooter && !promptShown)
            {
                Debug.Log("Showing shoot prompt");
                shootPromptText.gameObject.SetActive(true);
                shootPromptText.text = "Press space to shoot, you can fire every half a second!";
                shootPromptText.color = new Color(1, 1, 1, 1);
                promptShown = true;
                promptTimer = Time.time;
            }
            else
            {
                shootPromptText.gameObject.SetActive(false);
            }
        }

}
        public void SetupMonsterData(MonsterData monster)
    {
        ApplyMonsterConfiguration(monster);
    }
    
}
