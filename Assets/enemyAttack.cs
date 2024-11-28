using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1f;
    public int damageAmount = 1;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    [Header("Smear Effect")]
    public float dashDuration = 0.3f;
    public float dashSpeed = 15f;
    public int smearCount = 6;
    public float smearSpacing = 0.05f;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private SpriteRenderer[] smearSprites;
    private GameObject smearContainer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        CreateSmearSprites();
    }

    private void CreateSmearSprites()
    {
        smearContainer = new GameObject("SmearContainer");
        smearContainer.transform.parent = transform;
        smearSprites = new SpriteRenderer[smearCount];

        for (int i = 0; i < smearCount; i++)
        {
            GameObject smearObj = new GameObject($"Smear_{i}");
            smearObj.transform.parent = smearContainer.transform;
            SpriteRenderer smearRenderer = smearObj.AddComponent<SpriteRenderer>();
            
            smearRenderer.sprite = spriteRenderer.sprite;
            smearRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
            smearRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
            smearRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            
            smearSprites[i] = smearRenderer;
            smearObj.SetActive(false);
        }
    }

    private void Update()
    {
        if (isAttacking) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && Time.time >= nextAttackTime)
            {
                StartCoroutine(PerformAttack(hitCollider));
                break;
            }
        }
    }

    private IEnumerator PerformAttack(Collider2D playerCollider)
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        Vector3 direction = (playerCollider.transform.position - transform.position).normalized;
        Vector3 startPos = transform.position;
        
        foreach (var smear in smearSprites)
        {
            smear.gameObject.SetActive(true);
            smear.color = new Color(1f, 1f, 1f, 0.5f);
        }

        float elapsedTime = 0;
        while (elapsedTime < dashDuration)
        {
            float progress = elapsedTime / dashDuration;
            
            for (int i = 0; i < smearCount; i++)
            {
                float smearProgress = progress - (i * smearSpacing);
                if (smearProgress > 0 && smearProgress < 1)
                {
                    Vector3 smearPos = Vector3.Lerp(startPos, playerCollider.transform.position, smearProgress);
                    smearSprites[i].transform.position = smearPos;
                    
                    float alpha = Mathf.Sin(smearProgress * Mathf.PI) * 0.8f + 0.2f;
                    smearSprites[i].color = new Color(1f, 1f, 1f, alpha);
                    smearSprites[i].transform.localScale = transform.localScale;
                    smearSprites[i].gameObject.SetActive(true);
                }
                else
                {
                    smearSprites[i].gameObject.SetActive(false);
                }
            }

            rb.linearVelocity = direction * dashSpeed;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount, transform.position);
        }

        rb.linearVelocity = Vector2.zero;
        
        foreach (var smear in smearSprites)
        {
            smear.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}