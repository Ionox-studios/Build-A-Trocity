using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class dialogueBox : MonoBehaviour
{
    public GameObject DialogBox;
    public TextMeshProUGUI DialogText;
    public string Dialog;
    public bool playerInRange;
    private GameTimer gameTimer; // Reference to GameTimer component
    public bool isPaused = false; //AL
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    void Start()
    {
        DialogBox.SetActive(false);

                // Find the GameTimer component in the scene
        gameTimer = FindFirstObjectByType<GameTimer>();
        
        if (gameTimer == null)
        {
            Debug.LogWarning("GameTimer component not found in the scene!");
        }
    }


    // Update is called once per frame
    void Update() { 
        if(Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if(DialogBox.activeInHierarchy)
            {
                DialogBox.SetActive(false);
                gameTimer.ResumeTimer(); // Resume the timer when closing dialogue
            }
            else DialogBox.SetActive(true);
            DialogText.text = Dialog;
             gameTimer.PauseTimer(); // Pause the timer when opening dialogue

        }

    }
    private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                playerInRange = true;
                }
        }
    private void OnTriggerExit2D(Collider2D other)
        {if(other.CompareTag("Player"))
         {playerInRange = false;
         DialogBox.SetActive(false);
                     gameTimer.ResumeTimer(); // Make sure to resume timer when player leaves

         }
        }
    }
