using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDestruction : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    private Explodable explodable;
    public float fragmentLifetime = 2f;
    public ExplosionForce explosionForcePrefab;
    
    // Add reference to blood particle system prefab
    public ParticleSystem bloodEffectPrefab;
    public AudioClip squishSound;
    
    void Start()
    {
        currentHealth = maxHealth;
        explodable = GetComponent<Explodable>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            ApplyDamage(34f);
        }
    }

    void ApplyDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }

    void DestroyEnemy()
    {
        // Explode the enemy into fragments
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
        
        ScoreManagerNight.Instance.CultistDefeated();
        Destroy(gameObject);
        Vector3 explosionPos = transform.position;

        // Create explosion force
        if (explosionForcePrefab != null)
        {
            ExplosionForce explosion = Instantiate(explosionForcePrefab, explosionPos, Quaternion.identity);
            explosion.doExplosion(explosionPos);
        }

        // Spawn blood particle effect
        if (bloodEffectPrefab != null)
        {
            ParticleSystem bloodEffect = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            bloodEffect.Play();
            
            // Destroy the particle system after its duration
            float duration = bloodEffect.main.duration;
            Destroy(bloodEffect.gameObject, duration + 1f);
        }
        AudioManager.Instance.PlaySound(squishSound, transform.position,0.5f);
    }
}