using UnityEngine;
using Pathfinding;

public class enemyScriptDay : MonoBehaviour
{
    [Header("Detection Settings")]
    public float outerRadius = 10f;
    public float innerRadius = 5f;
    public float viewAngle = 90f;
    
    [Header("Movement Settings")]
    public float normalSpeed = 1f;
    public float chaseSpeed = 2f;
    public float stopDistance = 0.1f;
    
    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float waitTimeMin = 2f;
    public float waitTimeMax = 5f;
    public float playerMemoryTime = 3f;

    [Header("References")]
    public AIPath aiPath;
    public AIDestinationSetter destinationSetter;
    public Transform player;
    public Transform visualTransform;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip outerRadiusClip;
    public AudioClip innerRadiusClip;
    public AudioClip playerLostClip;

    private Vector3 lastKnownPosition;
    private bool wasPlayerDetected = false;
    private bool hasLineOfSight = false;
    private Transform tempTarget;
    private bool isPlayerInRange = false;
    private Vector3 startPosition;
    private Vector3 currentPatrolTarget;
    private float waitTimer;
    private float playerLostTimer;
    private bool isWaiting = false;
    private bool isPatrolling = true;
    private bool needsNewPatrolTarget = true;  // New variable to track when we need a new target
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    public float stuckThreshold = 0.1f; // Minimum distance to consider as movement
    public float stuckTimeLimit = 2f;   // Time limit to consider the enemy as stuck
    private bool isOuterRadiusPlaying = false;
    private bool isInnerRadiusPlaying = false;
    private bool shouldPlayLostClip = false;
    private bool canPlayInnerClip = false;

    void Start()
    {
        if (aiPath == null) aiPath = GetComponent<AIPath>();
        if (destinationSetter == null) destinationSetter = GetComponent<AIDestinationSetter>();
        if (visualTransform == null) visualTransform = transform;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        
        aiPath.maxSpeed = normalSpeed;
        startPosition = transform.position;
        SetNewPatrolTarget(); // Initial patrol point
    }

    void SetNewPatrolTarget()
    {
        stuckTimer = 0f;  // Reset stuck timer when setting new target
        lastPosition = transform.position;  // Reset last position

        int maxAttempts = 5;
        int attempts = 0;
        bool foundValidPoint = false;

        while (!foundValidPoint && attempts < maxAttempts)
        {
            Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
            Vector3 potentialTarget = startPosition + new Vector3(randomPoint.x, randomPoint.y, 0);
            potentialTarget.z = startPosition.z;

            // Check if the point is walkable using A* GraphNode
            GraphNode node = AstarPath.active.GetNearest(potentialTarget).node;
            if (node != null && node.Walkable)
            {
                currentPatrolTarget = potentialTarget;
                destinationSetter.target = CreateTempTarget(currentPatrolTarget);
                aiPath.maxSpeed = normalSpeed;
                aiPath.canMove = true;
                needsNewPatrolTarget = false;
                foundValidPoint = true;
            }

            attempts++;
        }

        // If we couldn't find a valid point, return to start position
        if (!foundValidPoint)
        {
            currentPatrolTarget = startPosition;
            destinationSetter.target = CreateTempTarget(currentPatrolTarget);
            aiPath.maxSpeed = normalSpeed;
            aiPath.canMove = true;
            needsNewPatrolTarget = false;
        }
    }

        Transform CreateTempTarget(Vector3 position)
        {
            if (tempTarget != null)
                Destroy(tempTarget.gameObject);
                
            GameObject temp = new GameObject("TempTarget");
            tempTarget = temp.transform;
            position.z = transform.position.z; // Ensure Z coordinate matches
            tempTarget.position = position;
            return tempTarget;
        }


