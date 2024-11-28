using UnityEngine;

public class ShowMeshRendererSorting : MonoBehaviour 
{
    void OnValidate()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            meshRenderer.sortingOrder = meshRenderer.sortingOrder;
        }
    }
}