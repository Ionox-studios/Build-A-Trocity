using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartIndicator : MonoBehaviour
{
    private Image heartImage;
    public ParticleSystem bloodParticles;
    
    public void Setup()
    {
        heartImage = GetComponent<Image>();
        //bloodParticles = GetComponentInChildren<ParticleSystem>();
    }
    
    public void StartBleeding()
    {
        if (bloodParticles != null)
            bloodParticles.Play();
    }
    
    public void StopBleeding()
    {
        if (bloodParticles != null)
            bloodParticles.Stop();
    }
    
    public void UpdateFade(float progress)
    {
        if (heartImage != null)
        {
            Color color = heartImage.color;
            color.a = progress;
            heartImage.color = color;
        }
    }
}