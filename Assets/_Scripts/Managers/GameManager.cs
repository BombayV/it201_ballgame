using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: Singleton<GameManager>
{

    [Header("Game State")]
    public bool IsGamePaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public int _timeLeft = 300;
    
    [SerializeField] private int MAX_LEVELS = 1;
    [SerializeField] private int _currentLevel = 0;

    private GameObject[] sceneCollectables;
    private int totalCollected = 0;

    [Header("Fall Detection")]
    public float fallThreshold = -10f;
    private float fallCheckCooldown = 0.5f;
    private float lastFallCheckTime = 0f;
    
    [Header("Prefabs")]
    public GameObject teleporterPrefab;
    

    protected override void Awake()
    {
        base.Awake();
        IsGamePaused = false;
        IsGameOver = false;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded -= OnRestartSceneLoaded;
        CancelInvoke("UpdateCountdownTimer");
    }
    
    // Game Management
    public void UpdateCountdownTimer()
    {
        if (IsGameOver || IsGamePaused) return;

        if (_timeLeft > 0)
        {
            _timeLeft--;
            CanvasSystem.Instance.UpdateTimer(_timeLeft);
        }
        else
        {
            GameOver();
            CancelInvoke("UpdateCountdownTimer");
        }
    }
    
    public void GameOver(bool won = false)
    {
        IsGameOver = true;
        if (won)
        {
            CanvasSystem.Instance.UpdateEndGameMessage("You Win!");
        }
        else
        {
            CanvasSystem.Instance.UpdateEndGameMessage("You Lost!");
        }
        CanvasSystem.Instance.SetEndGameScreen(true);
        AudioSystem.Instance.StopMusic();
    }
    
    public void SetPause(bool state)
    {
        IsGamePaused = state;
        CanvasSystem.Instance.SetPauseMenu(state);
        if (state)
        {
            AudioSystem.Instance.PauseMusic();
        } else
        {
            AudioSystem.Instance.ResumeMusic();
        }

        Time.timeScale = state ? 0f : 1f;
    }

    public void RestartGame()
    {
        CancelInvoke("UpdateCountdownTimer");
        Time.timeScale = 1f;
        IsGameOver = false;
        IsGamePaused = false;
        _currentLevel = 0;
        _timeLeft = 300;
        PlayerController.Instance.ResetLives();
        SceneManager.sceneLoaded += OnRestartSceneLoaded;
        SceneManager.LoadScene("MainMenu");
    }
    
    private void OnRestartSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnRestartSceneLoaded;
        CanvasSystem.Instance.SetMainMenu(true);
    }
    
    // Player/Box Management
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOver && _currentLevel > 0 && _currentLevel <= MAX_LEVELS)
        {
            SetPause(!IsGamePaused);
        }

        CheckPlayerFall();
    }

    
    public void BoxCollected()
    {
        totalCollected++;
        CanvasSystem.Instance.UpdateCollectibleCount(totalCollected, sceneCollectables.Length);
        AudioSystem.Instance.PlaySound();
        
        if (totalCollected >= sceneCollectables.Length)
        {
            GameObject tpLoc = GameObject.FindGameObjectWithTag("TeleporterSpawn");
            if (tpLoc != null && teleporterPrefab != null)
            {
                Instantiate(teleporterPrefab, tpLoc.transform.position, tpLoc.transform.rotation);
            }
        }
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
            PlayerController.Instance.ResetPositionToSpawn();
        }
    }
    
    public void PlayerCaught()
    {
        if (IsGameOver) return;

        PlayerController.Instance.LoseLife();
        if (PlayerController.Instance.GetLives() <= 0)
        {
            GameOver();
        }
        else
        {
            PlayerController.Instance.ResetPositionToSpawn();
        }
    }
    
    // Level management
    public void InitializeLevel()
    {
        sceneCollectables = GameObject.FindGameObjectsWithTag("CollectibleBox");
        totalCollected = 0;
        CanvasSystem.Instance.UpdateCollectibleCount(totalCollected, sceneCollectables.Length);
        CanvasSystem.Instance.UpdateLevelCount(_currentLevel);
        
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.ResetPositionToSpawn();
        }
        else
        {
            Debug.LogError("PlayerController instance not found when initializing level!");
        }
    }
    
    public void LoadNextLevel()
    {
        if (IsGameOver) return;

        _currentLevel++;
        if (_currentLevel > MAX_LEVELS)
        {
            bool levelComplete = sceneCollectables != null && totalCollected >= sceneCollectables.Length;
            GameOver(levelComplete);
        }
        else
        {
            CancelInvoke("UpdateCountdownTimer");
            Time.timeScale = 1f;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Level_" + _currentLevel);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        InitializeLevel();
        CanvasSystem.Instance.SetHUD(true);
        PlayerController.Instance.UpdateLivesDisplay();
        
        if (_currentLevel == 1)
        {
            AudioSystem.Instance.PlayMusic();
        }
        
        InvokeRepeating("UpdateCountdownTimer", 1f, 1f);
    }
}
