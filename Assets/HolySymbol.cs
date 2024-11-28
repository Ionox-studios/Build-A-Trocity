using UnityEngine;

public class HolySymbol : MonoBehaviour
{
    [SerializeField] private Texture2D noiseTexture;
    [SerializeField] private Color tintColor = Color.red;
    
    private SpriteRenderer spriteRenderer;
    private Material material;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = new Material(Shader.Find("Custom/DissolveSprite"));
        
        // Debug noise texture
        if (noiseTexture == null)
        {
            Debug.LogError("No noise texture assigned!");
            return;
        }
        
        Debug.Log($"Noise texture size: {noiseTexture.width}x{noiseTexture.height}");
        Debug.Log($"Noise texture format: {noiseTexture.format}");
        
        material.SetTexture("_NoiseTex", noiseTexture);
        material.SetColor("_Color", tintColor);
        spriteRenderer.material = material;
        
        // Start fully visible
        UpdateDecay(0);
    }

    public void UpdateDecay(float progress)
    {
        if (material != null)
        {
            Debug.Log($"Setting threshold to: {progress}");
            material.SetFloat("_Threshold", progress);
        }
    }

    void OnDestroy()
    {
        if (material != null)
            Destroy(material);
    }
}