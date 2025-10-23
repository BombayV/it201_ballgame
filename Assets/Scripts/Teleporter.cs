using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Set this in the Inspector: where should the player appear in the next stage?
    public Transform nextStageSpawnPoint; 

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found! Teleporter cannot function.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the Player
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                // Tell the GameManager to start the next stage.
                // We let the GameManager find the spawn point from its arrays.
                gameManager.StartNextStage(Vector3.zero); // Position is now handled by GameManager

                // Destroy this teleporter
                Destroy(gameObject);
            }
        }
    }
}