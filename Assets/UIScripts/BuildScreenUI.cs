// BuildScreenUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // If you're using TextMesh Pro
using System.Collections.Generic;

public class BuildScreenUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText; // Use TextMeshProUGUI if using TextMesh Pro
    [SerializeField] private Image frankensteinSilhouette;
    [SerializeField] private GameObject buildArea;
    [SerializeField] private Button completeButton;

    // Reference to BuildTransfer
    [SerializeField] private BuildTransfer buildTransfer;

    private void Start()
    {
        // Subscribe to the OnPartChanged event
        BuildManager.Instance.OnPartChanged += OnPartChanged;
        completeButton.onClick.AddListener(CompleteMonster);
        completeButton.interactable = true;  // Ensure the button is always interactable
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (BuildManager.Instance != null)
        {
            BuildManager.Instance.OnPartChanged -= OnPartChanged;
        }
        completeButton.onClick.RemoveListener(CompleteMonster);
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
    }

    private void OnPartChanged()
    {
        if (BuildManager.Instance.AreAllSpotsFilled())
        {
            ShowDialogue("Your creation is complete! Ready to unleash it upon the world?");
        }
        else
        {
            ShowDialogue("If you stop now I'll have to use your parts. ");  // Or any default message you'd like
        }
    }

    private void CompleteMonster()
    {
        // Collect parts from BuildSpots and start gameplay
        buildTransfer.CollectPartsFromBuildSpots();
        buildTransfer.StartGameplay();
    }
}
