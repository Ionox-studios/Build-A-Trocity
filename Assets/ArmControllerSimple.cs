using UnityEngine;

public class ArmControllerSimple : MonoBehaviour
{
    [Header("Pivot Setup")]
    public Transform armPivot;
    public bool isRightArm = false;

    [Header("Swing Settings")]
    public float swingSpeed = 5f;
    public float swingSpeedMultiplier = 2f; // New public variable
    public float maxSwingAngle = 135f;
    public float minSwingAngle = -45f;
    public float maxSwingSpeed = 30f;
    public float neutralAngle = -45f;

    public float arbitrarySpeedBoost= 100f;
    [Header("Debug")]
    public bool showDebugGizmos = true;
    public Color gizmoColor = Color.yellow;
    

    private float currentAngle;
    private int swingDirection = 0;
    private bool isSwinging = false;
    private bool isKeyHeld = false; // Tracks if the swing key is held
        public LimbVisuals limbVisuals;  // Add this reference
            public Transform footPoint; // Reference to the foot/end of the leg

    public bool isShooter = false;
    private Transform shoulderReference;
    private Quaternion initialLocalRotation;

    void Start()
    {
        if (armPivot == null)
        {
            Debug.LogWarning($"No arm pivot assigned to {gameObject.name}! Please assign a pivot point.");
        }
        initialLocalRotation = transform.localRotation;
        currentAngle = neutralAngle;
        UpdateArmRotation();
        Debug.Log($"Arm {gameObject.name} initial position: {transform.position}");

    }

    public void ConnectToShoulder(Transform shoulderPoint)
    {
    if (shoulderPoint != null)
    {
        shoulderReference = shoulderPoint;
        UpdateArmPosition();
        UpdateArmRotation();
    }
    }

    void Update()
    {
        // Adjust swing speed based on key hold
        float actualSwingSpeed = swingSpeed;
        if (isKeyHeld)
        {
            actualSwingSpeed *= swingSpeedMultiplier;
        }

        if (isSwinging)
        {
            float previousAngle = currentAngle;
            currentAngle += actualSwingSpeed * swingDirection * Time.deltaTime;
            
            if (currentAngle >= maxSwingAngle)
            {
                currentAngle = maxSwingAngle;
                swingDirection = -1;
            }
            else if (currentAngle <= neutralAngle)
            {
                currentAngle = neutralAngle;
                swingDirection = 0;
                isSwinging = false;
            }
            
            if (previousAngle != currentAngle)
            {
                UpdateArmRotation();
            }
        }
        else if (Mathf.Abs(currentAngle - neutralAngle) > 0.01f)
        {
            currentAngle = neutralAngle;
            UpdateArmRotation();
        }

        if (shoulderReference != null)
        {
            UpdateArmPosition();
        }
    }

    void UpdateArmPosition()
    {
        if (armPivot != null)
        {
            Vector3 offset = armPivot.position - transform.position;
            transform.position = shoulderReference.position - offset;
        }
    }

    void UpdateArmRotation()
    {
        if (armPivot != null)
        {
            transform.localRotation = initialLocalRotation;
            float finalAngle = currentAngle * (isRightArm ? 1 : -1); // Flip angle for left arm
            transform.RotateAround(armPivot.position, Vector3.forward, finalAngle);
        }
    }

    public void TriggerSwing()
    {
        if (!isSwinging)
        {
            isSwinging = true;
            swingDirection = 1;
            currentAngle = neutralAngle;
            UpdateArmRotation();
        }
    }

    public void SetKeyHeld(bool keyHeld)
    {
        isKeyHeld = keyHeld;
    }

    public bool IsKeyHeld()
    {
        return isKeyHeld;
    }

    public void IncreaseSwingSpeed()
    {
        swingSpeed = Mathf.Min(swingSpeed + 10f * Time.deltaTime, maxSwingSpeed);
    }

    public void ResetSwing()
    {
        isSwinging = false;
        swingDirection = 0;
        currentAngle = neutralAngle;
        UpdateArmRotation();
    }

    public bool IsSwingingUp()
    {
        return swingDirection > 0;
    }

    public bool IsSwingingDown()
    {
        return swingDirection < 0;
    }

    public bool IsSwingingForward()
    {
        // Simplify the forward check based on direction
        return swingDirection > 0;
    }

    public float GetCurrentAngle()
    {
        return currentAngle;
    }

    public bool IsSwinging()
    {
        return isSwinging;
    }
    public void Configure(ItemSO armConfig)
{
    // Apply arm-specific configurations
    if (armConfig.isTentacle)
    {
        // Configure tentacle behavior
    }
    if (armConfig.isStretchy)
    {
        // Configure stretchy behavior
    }
    // Set other properties


    swingSpeed = armConfig.swingSpeed*arbitrarySpeedBoost;
    maxSwingSpeed = armConfig.maxSwingSpeed*arbitrarySpeedBoost;


        if (limbVisuals != null)
    {
        limbVisuals.itemSO = armConfig;
        limbVisuals.UpdateSprite();
    }
            if (armConfig.isGun)
        {
            LimbShooter shooter = gameObject.AddComponent<LimbShooter>();
            shooter.Configure(armConfig, footPoint);
            isShooter = true;
        }
}


    private void OnDrawGizmos()
    {
        if (showDebugGizmos && armPivot != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(armPivot.position, 0.2f); //was 0.1f
            Gizmos.DrawLine(armPivot.position, armPivot.position + Vector3.up * 0.2f);
        }
    }
}
