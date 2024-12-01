using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class ScoreManagerNight : MonoBehaviour
{
    public static ScoreManagerNight Instance { get; private set; }
    
    [SerializeField] private int totalCultistsNeeded = 20;
    [SerializeField] private HolySymbol holySymbol;
    [SerializeField] private Light2D scoreLight;
    [SerializeField] private float startingIntensity = 1f;
    [SerializeField] private float minIntensity = 0.2f;
    
    [Header("Cultist Indicators")]
    [SerializeField] private GameObject cultistIndicatorPrefab;
    [SerializeField] private HorizontalLayoutGroup layoutGroup; // Reference this instead of uiPanel

    [SerializeField] private float spacingBetweenIndicators = 1f;
    [Header("Victory Settings")]
    [SerializeField] private string endSceneName = "IgorGood"; // Scene to load
    [SerializeField] private string badEndSceneName = "IgorGoodButBad"; // Scene to load

    [SerializeField] private string perFectEnding = "IgorPerfect"; // Scene to load
    public GameObject victoryTextObject; // Reference to existing UI text object
    public float delayBeforeSceneChange = 2f; // Delay in seconds
    private int cultistsDefeated = 0;
    private CultistIndicator[] cultistIndicators; 
    public MonsterControllerSimple monsterControllerSimple;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        scoreLight.intensity = startingIntensity;
        SetupCultistIndicators();
        if (victoryTextObject != null)
        {
            victoryTextObject.SetActive(false);
        }
    }

    private void SetupCultistIndicators()
    {
        cultistIndicators = new CultistIndicator[totalCultistsNeeded];
        
        for (int i = 0; i < totalCultistsNeeded; i++)
        {
            GameObject indicator = Instantiate(cultistIndicatorPrefab, layoutGroup.transform);
            cultistIndicators[i] = indicator.GetComponent<CultistIndicator>();
            cultistIndicators[i].Setup();
        }
    }

    public void CultistDefeated()
    {
        if (cultistsDefeated < totalCultistsNeeded)
        {
            cultistIndicators[cultistsDefeated].PlayDeathAnimation();
        }
        
        cultistsDefeated++;
        float progress = (float)cultistsDefeated / totalCultistsNeeded;
        
        // Update holy symbol decay
        holySymbol.UpdateDecay(progress);
        
        // Dim the score light
        scoreLight.intensity = Mathf.Lerp(startingIntensity, minIntensity, progress);

        // Check for victory condition
        if (cultistsDefeated >= totalCultistsNeeded)
        {
            StartCoroutine(HandleVictory());
        }
            HeadController headController = FindFirstObjectByType<HeadController>();
    if (headController != null && headController.headItem != null && 
        headController.headItem.headEffectType == ItemSO.HeadEffectType.Vampire)
    {
        HeartTimer heartTimer = FindFirstObjectByType<HeartTimer>();
        if (heartTimer != null)
        {
            Debug.Log("Vampire active, restoring time");
            heartTimer.RestoreTime(10f);
        }
    }
    }

    private IEnumerator HandleVictory()
    {
        // Show victory text
        if (victoryTextObject != null)
        {
            victoryTextObject.SetActive(true);
        }

        // Wait for specified delay
        yield return new WaitForSeconds(delayBeforeSceneChange);

        if (monsterControllerSimple.isBadEnding)
        {
            SceneManager.LoadScene(badEndSceneName);
        }
        else if (monsterControllerSimple.isGreatEnding)
        {
            SceneManager.LoadScene(perFectEnding);
        }
        else
        {
            SceneManager.LoadScene(endSceneName);
        }
        // Load the end scene
        
    }

    public int GetCultistsRemaining()
    {
        return totalCultistsNeeded - cultistsDefeated;
    }
}