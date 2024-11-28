using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform target;
    [Range(0.01f, 1f)]
    public float smoothness = 0.1f;
    public Vector3 offset;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z); //AL
        if (target != null)
        {
            // Set initial position
            transform.position = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z
            );
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothness
        );
    }
}
