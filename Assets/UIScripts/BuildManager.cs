// BuildManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }
    
    private Dictionary<ItemSO.ItemType, BuildSpot> buildSpots = new Dictionary<ItemSO.ItemType, BuildSpot>();
    private Dictionary<ItemSO.ItemType, ItemSO> assembledParts = new Dictionary<ItemSO.ItemType, ItemSO>();
    
    public System.Action<Dictionary<ItemSO.ItemType, ItemSO>> OnMonsterCompleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterBuildSpot(BuildSpot spot)
    {
        if (!buildSpots.ContainsKey(spot.GetAcceptedType()))
        {
            buildSpots.Add(spot.GetAcceptedType(), spot);
        }
    }

    public void PlacePart(ItemSO part, BuildSpot spot)
    {
        if (assembledParts.ContainsKey(part.itemType))
        {
            assembledParts[part.itemType] = part;
        }
        else
        {
            assembledParts.Add(part.itemType, part);
        }
        
        CheckCompletion();
    }

    public void RemovePart(ItemSO.ItemType type)
    {
        if (assembledParts.ContainsKey(type))
        {
            assembledParts.Remove(type);
        }
    }

    public bool ValidateMonster()
    {
        foreach (var spot in buildSpots.Values)
        {
            if (spot.IsRequired() && !assembledParts.ContainsKey(spot.GetAcceptedType()))
            {
                return false;
            }
        }
        return true;
    }

    private void CheckCompletion()
    {
        if (ValidateMonster())
        {
            FillMissingParts();
            OnMonsterCompleted?.Invoke(assembledParts);
        }
    }

    private void FillMissingParts()
    {
        foreach (var spot in buildSpots.Values)
        {
            var type = spot.GetAcceptedType();
            if (!assembledParts.ContainsKey(type) && spot.GetDefaultItem() != null)
            {
                assembledParts.Add(type, spot.GetDefaultItem());
            }
        }
    }

    public Dictionary<ItemSO.ItemType, ItemSO> GetAssembledParts()
    {
        return new Dictionary<ItemSO.ItemType, ItemSO>(assembledParts);
    }
}