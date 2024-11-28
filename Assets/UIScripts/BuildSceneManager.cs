// BuildSceneManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BuildSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button completeButton;
    [SerializeField] private TextMeshProUGUI statsText;
    
    [Header("Panels")]
    [SerializeField] private RectTransform buildAreaPanel;
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;
    [SerializeField] private RectTransform inventoryPanel;

    private void Start()
    {
        SetupCanvas();
        SetupPanels();
        SetupUICallbacks();
    }

    private void SetupCanvas()
    {
        CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;
    }

    private void SetupPanels()
    {
//        SetupPanel(leftPanel, new Vector2(0, 0), new Vector2(0.3f, 1));
//        SetupPanel(rightPanel, new Vector2(0.7f, 0), new Vector2(1, 1));
//        SetupPanel(buildAreaPanel, new Vector2(0.3f, 0.2f), new Vector2(0.7f, 1));
        //SetupPanel(inventoryPanel, new Vector2(0, 0), new Vector2(1, 0.2f));
    }

    private void SetupPanel(RectTransform panel, Vector2 anchorMin, Vector2 anchorMax)
    {
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;
    }

    private void SetupUICallbacks()
    {
        completeButton.onClick.AddListener(OnCompleteButtonClicked);
        BuildManager.Instance.OnMonsterCompleted += HandleMonsterCompleted;
        completeButton.interactable = false;
    }

    private void OnDestroy()
    {
        completeButton.onClick.RemoveListener(OnCompleteButtonClicked);
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.OnMonsterCompleted -= HandleMonsterCompleted;
        }
    }

    private void OnCompleteButtonClicked()
    {
        if (BuildManager.Instance.ValidateMonster())
        {
            StartCoroutine(TransitionToGameplay());
        }
    }

 private void HandleMonsterCompleted(Dictionary<ItemSO.ItemType, ItemSO> monster)
{
    completeButton.interactable = true;
    UpdateStats(monster);
    BuildTransfer.Instance.currentMonster.UpdateFromDictionary(monster);
    dialogueText.text = "Your creation is complete! Ready to unleash it upon the world?";
}

    private void UpdateStats(Dictionary<ItemSO.ItemType, ItemSO> monster)
    {
        float totalSpeed = 0f;
        float totalDamage = 0f;

        foreach (var part in monster.Values)
        {
            switch (part.itemType)
            {
                case ItemSO.ItemType.Leg:
                    totalSpeed += part.speed;
                    break;
                case ItemSO.ItemType.Arm:
                    totalDamage += part.damage;
                    break;
            }
        }

        statsText.text = $"Speed: {totalSpeed:F1}\nDamage: {totalDamage:F1}";
    }

private IEnumerator TransitionToGameplay()
{
    dialogueText.text = "IT'S ALIVE! IT'S ALIVE!";
    yield return new WaitForSeconds(2f);
    BuildTransfer.Instance.StartGameplay();
}
}