using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingDestruction : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    private SpriteRenderer spriteRenderer;
    private Material material;
    private Explodable explodable;
    public float fragmentLifetime = 2f; // How long fragments live
    // Reference to explosion force prefab if needed
    public ExplosionForce explosionForcePrefab;

    public AudioClip explosionSound;
    
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.SetFloat("_DamageLevel", 0f);
        explodable = GetComponent<Explodable>();

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            ApplyDamage(34f); // Adjust damage as needed
        }
    }

    void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        float damageLevel = 1f - (currentHealth / maxHealth);
        material.SetFloat("_DamageLevel", damageLevel);

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    void DestroyBuilding()
    {
        // First explode the building into fragments
        explodable.explode();
               foreach (GameObject fragment in explodable.fragments)
        {
            if (fragment != null)
            {
                Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 0f;
                    Destroy(fragment, fragmentLifetime);
                }
            }
        }
        
        // Get the explosion position (current object position)
        Vector3 explosionPos = transform.position;

        // Create explosion force at the position
        if (explosionForcePrefab != null)
        {
            ExplosionForce explosion = Instantiate(explosionForcePrefab, explosionPos, Quaternion.identity);
            explosion.doExplosion(explosionPos);
        }
        if (explosionSound != null)
        {
            AudioManager.Instance.PlaySound(explosionSound, transform.position,0.5f);
        }
    }

}