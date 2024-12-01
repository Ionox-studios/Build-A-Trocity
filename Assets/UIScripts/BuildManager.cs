// BuildManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance { get; private set; }

    private List<BuildSpot> buildSpots = new List<BuildSpot>();

    public System.Action OnPartChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBuildSpot(BuildSpot spot)
    {
        if (!buildSpots.Contains(spot))
        {
            buildSpots.Add(spot);
        }
    }

    public List<BuildSpot> GetAllBuildSpots()
    {
        return buildSpots;
    }

    public void PlacePart(ItemSO part, BuildSpot spot)
    {
        // Notify that a part has changed
        OnPartChanged?.Invoke();
    }

    public void RemovePart(ItemSO.ItemType type)
    {
        // Notify that a part has changed
        OnPartChanged?.Invoke();
    }

    public bool AreAllSpotsFilled()
    {
        foreach (var spot in buildSpots)
        {
            if (!spot.HasItem())
            {
                return false;
            }
        }
        return true;
    }
}
