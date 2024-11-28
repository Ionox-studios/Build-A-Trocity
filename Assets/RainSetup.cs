#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RainSetup : MonoBehaviour 
{
    [Header("Rain Settings")]
    public float startSpeed = 15f;
    public float startSize = 0.1f;
    public float lifetime = 2f;
    public float emissionRate = 500;
    
    [Header("Area Settings")]
    public Vector3 rainAreaScale = new Vector3(20f, 0.1f, 20f);
    public Vector3 spawnPosition = new Vector3(0, 10, 0);
    
    [Header("Particle Settings")]
    public Color rainColor = new Color(0.8f, 0.8f, 1f, 0.5f);
    public Material rainMaterial;
    
    [Header("Save Settings")]
    public string prefabPath = "Assets/Prefabs/RainSystem.prefab";
    
    [ContextMenu("Create And Save Rain Prefab")]
    void CreateAndSaveRainSystem()
    {
        // Create rain object
        GameObject rainObject = new GameObject("RainSystem");
        ParticleSystem rainParticles = rainObject.AddComponent<ParticleSystem>();
        
        // Setup main module
        var main = rainParticles.main;
        main.loop = true;
        main.startSpeed = startSpeed;
        main.startSize = startSize;
        main.startLifetime = lifetime;
        main.startColor = rainColor;
        
        // Emission
        var emission = rainParticles.emission;
        emission.rateOverTime = emissionRate;
        
        // Shape
        var shape = rainParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = rainAreaScale;
        
        // Position
        rainObject.transform.position = spawnPosition;
        
        // Material
        if (rainMaterial != null)
        {
            var renderer = rainObject.GetComponent<ParticleSystemRenderer>();
            renderer.material = rainMaterial;
        }
        
        // Create the directory if it doesn't exist
        string directory = System.IO.Path.GetDirectoryName(prefabPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }
        
        // Save as prefab
        PrefabUtility.SaveAsPrefabAsset(rainObject, prefabPath);
        Debug.Log($"Rain prefab saved to: {prefabPath}");
        
        // Clean up the temporary object
        DestroyImmediate(rainObject);
    }
}
#endif