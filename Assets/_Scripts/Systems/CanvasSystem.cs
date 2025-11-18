using UnityEngine;
using TMPro;

public class CanvasSystem : Singleton<CanvasSystem>
{
    [Header("Text References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collectibleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI endGameMessageText;
    
    [Header("Panel References")]
    public GameObject endGameScreenPanel;
    public GameObject pauseMenuPanel;
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    
    public void SetEndGameScreen(bool state)
    {
        if (endGameScreenPanel != null)
            endGameScreenPanel.SetActive(state);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(state);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
    }
    
    public void SetPauseMenu(bool state)
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(state);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (endGameScreenPanel != null)
            endGameScreenPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(!state);
    }
    
    public void SetMainMenu(bool state)
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(state);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        if (endGameScreenPanel != null)
            endGameScreenPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
    }
    
    public void SetHUD(bool state)
    {
        if (hudPanel != null)
            hudPanel.SetActive(state);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        if (endGameScreenPanel != null)
            endGameScreenPanel.SetActive(false);
    }

    public void UpdateTimer(int time)
    {
        if (timerText == null) return;
        int minutes = time / 60;
        int seconds = time % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void UpdateCollectibleCount(int current, int total)
    {
        if (collectibleText == null) return;
        collectibleText.text = current + " / " + total;
    }
    
    public void UpdateLevelCount(int current)
    {
        if (levelText == null) return;
        levelText.text = "Level " + current;
    }
    
    public void UpdateLives(int current, int total)
    {
        if (livesText == null) return;
        livesText.text = current + " / " + total;
    }
    
    public void UpdateEndGameMessage(string message)
    {
        if (endGameMessageText == null) return;
        endGameMessageText.text = message;
    }
    
    public void OnNextLevelButtonClicked()
    {
        GameManager.Instance.LoadNextLevel();
    }
    
    public void OnRestartLevelButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void OnResumeButtonClicked()
    {
        GameManager.Instance.SetPause(false);
    }
}
