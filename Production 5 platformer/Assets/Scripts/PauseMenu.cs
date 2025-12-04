using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;  

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject PauseMenuPanel;
    
    private bool isPaused = false;
    
    void Awake()
    {
        // Force reset on script load
        isPaused = false;
        Time.timeScale = 1f;
        
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
        }
    }
    
    void Start()
    {
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
        }
        
        isPaused = false;
        Time.timeScale = 1f;
    }
    
    void Update()
    {
        // Toggle pause with P
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    
    public void Pause()
    {
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(true);
            Time.timeScale = 0f; // Freeze game time
            isPaused = true;
            Debug.Log("Game Paused");
        }
    }
    
    public void Resume()
    {
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game time
            isPaused = false;
            Debug.Log("Game Resumed");
        }
    }
    
    public void ExitToMainMenu()
    {
        // Hide pause menu first
        if (PauseMenuPanel != null)
        {
            PauseMenuPanel.SetActive(false);
        }
        
        Time.timeScale = 1f; // Reset time scale
        isPaused = false;
        
        MusicManager.StopMusic(); // Stop the music
        
        Debug.Log("Exiting to main menu...");
        
        // Load main menu 
        SceneManager.LoadScene("MainMenu");
    }
}
