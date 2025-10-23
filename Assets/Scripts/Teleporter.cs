using UnityEngine;

public class Teleporter : MonoBehaviour
{
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
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.StartNextStage(Vector3.zero);
                Destroy(gameObject);
            }
        }
    }
}