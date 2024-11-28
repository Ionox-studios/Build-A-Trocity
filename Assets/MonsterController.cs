// MonsterController.cs
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public ArmController leftArm;
    public ArmController rightArm;
    public LegController leftLeg;
    public LegController rightLeg;

    [Header("Movement Settings")]
    public float baseMovementSpeed = 5f;
    public float armSwingForce = 2f;
    public float legLengthInfluence = 1f;

    [Header("Arm Control Settings")]
    public float maxSwingSpeed = 30f;
    
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        HandleInput();
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        
        // Apply swing speed decay
        if(leftArm != null)
            leftArm.DecaySwingSpeed(leftArm.GetBaseSwingSpeed());
        if(rightArm != null)
            rightArm.DecaySwingSpeed(rightArm.GetBaseSwingSpeed());
    }

    void FixedUpdate()
    {
        UpdateMovement();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.Q) && leftArm != null)
        {
            if(leftArm.swingSpeed < maxSwingSpeed)
            {
                leftArm.IncreaseSwingSpeed();
            }
        }
        
        if (Input.GetKey(KeyCode.E) && rightArm != null)
        {
            if(rightArm.swingSpeed < maxSwingSpeed)
            {
                rightArm.IncreaseSwingSpeed();
            }
        }
    }

    void UpdateMovement()
    {
        Vector2 finalForce = movement * baseMovementSpeed;

        // Arm influence
        float verticalArmForce = 0f;
        if (leftArm.IsSwingingUp() && rightArm.IsSwingingUp())
        {
            verticalArmForce = armSwingForce;
        }
        else if (leftArm.IsSwingingDown() && rightArm.IsSwingingDown())
        {
            verticalArmForce = -armSwingForce;
        }

        float horizontalArmForce = 0f;
        if (leftArm.IsSwingingRight() != rightArm.IsSwingingRight())
        {
            horizontalArmForce = (leftArm.IsSwingingRight() ? -1 : 1) * armSwingForce;
        }

        finalForce += new Vector2(horizontalArmForce, verticalArmForce);

        // Leg influence
        float avgLegSpeed = (leftLeg.speed + rightLeg.speed) / 2f;
        float legDifference = (leftLeg.length - rightLeg.length) * legLengthInfluence;
        
        finalForce *= avgLegSpeed;
        finalForce.x += legDifference;

        rb.AddForce(finalForce);
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, baseMovementSpeed * avgLegSpeed);
    }
}