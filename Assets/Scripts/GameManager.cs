using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Stage Setup (Set Size in Inspector)")]
    public GameObject[] stageCollectibles;
    public DisappearingWall[] stageWalls;
    public GameObject[] stageTeleporterPrefabs;
    public EnemyController[] stageEnemies;

    [Header("Transform Points (Set Size in Inspector)")]
    public Transform[] stageTeleporterSpawnPoints;
    public Transform[] stagePlayerStartPoints;

    [Header("Fall Detection")]
    public float fallThreshold = -10f;
    private float fallCheckCooldown = 0.5f;
    private float lastFallCheckTime = 0f;

    [Header("Game State")]
    private int currentStageIndex = 0;
    private int totalBoxesInStage = 0;
    private int boxesCollected = 0;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI winLoseText;

    void Start()
    {
        InitializeStage(0);
    }

    void Update()
    {
        CheckPlayerFall();
    }

    private void CheckPlayerFall()
    {
        if (Time.time - lastFallCheckTime < fallCheckCooldown)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        if (player.transform.position.y < fallThreshold)
        {
            lastFallCheckTime = Time.time;
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && currentStageIndex < stagePlayerStartPoints.Length && stagePlayerStartPoints[currentStageIndex] != null)
        {
            Debug.Log($"Player fell! Respawning at stage {currentStageIndex + 1} spawn point.");
            player.transform.position = stagePlayerStartPoints[currentStageIndex].position;

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }
        }
    }

    void InitializeStage(int stageIndex)
    {
        foreach (var collectibles in stageCollectibles)
        {
            if (collectibles != null) collectibles.SetActive(false);
        }
        foreach (var wall in stageWalls)
        {
            if (wall != null) wall.ResetWall();
        }
        foreach (var enemy in stageEnemies)
        {
            if (enemy != null) enemy.Deactivate();
        }

        currentStageIndex = stageIndex;
        
        if (stageIndex < stageCollectibles.Length && stageCollectibles[stageIndex] != null)
        {
            stageCollectibles[stageIndex].SetActive(true);
            totalBoxesInStage = stageCollectibles[stageIndex].transform.childCount;
        }

        if (stageIndex < stageEnemies.Length && stageEnemies[stageIndex] != null)
        {
            stageEnemies[stageIndex].Reactivate();
            Debug.Log($"Enemy for stage {stageIndex + 1} has been activated!");
        }

        boxesCollected = 0;

        Debug.Log($"--- STAGE {stageIndex + 1} STARTED ---. Boxes to collect: {totalBoxesInStage}");
        
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        if (countText != null)
        {
            countText.text = $"Collected in Stage: {boxesCollected}/{totalBoxesInStage}";
        }
    }

    public void BoxCollected()
    {
        boxesCollected++;
        Debug.Log($"Box Collected! Progress: {boxesCollected} / {totalBoxesInStage}");
        
        UpdateCountText();

        if (boxesCollected >= totalBoxesInStage && totalBoxesInStage > 0)
        {
            HandleStageComplete();
            totalBoxesInStage = 0;
        }
    }

    private void HandleStageComplete()
    {
        Debug.Log($"Stage {currentStageIndex + 1} Complete!");

        if (currentStageIndex < stageEnemies.Length && stageEnemies[currentStageIndex] != null)
        {
            stageEnemies[currentStageIndex].Deactivate();
        }

        if (currentStageIndex < stageWalls.Length && stageWalls[currentStageIndex] != null)
        {
            Debug.Log($"Disabling wall for stage {currentStageIndex + 1}");
            stageWalls[currentStageIndex].Vanish();
        }
        else if (currentStageIndex < stageWalls.Length)
        {
            Debug.Log($"Stage {currentStageIndex + 1} has no wall/door");
        }

        if (currentStageIndex < stageTeleporterPrefabs.Length && stageTeleporterPrefabs[currentStageIndex] != null)
        {
            if (currentStageIndex < stageTeleporterSpawnPoints.Length && stageTeleporterSpawnPoints[currentStageIndex] != null)
            {
                Debug.Log($"Spawning teleporter for stage {currentStageIndex + 1}");
                Instantiate(stageTeleporterPrefabs[currentStageIndex], 
                            stageTeleporterSpawnPoints[currentStageIndex].position, 
                            Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"Teleporter prefab exists for stage {currentStageIndex + 1}, but spawn point is missing!");
            }
        }
        else if (currentStageIndex < stageTeleporterPrefabs.Length)
        {
            Debug.Log($"Stage {currentStageIndex + 1} has no teleporter");
        }
    }

    public void StartNextStage(Vector3 nextPlayerPosition)
    {
        int nextStageIndex = currentStageIndex + 1;

        if (nextStageIndex < stageCollectibles.Length)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = stagePlayerStartPoints[nextStageIndex].position;
                
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
                }
            }
            
            InitializeStage(nextStageIndex);
        }
        else
        {
            Debug.Log("Congratulations! You have completed all stages!");
            ShowWinScreen();
        }
    }

    private void ShowWinScreen()
    {
        if (winLoseText != null)
        {
            winLoseText.text = "Winner!";
            winLoseText.gameObject.SetActive(true);
        }
    }

    public void PlayerCaught()
    {
        Debug.Log("GAME OVER! The player was caught by the enemy!");
        
        foreach (var enemy in stageEnemies)
        {
            if (enemy != null)
                enemy.Deactivate();
        }

        ShowLoseScreen();
    }

    private void ShowLoseScreen()
    {
        if (winLoseText != null)
        {
            winLoseText.text = "You lost!";
            winLoseText.gameObject.SetActive(true);
        }
    }
}