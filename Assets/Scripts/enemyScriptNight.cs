using UnityEngine;
using Pathfinding;

public class enemyScriptNight : MonoBehaviour
{
    [Header("Detection Settings")]
    public float baseDetectionRadius = 5f;
        private float currentDetectionRadius; // New variable for actual detection radius

    
    [Header("Movement Settings")]
    public float fleeSpeed = 2f;
    public float basefleeTargetDistance = 10f;
     private float currentFleeDistance; // New variable for actual flee spee
    

        [Header("Game State")]
    public bool reduceFear = false; // New variable for fear reduction
    [Header("References")]
    public AIPath aiPath;
    public AIDestinationSetter destinationSetter;
    public Transform player;
    public Transform visualTransform;
    
    private Transform tempTarget;
    private bool isFleeing = false;
    private Vector3 startPosition;
    private bool isValidFleePoint = false;
    private float originalZ;
[Header("Audio")]
public AudioSource audioSource;
public AudioClip screamSound;
    void Start()
    {
        if (aiPath == null) aiPath = GetComponent<AIPath>();
        if (destinationSetter == null) destinationSetter = GetComponent<AIDestinationSetter>();
        if (visualTransform == null) visualTransform = transform;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();


        
        originalZ = transform.position.z;
        startPosition = transform.position;
        startPosition.z = originalZ;
        // Initialize current values
        UpdateFearValues();
    }

        void UpdateFearValues()
    {
        // If fear is reduced, halve the detection radius and flee speed
        currentDetectionRadius = reduceFear ? baseDetectionRadius * 0.5f : baseDetectionRadius;
        currentFleeDistance = reduceFear ? basefleeTargetDistance * 0.5f : basefleeTargetDistance;
    }

    Transform CreateTempTarget(Vector3 position)
    {
        if (tempTarget != null)
            Destroy(tempTarget.gameObject);
            
        GameObject temp = new GameObject("TempTarget");
        tempTarget = temp.transform;
        position.z = originalZ;
        tempTarget.position = position;
        return tempTarget;
    }

    Vector3 FindFleePosition()
    {
        Vector2 playerPos2D = new Vector2(player.position.x, player.position.y);
        Vector2 myPos2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 directionFromPlayer = (myPos2D - playerPos2D).normalized;
        
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector2 rotatedDirection = new Vector2(
                directionFromPlayer.x * Mathf.Cos(angle) - directionFromPlayer.y * Mathf.Sin(angle),
                directionFromPlayer.x * Mathf.Sin(angle) + directionFromPlayer.y * Mathf.Cos(angle)
            );

            Vector2 potentialTarget2D = myPos2D + (rotatedDirection * currentFleeDistance);
            Vector3 potentialTarget = new Vector3(potentialTarget2D.x, potentialTarget2D.y, originalZ);
            
            GraphNode node = AstarPath.active.GetNearest(potentialTarget).node;
            if (node != null && node.Walkable)
            {
                isValidFleePoint = true;
                return potentialTarget;
            }
        }

        isValidFleePoint = false;
        Vector2 fallbackTarget = myPos2D + (directionFromPlayer * currentFleeDistance);
        return new Vector3(fallbackTarget.x, fallbackTarget.y, originalZ);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(player.position.x, player.position.y)
        );

        if (transform.position.z != originalZ)
        {
            Vector3 pos = transform.position;
            pos.z = originalZ;
            transform.position = pos;
        }

        // If we're not currently fleeing, check if we need to start
        if (!isFleeing)
        {
            if (distanceToPlayer <= currentDetectionRadius)
            {
                StartFleeing();
            }
        }
        // If we are fleeing, check if we've reached our destination
        else if (aiPath.reachedDestination)
        {
            isFleeing = false;
            aiPath.canMove = false;
            
            // Immediately check if we need to flee again
            if (distanceToPlayer <= currentDetectionRadius)
            {
                StartFleeing();
            }
        }

        // Update facing direction while moving
        if (aiPath.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 velocity2D = new Vector2(aiPath.velocity.x, aiPath.velocity.y);
            SetFacingDirection(velocity2D.normalized);
        }
    }

    void StartFleeing()
    {
        isFleeing = true;
        Vector3 fleePosition = FindFleePosition();
        destinationSetter.target = CreateTempTarget(fleePosition);
        aiPath.maxSpeed = fleeSpeed;
        aiPath.canMove = true;
            // Play scream sound if we have both the audio source and clip
    if (audioSource != null && screamSound != null)
    {
        audioSource.PlayOneShot(screamSound);
    }
    }

    public void SetFacingDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        visualTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, currentDetectionRadius);

        // Draw flee target if fleeing
        if (isFleeing && tempTarget != null)
        {
            Gizmos.color = isValidFleePoint ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(tempTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position, tempTarget.position);
        }
    }
}