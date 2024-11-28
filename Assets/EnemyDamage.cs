using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int health = 100;
    private Rigidbody2D rb;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        rb.AddForce(direction * 500f); // Adjust force as needed
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }

    void Die()
    {
        // Add death effects or animations here
        Destroy(gameObject);
    }
}
