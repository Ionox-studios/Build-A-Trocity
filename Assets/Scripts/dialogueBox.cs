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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  
    void Start()
    {
        DialogBox.SetActive(false);
    }

    // Update is called once per frame
    void Update() { 
        if(Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if(DialogBox.activeInHierarchy)
            {
                DialogBox.SetActive(false);
            }
            else DialogBox.SetActive(true);
            DialogText.text = Dialog;
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
         }
        }
    }
