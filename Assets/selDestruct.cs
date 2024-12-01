using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifetime =1f;


    void Start()
    {
        Destroy(gameObject, lifetime);
    }


}