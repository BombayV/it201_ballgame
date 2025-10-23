using UnityEngine;

public class BoxController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        // Find the single instance of the GameManager in the scene
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Box cannot report collection.");
        }
    }

    // This public method is called by the PlayerController upon collision
    public void CollectBox()
    {
        if (gameManager != null)
        {
            gameManager.BoxCollected();
        }
        // Destroy the box GameObject
        Destroy(gameObject);
    }
}