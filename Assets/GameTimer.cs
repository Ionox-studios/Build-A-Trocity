using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float totalGameMinutes = 5f; // Real-world minutes to complete the cycle
    public int startHour = 19;
    public int endHour = 5;

    private float currentGameTime;
    private float totalGameSeconds;
    private float gameSpeedMultiplier;
    private float inGameSeconds;

    [Header("Optional Visual Elements")]
    public UnityEngine.UI.Image skyboxImage;  // Reference to background image that can change with time
    public Color dayColor = Color.white;
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);
    
    [Header("Time Events")]
    public bool isPaused = false;
    public delegate void TimeOfDayHandler(int hour);
    public static event TimeOfDayHandler OnHourChanged;
    private int lastHour = -1;
    public string timeUpSceneName = "BuildABear";

    void Awake()
{

        totalGameSeconds = totalGameMinutes * 60; // Convert minutes to seconds
        
        // Calculate total in-game seconds we need to simulate
        int totalHours = (endHour - startHour + 24) % 24;
        float totalInGameSeconds = totalHours * 3600; // Convert hours to seconds
        
        // Calculate how fast time should move
        gameSpeedMultiplier = totalInGameSeconds / totalGameSeconds;
        
        // Load saved time from GameManager
        if (GameManager.Instance != null)
        {
            Debug.Log("Loading saved time from GameManager");
            currentGameTime = GameManager.Instance.gameTime;
            inGameSeconds = startHour * 3600 + currentGameTime * gameSpeedMultiplier;
        }
        else
        {
            inGameSeconds = startHour * 3600; // Start at the beginning hour in seconds
        }


}   
    void Start()
    {


        UpdateTimerDisplay();
        UpdateVisuals();
    }

    void Update()
    {
        if (!isPaused && currentGameTime < totalGameSeconds)
        {
            currentGameTime += Time.deltaTime;
            inGameSeconds += Time.deltaTime * gameSpeedMultiplier;
            
            // Save time to GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.gameTime = currentGameTime;
            }
            
            UpdateTimerDisplay();
            UpdateVisuals();
            CheckHourChange();
            if (currentGameTime >= totalGameSeconds)
            {
                // Game over
                Debug.Log("Game Over!");
                OnTimeUp();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        // Convert total seconds to hour, minute, second
        float totalHours = inGameSeconds / 3600f;
        int currentHour = (int)totalHours % 24;
        int currentMinute = (int)((totalHours % 1) * 60);
        int currentSecond = (int)((((totalHours % 1) * 60) % 1) * 60);

        // Convert to 12-hour format
        string period = currentHour >= 12 ? "PM" : "AM";
        int displayHour = currentHour % 12;
        if (displayHour == 0) displayHour = 12;
        string timeString = $"{displayHour}:{currentMinute:00} {period}"; 
 //       string timeString = $"{displayHour}:{currentMinute:00}:{currentSecond:00} {period}"; 
        if (timerText != null)
        {
            timerText.text = timeString;
        }
    }

    void UpdateVisuals()
    {
        if (skyboxImage != null)
        {
            float totalHours = inGameSeconds / 3600f;
            int currentHour = (int)totalHours % 24;
            
            // Simple day/night cycle
            float t = 0f;
            if (currentHour >= 6 && currentHour < 18) // Daytime
            {
                t = 1f;
            }
            else if (currentHour >= 18 || currentHour < 6) // Nighttime
            {
                t = 0f;
            }
            
            skyboxImage.color = Color.Lerp(nightColor, dayColor, t);
        }
    }

    void CheckHourChange()
    {
        int currentHour = (int)(inGameSeconds / 3600f) % 24;
        if (currentHour != lastHour)
        {
            lastHour = currentHour;
            OnHourChanged?.Invoke(currentHour);
        }
    }

    public void PauseTimer()
    {
        isPaused = true;
    }

    public void ResumeTimer()
    {
        isPaused = false;
    }

    public void ResetTimer()
    {
        currentGameTime = 0;
        inGameSeconds = startHour * 3600;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameTime = currentGameTime;
        }
        UpdateTimerDisplay();
        UpdateVisuals();
    }

    public int GetCurrentHour()
    {
        return (int)(inGameSeconds / 3600f) % 24;
    }

    public bool IsNighttime()
    {
        int currentHour = GetCurrentHour();
        return currentHour >= 18 || currentHour < 6;
    }

    public float GetTimeProgress()
    {
        return currentGameTime / totalGameSeconds;
    }

    void OnDestroy()
    {
        // Save final time state when object is destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameTime = currentGameTime;
Debug.Log($"Saving time state from instance {gameObject.GetInstanceID()}");        }
    }
    private void OnTimeUp()
    {
        if (!string.IsNullOrEmpty(timeUpSceneName))
        {
            SceneManager.LoadScene(timeUpSceneName);
        }
    }
}