using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class BuildScreenUI : MonoBehaviour
{
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image frankensteinSilhouette;
    [SerializeField] private GameObject buildArea;
    [SerializeField] private Button completeButton;
    
    private void Start()
    {
        BuildManager.Instance.OnMonsterCompleted += OnMonsterCompleted;
        completeButton.onClick.AddListener(CompleteMonster);
    }

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
    }

    private void OnMonsterCompleted(Dictionary<ItemSO.ItemType, ItemSO> monster)
    {
        completeButton.interactable = true;
        ShowDialogue("It's alive! IT'S ALIVE!");
    }

    private void CompleteMonster()
    {
        // Transition to gameplay with assembled monster
        // SceneManager.LoadScene("Gameplay"); Tho make it the game maangaere
    }
}
