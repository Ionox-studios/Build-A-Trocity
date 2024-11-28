// BuildTransfer.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildTransfer : MonoBehaviour  // Add MonoBehaviour inheritance here
{
    public static BuildTransfer Instance { get; private set; }
    public MonsterData currentMonster;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentMonster = ScriptableObject.CreateInstance<MonsterData>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGameplay()
    {
        SceneManager.LoadScene("monsterSpec"); // Replace with your scene name
    }
}