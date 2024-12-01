using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Arm,
        Leg,
        Head,
        Torso
    }
    public enum BodySide
    {
        Left,
        Right
    }

    [Header("Basic Properties")]
    public string itemName;
    public Sprite icon;
    public string description;
    public ItemType itemType;

    [Header("Limb Properties")]
    [SerializeField]
    public BodySide side;

    [Header("Arm Properties")]
    public bool isTentacle;
    public bool isStretchy;
    public float maxStretchLength = 2f;
    public int damage = 10;
    public float swingSpeed = 5f;
    public float maxSwingSpeed = 30f;

    [Header("Leg Properties")]
    public float length = 1f;
    public float speed = 1f;

    public enum HeadEffectType
    {
        None,
        SpeedIncrease,
        EnemyFearReduce,
        DamageCollider,
        LaserShoot,
        GhostArms,
         Vampire  // Add this new type
    }

    public HeadEffectType headEffectType;

    // Additional properties for specific effects
    public GameObject laserPrefab; // For LaserShoot effect
    public int damageAmount = 10;  // For DamageCollider and LaserShoot

    [Header("Sound Effects")]
    public AudioClip useSound;          // Played when item is used/activated

    [Header("Torso Properties")]
public float torsoSpeed = 1f;
public int torsoHealth = 4;
[Header("Gun Properties")]
public bool isGun = false;
public GameObject bulletPrefab;
public float bulletSpeed = 10f;
public float fireRate = 0.5f;

}
