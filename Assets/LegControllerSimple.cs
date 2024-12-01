using UnityEngine;
using System.Collections;

public class LegControllerSimple : MonoBehaviour
{
    [Header("Leg Properties")]
    public float speed = 1f;
    public float length = 1f;

    [Header("Running Motion")]
    public float runningFrequency = 5f; // How fast the leg cycles

    [Header("Leg Swing")]
    public float maxSwingAngle = 30f; // Max swing angle in degrees

    private float runningPhase = 0f;
    private Rigidbody2D parentRB;

    [Header("Hip Joint")]
    public Transform hipTransform; // The static hip point
    [Header("Item Configuration")]
    public ItemSO itemSO;
    public LimbVisuals visuals;

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public Color gizmoColor = Color.green;

    [Header("Audio")]
public AudioSource audioSource;
public AudioClip[] footstepSounds;  // Array of different footstep sounds
public float minTimeBetweenSteps = 0.3f;  // Minimum time between footstep sounds
public float footstepVolumeMin = 0.5f;
public float footstepVolumeMax = 1.0f;
public bool isShooter = false;
private float lastStepTime;  // Track when we last played a step sound
private bool wasGrounded;    // Track if we were grounded last frame
    public Transform endPoint; // Reference to the end/tip of the arm


    void Start()
    {
        parentRB = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        if (parentRB != null)
        {
            float movementSpeed = parentRB.linearVelocity.magnitude;
            if (movementSpeed > 0.1f)
            {
                // Increase phase based on movement speed
                runningPhase += Time.deltaTime * runningFrequency * (movementSpeed / 5f);

                // Calculate the swing angle based on the running phase
                float swingAngle = Mathf.Sin(runningPhase) * maxSwingAngle;

                if (hipTransform != null)
                {
                    // Convert angle to radians
                    float radAngle = swingAngle * Mathf.Deg2Rad;
                    //length = 1f;
                    // Calculate offset from hip based on swing angle
                    Vector3 offset = new Vector3(Mathf.Sin(radAngle), -Mathf.Cos(radAngle), 0) * 1f;

                    // Set the leg's position
                    transform.position = hipTransform.position + offset;

                    // Rotate the leg to match the swing angle
                    transform.rotation = Quaternion.Euler(0, 0, swingAngle);
                    if (Mathf.Sin(runningPhase) < 0 && Mathf.Sin(runningPhase + Time.deltaTime * runningFrequency) >= 0)
                {
                PlayFootstepSound();
                }
                }
            }
            else
            {
                // Return to default position when not moving
                if (hipTransform != null)
                {
                    // Leg pointing straight down
                    float swingAngle = 0f;

                    // Convert angle to radians
                    float radAngle = swingAngle * Mathf.Deg2Rad;

                    // Calculate offset from hip based on base angle
                    Vector3 offset = new Vector3(Mathf.Sin(radAngle), -Mathf.Cos(radAngle), 0) * 1f;

                    // Set the leg's position
                    transform.position = hipTransform.position + offset;

                    // Set the leg's rotation to default
                    transform.rotation = Quaternion.Euler(0, 0, swingAngle);
                }
            }
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetLength()
    {
        return length;
    }

    // Optional: Add phase offset for left/right legs
    public void SetPhaseOffset(float offset)
    {
        runningPhase += offset;
    }
        // Add to LegControllerSimple.cs
    public void Configure(ItemSO legConfig)
    {
        itemSO = legConfig;
        
        // Update visuals
        if (visuals != null)
        {
            visuals.SetItemSO(legConfig);
        }

        // Configure leg properties from ItemSO
        if (legConfig != null)
        {
            length = legConfig.length;
            speed = legConfig.speed;
            // Set any other leg-specific properties
            //length = 1f;
        }
        if (legConfig.isGun)
        {
            LimbShooter shooter = gameObject.AddComponent<LimbShooter>();
            shooter.Configure(legConfig, endPoint);
            isShooter = true;
        }
    }
    private void PlayFootstepSound()
{
    if (audioSource != null && footstepSounds != null && footstepSounds.Length > 0)
    {
        // Check if enough time has passed since last footstep
        if (Time.time - lastStepTime >= minTimeBetweenSteps)
        {
            // Pick a random footstep sound
            AudioClip randomStep = footstepSounds[Random.Range(0, footstepSounds.Length)];
            
            // Randomize volume slightly
            float randomVolume = Random.Range(footstepVolumeMin, footstepVolumeMax);
            
            audioSource.PlayOneShot(randomStep, randomVolume);
            lastStepTime = Time.time;
        }
    }
}
        private void OnDrawGizmos()
    {
        if (showDebugGizmos && hipTransform != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(hipTransform.position, 0.1f);

            // Draw leg at current position
            Gizmos.DrawLine(hipTransform.position, transform.position);
        }
    }
}
