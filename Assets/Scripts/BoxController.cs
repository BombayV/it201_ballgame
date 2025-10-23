using UnityEngine;

public class BoxController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Box cannot report collection.");
        }
    }

    public void CollectBox()
    {
        if (gameManager != null)
        {
            gameManager.BoxCollected();
        }
        Destroy(gameObject);
    }
}