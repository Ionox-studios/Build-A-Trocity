using UnityEngine;

public class NoiseTextureGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    
    void Start()
    {
        Texture2D noiseTexture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                
                // Get multiple octaves of noise and combine them
                float noise = 0f;
                float amplitude = 1f;
                float frequency = 1f;
                
                for(int i = 0; i < 4; i++)
                {
                    noise += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
                    amplitude *= 0.5f;
                    frequency *= 2f;
                }
                
                // Normalize to 0-1 range
                noise = Mathf.Clamp01(noise);
                
                Color color = new Color(noise, noise, noise, 1);
                noiseTexture.SetPixel(x, y, color);
            }
        }
        
        noiseTexture.Apply();
        
        // Save texture with import settings
        byte[] bytes = noiseTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/NoiseTexture.png", bytes);
        
        Debug.Log("Noise texture saved. Remember to set texture type to 'Default' and disable sRGB (Color Texture) in import settings!");
    }
}