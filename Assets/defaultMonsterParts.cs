using UnityEngine;

public class DefaultMonsterParts : MonoBehaviour
{
    public static DefaultMonsterParts Instance { get; private set; }
    
    public ItemSO defaultLeftArm;
    public ItemSO defaultRightArm;
    public ItemSO defaultLeftLeg;
    public ItemSO defaultRightLeg;
    public ItemSO defaultHead;
    public ItemSO defaultTorso;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}