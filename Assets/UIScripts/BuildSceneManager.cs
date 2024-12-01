// BuildSceneManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button completeButton;
    [SerializeField] private TextMeshProUGUI statsText;

    // Assign BuildTransfer in the inspector
    [SerializeField] private BuildTransfer buildTransfer;
    public AudioSource audioSource;
    public AudioClip completeSound;

    private void Start()
    {
        SetupCanvas();
        SetupUICallbacks();
        UpdateStats(); // Initial update
    }

    private void SetupCanvas()
    {
        CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;
    }

    private void SetupUICallbacks()
    {
        completeButton.onClick.AddListener(OnCompleteButtonClicked);
        BuildManager.Instance.OnPartChanged += HandlePartChanged;
    }

    private void OnDestroy()
    {
        completeButton.onClick.RemoveListener(OnCompleteButtonClicked);
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.OnPartChanged -= HandlePartChanged;
        }
    }

    private void OnCompleteButtonClicked()
    {
        audioSource.PlayOneShot(completeSound);
        buildTransfer.CollectPartsFromBuildSpots();
        buildTransfer.StartGameplay();
    }

    private void HandlePartChanged()
    {
        UpdateStats();

        if (BuildManager.Instance.AreAllSpotsFilled())
        {
            dialogueText.text = "AHAHAHAA!! Trocity is ALIVE!!!! LET'S GET OUR REVENGE.";
        }
        else
        {
            dialogueText.text = "If you stop now, I'll have to use your parts."; // Or any default message
        }
    }

private void UpdateStats()
{
    float totalSpeed = 0f;
    float totalDamage = 0f;
    int health = 0;
    float speedMultiplier = 1f;

    // Get all items from BuildSpots
    var leftArmItem = buildTransfer.leftArmSpot.GetCurrentItem();
    var rightArmItem = buildTransfer.rightArmSpot.GetCurrentItem();
    var leftLegItem = buildTransfer.leftLegSpot.GetCurrentItem();
    var rightLegItem = buildTransfer.rightLegSpot.GetCurrentItem();
    var headItem = buildTransfer.headSpot.GetCurrentItem();
    var torsoItem = buildTransfer.torsoSpot.GetCurrentItem();

    // Calculate base speed from legs
    if (leftLegItem != null)
        totalSpeed += leftLegItem.speed;
    if (rightLegItem != null)
        totalSpeed += rightLegItem.speed;

    // Apply torso speed multiplier
    if (torsoItem != null)
    {
        speedMultiplier *= torsoItem.torsoSpeed;
        health = torsoItem.torsoHealth;
    }

    // Apply head speed boost if applicable
    if (headItem != null && headItem.headEffectType == ItemSO.HeadEffectType.SpeedIncrease)
    {
        speedMultiplier *= 1.5f;
    }

    // Calculate total speed with multipliers
    totalSpeed *= speedMultiplier;

    // Calculate damage
    if (leftArmItem != null)
        totalDamage += leftArmItem.damage;
    if (rightArmItem != null)
        totalDamage += rightArmItem.damage;
    if (leftLegItem != null)
        totalDamage += leftLegItem.damage;
    if (rightLegItem != null)
        totalDamage += rightLegItem.damage;

    // Update stats text with all three values
    statsText.text = $"Speed: {totalSpeed:F1}\nDamage: {totalDamage:F1}\nHealth: {health}";

}
}