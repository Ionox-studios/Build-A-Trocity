using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Assuming the enemy has a Health component
        //Health enemyHealth = collision.GetComponent<Health>();
        //if (enemyHealth != null)
        //{
         //   enemyHealth.TakeDamage(damage);
        //}
    }
}
