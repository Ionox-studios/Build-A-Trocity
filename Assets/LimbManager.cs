using Unity.Burst.Intrinsics;
using UnityEngine;

public class LimbManager : MonoBehaviour
{
    [SerializeField] private ItemSO _itemSO;
    
    private ArmController _armController;
    private LegController _legController;
    private ArmControllerSimple _armControllerSimple;
    private LimbVisuals _limbVisuals;

    private void Awake()
    {
        _armController = GetComponent<ArmController>();
        _legController = GetComponent<LegController>();
        _limbVisuals = GetComponent<LimbVisuals>();
    }

    private void Start()
    {
        if (_itemSO != null)
        {
            SetItem(_itemSO);
        }
    }

    public void SetItem(ItemSO newItem)
    {
        _itemSO = newItem;
        
        if (_armController != null) _armController.SetItem(newItem);
        if (_legController != null) _legController.SetItem(newItem);
        if (_limbVisuals != null) _limbVisuals.SetItemSO(newItem);
    }
}