    private Vector2 GetForwardDirection()
    {
        float angle = visualTransform.rotation.eulerAngles.z;
        return new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );
    }

    void Update()
    {
        if (player == null) return;
        // Stuck detection
    if (isOuterRadiusPlaying && !audioSource.isPlaying)
    {
        isOuterRadiusPlaying = false;
        canPlayInnerClip = true;
    }

    if (aiPath.velocity.sqrMagnitude > 0.01f)
    {
    Vector2 moveDirection = aiPath.velocity.normalized;
    SetFacingDirection(moveDirection);
    }
    // Stuck detection - only check when we're actively moving to a patrol point
    if (isPatrolling && !isWaiting && !aiPath.reachedDestination)
    {
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        if (movedDistance < stuckThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTimeLimit)
            {
                // We're stuck, get a new patrol target
                needsNewPatrolTarget = true;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
        lastPosition = transform.position;
    }

        Vector2 directionToPlayer = (Vector2)(player.position - transform.position);
        float distanceToPlayer = directionToPlayer.magnitude;
        Vector2 forward = GetForwardDirection();

        // Check if player is within outer radius
        if (distanceToPlayer <= outerRadius)
        {
            float angle = Vector2.Angle(forward, directionToPlayer.normalized);
            
            if (angle <= viewAngle / 2)
            {
                hasLineOfSight = CheckLineOfSight(player.position);
                
                if (hasLineOfSight)
                {
                    isPlayerInRange = true;
                    wasPlayerDetected = true;
                    isPatrolling = false;
                    lastKnownPosition = player.position;
                    playerLostTimer = playerMemoryTime;
                    // let it move
                    aiPath.canMove = true;
                    // Initial detection audio
                    if (!isOuterRadiusPlaying && !audioSource.isPlaying && !canPlayInnerClip)
                    {
                        audioSource.clip = outerRadiusClip;
                        audioSource.Play();
                        isOuterRadiusPlaying = true;
                    }

                    // Play inner radius clip after outer radius clip finishes
                    if (canPlayInnerClip && !audioSource.isPlaying && !isInnerRadiusPlaying)
                    {
                        audioSource.clip = innerRadiusClip;
                        audioSource.Play();
                        isInnerRadiusPlaying = true;
                        canPlayInnerClip = false;
                    }
                    
                    if (distanceToPlayer <= innerRadius)
                    {
                        aiPath.maxSpeed = chaseSpeed;
                    }
                    else
                    {
                        aiPath.maxSpeed = normalSpeed;
                    }
                    
                    destinationSetter.target = player;
                }
            }
        }
        else
        {
            isPlayerInRange = false;
        }

        // Handle losing the player
        if (wasPlayerDetected && (!hasLineOfSight || !isPlayerInRange))
        {
            playerLostTimer -= Time.deltaTime;
            
            if (playerLostTimer <= 0)
            {
                // Play lost player audio when previous audio is finished
                if (!audioSource.isPlaying && !shouldPlayLostClip)
                {
                    audioSource.clip = playerLostClip;
                    audioSource.Play();
                    shouldPlayLostClip = true;
                }

                wasPlayerDetected = false;
                isPatrolling = true;
                SetNewPatrolTarget();
                aiPath.canMove = true;
                if (!audioSource.isPlaying && shouldPlayLostClip)
                {
                    isOuterRadiusPlaying = false;
                    isInnerRadiusPlaying = false;
                    shouldPlayLostClip = false;
                    canPlayInnerClip = false;
                }
  
            }
            else if (destinationSetter.target != tempTarget)
            {
                // Move to last known position
                lastKnownPosition.z = transform.position.z; // Ensure Z coordinate matches
                destinationSetter.target = CreateTempTarget(lastKnownPosition);
                aiPath.maxSpeed = normalSpeed;
                aiPath.canMove = true; // Ensure the enemy can move
            }
        }


        // Handle patrol behavior
        // Update the patrol section in Update():
        // Replace the existing patrol behavior in Update() with this:
        if (isPatrolling)
        {
            if (aiPath.reachedDestination)
            {
                if (!isWaiting)
                {
                    // Only start waiting when first reaching the destination
                    isWaiting = true;
                    waitTimer = Random.Range(waitTimeMin, waitTimeMax);
                    aiPath.canMove = false;
                }
                else
                {
                    // Only count down timer when we're at the destination and waiting
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0)
                    {
                        // Finished waiting, get new target
                        isWaiting = false;
                        needsNewPatrolTarget = true;
                        aiPath.canMove = true;
                    }
                }
            }
            else
            {
                // We're not at the destination, so we're not waiting
                isWaiting = false;
                needsNewPatrolTarget = false;
                aiPath.canMove = true;
            }

            if (needsNewPatrolTarget && !isWaiting)
            {
                SetNewPatrolTarget();
                aiPath.canMove = true;
            }
        }


    }

    bool CheckLineOfSight(Vector3 targetPosition)
    {
        Vector2 directionToTarget = (targetPosition - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToTarget,
            outerRadius,
            LayerMask.GetMask("Default", "Player","TransparentFX","Obstacle")
        );

        Debug.DrawRay(transform.position, directionToTarget * outerRadius, Color.red);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    public void SetFacingDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        visualTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying && visualTransform == null)
            visualTransform = transform;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, innerRadius);
        
        Vector2 forward = GetForwardDirection();
        float halfAngle = viewAngle * 0.5f * Mathf.Deg2Rad;
        
        Vector2 leftDir = new Vector2(
            forward.x * Mathf.Cos(-halfAngle) - forward.y * Mathf.Sin(-halfAngle),
            forward.x * Mathf.Sin(-halfAngle) + forward.y * Mathf.Cos(-halfAngle)
        );
        
        Vector2 rightDir = new Vector2(
            forward.x * Mathf.Cos(halfAngle) - forward.y * Mathf.Sin(halfAngle),
            forward.x * Mathf.Sin(halfAngle) + forward.y * Mathf.Cos(halfAngle)
        );

        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(leftDir * outerRadius));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rightDir * outerRadius));
        
        int segments = 20;
        Vector3 previousPoint = transform.position + (Vector3)(leftDir * outerRadius);
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector2 direction = new Vector2(
                forward.x * Mathf.Cos(currentAngle) - forward.y * Mathf.Sin(currentAngle),
                forward.x * Mathf.Sin(currentAngle) + forward.y * Mathf.Cos(currentAngle)
            );
            Vector3 currentPoint = transform.position + (Vector3)(direction * outerRadius);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(forward * 2));

        if (wasPlayerDetected)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        }

        // Draw patrol radius
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(startPosition, patrolRadius);
        }
    }
}