using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Stage Setup (Set Size in Inspector)")]
    // Parent GameObject holding all boxes for Stage 1, Stage 2, etc.
    public GameObject[] stageCollectibles;
    // The wall that disappears for Stage 1, Stage 2, etc. (Can be null if stage has no wall/door)
    public DisappearingWall[] stageWalls;
    // The Teleporter Prefab to spawn for Stage 1, Stage 2, etc. (Can be null if stage has no teleporter)
    public GameObject[] stageTeleporterPrefabs;
    // The Enemy for Stage 1, Stage 2, etc. (Can be null if stage has no enemy)
    public EnemyController[] stageEnemies;

    [Header("Transform Points (Set Size in Inspector)")]
    // Where to spawn the teleporter for Stage 1, Stage 2, etc.
    public Transform[] stageTeleporterSpawnPoints;
    // Where the player starts for Stage 1, Stage 2, etc.
    public Transform[] stagePlayerStartPoints;

    [Header("Fall Detection")]
    // Y-axis threshold: if player falls below this, they respawn
    public float fallThreshold = -10f;
    // Delay between fall detections to prevent repeated triggers
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
        // Initialize the first stage
        InitializeStage(0);
    }

    void Update()
    {
        // Check if the player has fallen off the stage
        CheckPlayerFall();
    }

    private void CheckPlayerFall()
    {
        // Cooldown to prevent multiple triggers
        if (Time.time - lastFallCheckTime < fallCheckCooldown)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        // If player falls below threshold, respawn them
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

            // Teleport the player back to the current stage spawn point
            player.transform.position = stagePlayerStartPoints[currentStageIndex].position;

            // Reset velocity
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
        // --- Reset all level objects ---
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

        // --- Activate objects for the CURRENT stage ---
        currentStageIndex = stageIndex;
        
        // Activate the collectible parent object
        if (stageIndex < stageCollectibles.Length && stageCollectibles[stageIndex] != null)
        {
            stageCollectibles[stageIndex].SetActive(true);
            // Count how many boxes are children of this object
            totalBoxesInStage = stageCollectibles[stageIndex].transform.childCount;
        }

        // Activate the enemy for this stage (if it exists)
        if (stageIndex < stageEnemies.Length && stageEnemies[stageIndex] != null)
        {
            stageEnemies[stageIndex].Reactivate();
            Debug.Log($"Enemy for stage {stageIndex + 1} has been activated!");
        }

        boxesCollected = 0; // Reset box count for the new stage

        Debug.Log($"--- STAGE {stageIndex + 1} STARTED ---. Boxes to collect: {totalBoxesInStage}");
        
        // Update the count text display
        UpdateCountText();
    }

    private void UpdateCountText()
    {
        if (countText != null)
        {
            countText.text = $"Collected in Stage: {boxesCollected}/{totalBoxesInStage}";
        }
    }

    // Called by BoxController
    public void BoxCollected()
    {
        boxesCollected++;
        Debug.Log($"Box Collected! Progress: {boxesCollected} / {totalBoxesInStage}");
        
        // Update the count text display
        UpdateCountText();

        // Check if stage is complete
        if (boxesCollected >= totalBoxesInStage && totalBoxesInStage > 0)
        {
            HandleStageComplete();
            totalBoxesInStage = 0; // Prevents this from running multiple times
        }
    }

    private void HandleStageComplete()
    {
        Debug.Log($"Stage {currentStageIndex + 1} Complete!");

        // 1. Deactivate the enemy for this stage (if it exists)
        if (currentStageIndex < stageEnemies.Length && stageEnemies[currentStageIndex] != null)
        {
            stageEnemies[currentStageIndex].Deactivate();
        }

        // 2. Tell the stage wall to vanish (if this stage has a wall)
        if (currentStageIndex < stageWalls.Length && stageWalls[currentStageIndex] != null)
        {
            Debug.Log($"Disabling wall for stage {currentStageIndex + 1}");
            stageWalls[currentStageIndex].Vanish();
        }
        else if (currentStageIndex < stageWalls.Length)
        {
            Debug.Log($"Stage {currentStageIndex + 1} has no wall/door");
        }

        // 3. Spawn the teleporter (if this stage has one)
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

    // Called by the Teleporter
    public void StartNextStage(Vector3 nextPlayerPosition)
    {
        int nextStageIndex = currentStageIndex + 1;

        if (nextStageIndex < stageCollectibles.Length)
        {
            // Move the player to the next starting point
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Teleport the player
                player.transform.position = stagePlayerStartPoints[nextStageIndex].position;
                
                // Reset the player's velocity
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
                }
            }
            
            // Set up the new stage
            InitializeStage(nextStageIndex);
        }
        else
        {
            // Game is finished!
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

    // Called by EnemyController when the player is caught
    public void PlayerCaught()
    {
        Debug.Log("GAME OVER! The player was caught by the enemy!");
        
        // Deactivate all enemies to stop them from chasing
        foreach (var enemy in stageEnemies)
        {
            if (enemy != null)
                enemy.Deactivate();
        }

        // Show the lose screen
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