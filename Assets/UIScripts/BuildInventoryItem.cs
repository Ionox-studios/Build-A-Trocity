using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private ItemSO item;
    public ItemSO Item => item;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    // Change this from private to public
    public Transform originalParent { get; private set; }
    private Canvas mainCanvas;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCanvas = GetComponentInParent<Canvas>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform.sizeDelta = new Vector2(100f, 100f);
    }

    public void Initialize(ItemSO newItem)
    {
        item = newItem;
        if (itemImage != null && item != null)
        {
            itemImage.sprite = item.icon;
            itemImage.preserveAspect = true; //was false
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        Vector2 currentPosition = rectTransform.position;
        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling();
        rectTransform.position = currentPosition;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if we're over a BuildSpot
        BuildSpot buildSpot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<BuildSpot>();
        if (buildSpot != null && buildSpot.CanAcceptItem(item))
        {
            buildSpot.PlaceItem(this);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }

public void OnPointerEnter(PointerEventData eventData)
{
    HoverPreviewManager.Instance.ShowPreview(item);  // Remove the position parameter
    HoverPreviewManager.Instance.ShowDescription(item);
    HoverPreviewManager.Instance.PlayHoverSound();

}

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverPreviewManager.Instance.HidePreview();
        HoverPreviewManager.Instance.HideDescription();
    }
}