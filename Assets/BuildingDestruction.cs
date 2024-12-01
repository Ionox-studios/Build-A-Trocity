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
    public float fragmentLifetime = 2f;
    public ExplosionForce explosionForcePrefab;

    public AudioClip explosionSound;
    public AudioClip hitSound;

    // Flash parameters
    public float flashDuration = 0.1f;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;
        material.SetFloat("_DamageLevel", 0f);
        explodable = GetComponent<Explodable>();
        originalColor = spriteRenderer.color;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Monster"))
        {
            ApplyDamage(34f, true);
        }
        if (collision.collider.gameObject.CompareTag("MonsterInstaKill"))
        {
            ApplyDamage(1000f, false);
        }
    }

    void ApplyDamage(float damage, bool isNormalMonster)
    {
        currentHealth -= damage;
        float damageLevel = 1f - (currentHealth / maxHealth);
        material.SetFloat("_DamageLevel", damageLevel);

        if (isNormalMonster)
        {
            if (hitSound != null)
            {
                AudioManager.Instance.PlaySound(hitSound, transform.position, 0.5f);
            }
            if (!isFlashing)
            {
                StartCoroutine(FlashRed());
            }
        }

        if (currentHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    IEnumerator FlashRed()
    {
        isFlashing = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    void DestroyBuilding()
    {
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
        
        Vector3 explosionPos = transform.position;

        if (explosionForcePrefab != null)
        {
            ExplosionForce explosion = Instantiate(explosionForcePrefab, explosionPos, Quaternion.identity);
            explosion.doExplosion(explosionPos);
        }
        if (explosionSound != null)
        {
            AudioManager.Instance.PlaySound(explosionSound, transform.position, 0.5f);
        }
    }
}