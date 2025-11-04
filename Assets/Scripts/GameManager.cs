using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Stage Setup (Set Size in Inspector)")]
    public GameObject[] stageCollectibles;

    public DisappearingWall[] stageWalls;
    public GameObject[] stageTeleporterPrefabs;
    public EnemyController[] stageEnemies;

    public AudioSource backgroundMusicSource;
    public AudioSource collectSound;

    [Header("Transform Points (Set Size in Inspector)")]
    public Transform[] stageTeleporterSpawnPoints;

    public Transform[] stagePlayerStartPoints;

    [Header("Fall Detection")] public float fallThreshold = -10f;
    private float fallCheckCooldown = 0.5f;
    private float lastFallCheckTime = 0f;

    [Header("Game State")] private int currentStageIndex = 0;
    private int totalBoxesInStage = 0;
    private int boxesCollected = 0;
    public int timeLeft = 300;
    public int totalLives = 3;

    public TextMeshProUGUI countText;
    public TextMeshProUGUI winLoseText;
    public TextMeshProUGUI pauseText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI livesText;

    private bool paused = false;
    private bool gameOver = false;

    void Start()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.loop = true;
            backgroundMusicSource.volume = 0.1f;
            backgroundMusicSource.Play();
        }

        if (collectSound != null)
        {
            collectSound.volume = 0.5f;
        }

        if (countDownText != null)
        {
            countDownText.text = $"Time Left: {timeLeft}s";
            InvokeRepeating(nameof(UpdateCountdown), 1f, 1f);
        }

        if (livesText != null)
        {
            livesText.text = $"Lives: {totalLives}";
        }

        InitializeStage(0);
    }

    private void UpdateCountdown()
    {
        if (!paused)
        {
            timeLeft--;
            if (countDownText != null)
            {
                countDownText.text = $"Time Left: {timeLeft}s";
            }

            if (timeLeft <= 0)
            {
                totalLives = 0;
                CancelInvoke(nameof(UpdateCountdown));
                PlayerCaught();
            }
        }

        if (gameOver)
        {
            CancelInvoke(nameof(UpdateCountdown));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Time.timeScale = 1f;
                pauseText.text = "Escape - Pause Game";
                paused = false;
                backgroundMusicSource.UnPause();
            }
            else
            {
                Time.timeScale = 0f;
                pauseText.text = "Escape - Resume Game";
                paused = true;
                backgroundMusicSource.Pause();
            }
        }
        else if (gameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

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
        if (player != null && currentStageIndex < stagePlayerStartPoints.Length &&
            stagePlayerStartPoints[currentStageIndex] != null)
        {
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
        }

        boxesCollected = 0;

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

        UpdateCountText();

        if (collectSound != null)
        {
            collectSound.Play();
        }

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
            if (currentStageIndex < stageTeleporterSpawnPoints.Length &&
                stageTeleporterSpawnPoints[currentStageIndex] != null)
            {
                Debug.Log($"Spawning teleporter for stage {currentStageIndex + 1}");
                Instantiate(stageTeleporterPrefabs[currentStageIndex],
                    stageTeleporterSpawnPoints[currentStageIndex].position,
                    Quaternion.identity);
            }
            else
            {
                Debug.LogWarning(
                    $"Teleporter prefab exists for stage {currentStageIndex + 1}, but spawn point is missing!");
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
            gameOver = true;
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
        if (!gameOver && totalLives > 0)
        {
            totalLives--;
            if (livesText != null)
            {
                livesText.text = $"Lives: {totalLives}";
            }

            Debug.Log($"Player caught! Lives remaining: {totalLives}");
            RespawnPlayer();
            return;
        }

        gameOver = true;
        foreach (var enemy in stageEnemies)
        {
            if (enemy != null)
                enemy.Deactivate();
        }

        ShowLoseScreen();
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }

        Destroy(GameObject.FindGameObjectWithTag("Player"));
    }

    private void ShowLoseScreen()
    {
        if (winLoseText != null)
        {
            winLoseText.text = "You lost!";
            winLoseText.gameObject.SetActive(true);
        }
    }

    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}