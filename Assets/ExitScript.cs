using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    [SerializeField] private ConfirmationWindow myConfirmationWindow;
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            OpenConfirmationWindow("Are you sure you want to leave?");}
    }

 
    void Start()
    {

    }
    private void OpenConfirmationWindow(string message)
    {
        myConfirmationWindow.gameObject.SetActive(true);
        myConfirmationWindow.yesButton.onClick.AddListener(YesClicked);
        myConfirmationWindow.noButton.onClick.AddListener(NoClicked);
        myConfirmationWindow.messageText.text = message;
        
    }
    private void YesClicked()
    {
        myConfirmationWindow.gameObject.SetActive(false);
        Debug.Log("Yes Clicked");
        {
            playerStorage.initialValue = playerPosition;
            SceneManager.LoadScene(sceneToLoad);
        }

    }
        private void NoClicked()
    {
        myConfirmationWindow.gameObject.SetActive(false);
        Debug.Log("No Clicked");
        
    }
}
