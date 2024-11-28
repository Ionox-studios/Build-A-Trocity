using UnityEngine;

public class BodyPartsUI : MonoBehaviour 
{
    [SerializeField] private GameObject torsoSquare;
    [SerializeField] private GameObject headSquare;
    [SerializeField] private GameObject leftArmSquare;
    [SerializeField] private GameObject rightArmSquare;
    [SerializeField] private GameObject leftLegSquare;
    [SerializeField] private GameObject rightLegSquare;

    public void RemoveBodyPartSquare(ItemSO.ItemType partType)
    {
        switch(partType)
        {
            case ItemSO.ItemType.Torso:
                torsoSquare.SetActive(false);
                break;
                
            case ItemSO.ItemType.Head:
                headSquare.SetActive(false);
                break;
                
            case ItemSO.ItemType.Arm:
                if(leftArmSquare.activeSelf)
                {
                    leftArmSquare.SetActive(false);
                }
                else
                {
                    rightArmSquare.SetActive(false);
                }
                break;
                
            case ItemSO.ItemType.Leg:
                if(leftLegSquare.activeSelf)
                {
                    leftLegSquare.SetActive(false);
                }
                else
                {
                    rightLegSquare.SetActive(false);
                }
                break;
        }
    }
}