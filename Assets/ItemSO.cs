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
    public BodySide side;  // This will show up as a dropdown in the inspector

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
}