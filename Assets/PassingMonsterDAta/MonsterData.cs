// MonsterData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject 
{
    public ItemSO leftArm;
    public ItemSO rightArm;
    public ItemSO leftLeg;
    public ItemSO rightLeg;
    public ItemSO head;
    public ItemSO torso;

    public void UpdateFromDictionary(Dictionary<ItemSO.ItemType, ItemSO> parts)
    {
        if (parts.TryGetValue(ItemSO.ItemType.Arm, out ItemSO armItem))
        {
            // Assuming the first arm added is left, second is right
            if (leftArm == null) leftArm = armItem;
            else rightArm = armItem;
        }
        if (parts.TryGetValue(ItemSO.ItemType.Leg, out ItemSO legItem))
        {
            if (leftLeg == null) leftLeg = legItem;
            else rightLeg = legItem;
        }
        if (parts.TryGetValue(ItemSO.ItemType.Head, out ItemSO headItem))
        {
            head = headItem;
        }
        if (parts.TryGetValue(ItemSO.ItemType.Torso, out ItemSO torsoItem))
        {
            torso = torsoItem;
        }
    }
}