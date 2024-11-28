using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Animator animator;
    private Vector2 previousPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Calculate movement direction
        Vector2 currentPosition = transform.position;
        Vector2 movement = currentPosition - previousPosition;

        // Update previous position for the next frame
        previousPosition = currentPosition;

        if (movement != Vector2.zero)
        {
            // Normalize to get the direction
            Vector2 direction = movement.normalized;

            // Update animator parameters
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
            animator.SetBool("moving", true); //AL
        }
        else
        {
            // Optionally, handle the idle state
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
            animator.SetBool("moving", false); //AL
        }
    }
}
