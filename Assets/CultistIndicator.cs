using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CultistIndicator : MonoBehaviour
{
    public ParticleSystem fireEffect;
    public Image image; // Using Image instead of SpriteRenderer for UI
    
    public void Setup()
    {
        //fireEffect = GetComponentInChildren<ParticleSystem>();
        //image = GetComponent<Image>();
    }

    public void PlayDeathAnimation()
    {
        StartCoroutine(ShakeAndDisappear());
    }

    private IEnumerator ShakeAndDisappear()
    {
        RectTransform rectTransform = (RectTransform)transform;
        Vector2 originalPosition = rectTransform.anchoredPosition;
        float shakeDuration = 0.5f;
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            rectTransform.anchoredPosition = originalPosition + Random.insideUnitCircle * 5f; // Shake in UI space
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fireEffect.Stop();
        
        float fadeTime = 0.3f;
        float alpha = 1f;
        Color startColor = image.color;
        
        while (alpha > 0)
        {
            alpha -= Time.deltaTime / fadeTime;
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}