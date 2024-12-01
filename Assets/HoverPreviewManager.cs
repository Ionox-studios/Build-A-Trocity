using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverPreviewManager : MonoBehaviour
{
    [Header("Preview Panel (Left Side)")]
    [SerializeField] public GameObject previewPanel;
    [SerializeField] public Image previewImage;
    [SerializeField] public TMP_Text itemNameText;
    //[SerializeField] public CanvasGroup previewCanvasGroup;

    [Header("Description Panel (Right Side)")]
    [SerializeField] public GameObject descriptionPanel;
    [SerializeField] public TMP_Text descriptionText;
    //[SerializeField] public CanvasGroup descriptionCanvasGroup;

    [Header("Animation Settings")]
    [SerializeField] public float fadeDuration = 0.2f;
    [SerializeField] public float scaleUpDuration = 0.2f;
    
    [Header("Layout Settings")]
    [SerializeField] public Vector2 previewImageSize = new Vector2(200f, 200f);
    [SerializeField] public int descriptionFontSize = 14;
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    
    public static HoverPreviewManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
                        // Add AudioSource if it doesn't exist
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.volume = 0.1f;
                audioSource.playOnAwake = false;
            }
        else
            Destroy(gameObject);
    }

    public void ShowPreview(ItemSO item)
    {
        if (item == null) return;
        
        previewPanel.SetActive(true);
        previewImage.sprite = item.icon;
        itemNameText.text = item.itemName;
    }

    public void ShowDescription(ItemSO item)
    {
        if (item == null) return;

        descriptionPanel.SetActive(true);
        descriptionText.text = item.description;
    }

    public void HidePreview()
    {
        previewPanel.SetActive(false);
    }

    public void HideDescription()
    {
        descriptionPanel.SetActive(false);
    }
    public void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}