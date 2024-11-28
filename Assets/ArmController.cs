// ArmController.cs
using UnityEngine;
using System.Collections;

public class ArmController : MonoBehaviour
{
    private ItemSO _armItem;
    public float swingSpeed { get; private set; } = 5f; // Default value
    private float swingAngle;
    private int swingDirection = 1;
    private float baseLength = 1f; // Default value
    private float currentStretchLength;
    private Rigidbody2D rb;

    [Header("Speed Control")]
    public float speedIncreaseRate = 10f;  // How fast speed ramps up
    public float speedDecreaseRate = 5f;   // How fast speed ramps down

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseLength = transform.localScale.y;
        currentStretchLength = baseLength;
    }

    public float GetBaseSwingSpeed()
    {
        return _armItem != null ? _armItem.swingSpeed : 5f;
    }

    void Update()
    {
        SwingArm();
        if (_armItem != null && _armItem.isStretchy)
        {
            AdjustStretch();
        }
    }

    public void SetItem(ItemSO newItem)
    {
        if (newItem == null || newItem.itemType != ItemSO.ItemType.Arm)
        {
            Debug.LogError($"Invalid item assigned to ArmController on {gameObject.name}");
            return;
        }

        _armItem = newItem;
        UpdateItemProperties();
    }

    private void UpdateItemProperties()
    {
        if (_armItem == null) return;
        
        swingSpeed = _armItem.swingSpeed;
        baseLength = Mathf.Max(_armItem.length, 0.1f);
        currentStretchLength = baseLength;
        
        Vector3 currentScale = transform.localScale;
        currentScale.y = baseLength;
        transform.localScale = currentScale;
    }

    public void IncreaseSwingSpeed()
    {
        float maxAllowedSpeed = _armItem != null ? _armItem.maxSwingSpeed : 30f;
        swingSpeed = Mathf.Min(swingSpeed + (speedIncreaseRate * Time.deltaTime), maxAllowedSpeed);
        
        if (_armItem != null && _armItem.isStretchy)
        {
            float maxStretch = _armItem.maxStretchLength > 0 ? _armItem.maxStretchLength : 2f;
            currentStretchLength = Mathf.Min(currentStretchLength + 0.1f, maxStretch);
        }
    }

    public void DecaySwingSpeed(float baseSpeed)
    {
        if(swingSpeed > baseSpeed)
        {
            swingSpeed = Mathf.Max(swingSpeed - (speedDecreaseRate * Time.deltaTime), baseSpeed);
        }
    }

    void SwingArm()
    {
        swingAngle += swingSpeed * swingDirection * Time.deltaTime;
        if (swingAngle >= 45f || swingAngle <= -45f) // Max swing angle
        {
            swingDirection *= -1;
        }
        transform.localRotation = Quaternion.Euler(0f, 0f, swingAngle);
    }

    void AdjustStretch()
    {
        if (baseLength <= 0f)
        {
            Debug.LogError($"Invalid baseLength {baseLength} on {gameObject.name}");
            return;
        }

        float stretchFactor = Mathf.Max(currentStretchLength / baseLength, 0.1f);
        Vector3 currentScale = transform.localScale;
        currentScale.y = baseLength * stretchFactor;
        transform.localScale = currentScale;
    }

    public bool IsSwingingUp()
    {
        return swingDirection > 0 && swingAngle > 0;
    }

    public bool IsSwingingDown()
    {
        return swingDirection < 0 && swingAngle < 0;
    }

    public bool IsSwingingRight()
    {
        return swingDirection > 0;
    }

    public bool IsSwingingLeft()
    {
        return swingDirection < 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyDamage enemy = other.GetComponent<EnemyDamage>();
            if (enemy != null && _armItem != null)
            {
                enemy.TakeDamage(_armItem.damage);

                if (_armItem.isTentacle)
                {
                    StartCoroutine(GrabAndEat(enemy));
                }
                else
                {
                    Vector2 knockbackDir = other.transform.position - transform.position;
                    enemy.ApplyKnockback(knockbackDir.normalized);
                }
            }
        }
    }

    IEnumerator GrabAndEat(EnemyDamage enemy)
    {
        enemy.DisableMovement();

        Vector3 headPosition = transform.position + Vector3.up * 2f;
        while (Vector3.Distance(enemy.transform.position, headPosition) > 0.1f)
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, headPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }
}