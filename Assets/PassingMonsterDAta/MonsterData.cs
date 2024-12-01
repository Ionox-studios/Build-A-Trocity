// MonsterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject 
{
    public ItemSO leftArm;
    public ItemSO rightArm;
    public ItemSO leftLeg;
    public ItemSO rightLeg;
    public ItemSO head;
    public ItemSO torso;
}
