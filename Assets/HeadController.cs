using UnityEngine;
using TMPro; // Add this for TextMeshPro
public class HeadController : MonoBehaviour
{
    public ItemSO headItem;
    public GameObject[] arms; // Reference to arm GameObjects

    private MonsterControllerSimple monsterController;
    private Rigidbody2D rb;
   private HeartTimer heartTimer;
    [SerializeField] private float vampireTimeRestore = 10f;
    private bool isVampireActive = false;
    [SerializeField] private float laserSpeed = 10f;
    [SerializeField] private GameObject cooldownGlow;
    private float nextFireTime = 0f;
        private AudioSource audioSource;
    private float nextSoundTime = 0f;
       [SerializeField] private TMP_Text hintText;
    private float hintFadeTime = 5f;
    private float hintTimer = 0f;
    private bool hintShown = false;

    [SerializeField] private float soundDelay = 5f; // Delay after sound finishes

    public void Setup()
    {
        monsterController = GetComponentInParent<MonsterControllerSimple>();
        rb = GetComponentInParent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();

        
        // Initialize glow state
        if (cooldownGlow != null)
        {
            cooldownGlow.SetActive(false);
        }

        ApplyHeadEffect();
    }

    void Update()
    {
        Debug.Log(headItem.headEffectType);
        if (headItem != null && headItem.headEffectType == ItemSO.HeadEffectType.LaserShoot)
        {
            // Handle glow visibility
            if (cooldownGlow != null)
            {
                cooldownGlow.SetActive(Time.time >= nextFireTime);
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Shoot laser");
                ShootLaser();
            }
                        if (!hintShown && hintText != null)
            {
                hintText.gameObject.SetActive(true);
                hintText.text = "Press space to shoot, can shoot again when glow returns";
                hintText.color = new Color(1, 1, 1, 1);
                hintShown = true;
                hintTimer = Time.time;
            }

            if (hintShown && Time.time > hintTimer + hintFadeTime)
            {
                float alpha = 1f - ((Time.time - (hintTimer + hintFadeTime)) / 0.5f);
                if (alpha <= 0)
                {
                    hintText.gameObject.SetActive(false);
                }
                else
                {
                    hintText.color = new Color(1, 1, 1, alpha);
                }
            }
        }
        else
        {
            // No laser head, ensure glow is off
        if (cooldownGlow != null)
            {
                cooldownGlow.SetActive(false);
            }
            hintShown = false;
            if (hintText != null)
            {
                hintText.gameObject.SetActive(false);
            }
        }
        if (!audioSource.isPlaying && Time.time >= nextSoundTime)
        {
            PlayHeadSound();
        }

        
    }

    void ApplyHeadEffect()
    {
        if (headItem == null)
            return;

        switch (headItem.headEffectType)
        {
            case ItemSO.HeadEffectType.SpeedIncrease:
                if (monsterController != null)
                {
                    monsterController.moveSpeed *= 1.5f;
                }
                break;
            case ItemSO.HeadEffectType.Vampire:
                isVampireActive = true;
                break;
            case ItemSO.HeadEffectType.EnemyFearReduce:
                if (monsterController != null)
                {
                    monsterController.reduceEnemyFear = true;
                }
                break;
            case ItemSO.HeadEffectType.DamageCollider:
            audioSource.volume = audioSource.volume * 0.1f;
                AddDamageCollider();
                break;
            case ItemSO.HeadEffectType.LaserShoot:
                // Handled in Update()
                break;
            case ItemSO.HeadEffectType.GhostArms:

                ActivateGhostArms();
                break;
            default:
                break;
        }
    }

    void ActivateGhostArms()
    {
        if (arms == null || arms.Length == 0)
            return;
        Debug.Log("we ballish");
        foreach (GameObject arm in arms)
        {
            // Disable the regular collider on the arm
            Debug.Log("we ball");
            PolygonCollider2D armCollider = arm.GetComponent<PolygonCollider2D>();
            Debug.Log(armCollider);
            if (armCollider != null)
            {
                armCollider.enabled = false;
            }

            // Find and enable the ghost collider child object
            Transform ghostCollider = arm.transform.Find("GhostCollider");
            if (ghostCollider != null)
            {
                ghostCollider.gameObject.SetActive(true);
            }
        }
    }

    void AddDamageCollider()
    {
        gameObject.tag = "MonsterInstaKill";
    }

    void PlayHeadSound()
    {
        if (headItem.useSound != null)
        {
            audioSource.clip = headItem.useSound;
            audioSource.Play();
            // Set next sound time to current time + clip length + delay
            nextSoundTime = Time.time + headItem.useSound.length + soundDelay;
        }
    }
    void ShootLaser()
    {
        if (headItem.laserPrefab != null && Time.time >= nextFireTime)
        {
            Debug.Log("Shoot laser2");
            GameObject laser = Instantiate(headItem.laserPrefab, transform.position, transform.rotation);
            Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();
            laser.tag = "MonsterInstaKill";
            if (laserRb != null)
            {
                laserRb.linearVelocity = transform.right * laserSpeed;
            }

            DamageDealer damageDealer = laser.GetComponent<DamageDealer>();
            if (damageDealer != null)
            {
                damageDealer.damage = headItem.damageAmount;
            }
            
            nextFireTime = Time.time + 1f;
            
            // Turn off glow when firing
            if (cooldownGlow != null)
            {
                cooldownGlow.SetActive(false);
            }
        }
    }
}