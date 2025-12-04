using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource AudioSource;
    public AudioClip GameMusic;
 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            AudioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            
            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void Start()
    {
        if (GameMusic != null)
        {
            PlayMusic(false, GameMusic);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play music when game scene loads, stop music in main menu
        if (scene.name == "GameScene") 
        {
            if (GameMusic != null)
            {
                PlayMusic(true, GameMusic);
            }
        }
        else if (scene.name == "MainMenu") 
        {
            // Stop music in main menu
            StopMusic();
        }
    }

    public static void PlayMusic(bool ResetSong, AudioClip audioClip = null)
    {
        if (Instance == null) return;
        
        if (audioClip != null)
        {
            Instance.AudioSource.clip = audioClip;
        }

        if (Instance.AudioSource.clip != null)
        {
            if (ResetSong)
            {
                Instance.AudioSource.Stop();
            }
            
            if (!Instance.AudioSource.isPlaying)
            {
                Instance.AudioSource.Play();
            }
        }
    }

    public static void PauseMusic()
    {
        if (Instance != null && Instance.AudioSource != null)
        {
            Instance.AudioSource.Pause();
        }
    }

    public static void StopMusic()
    {
        if (Instance != null && Instance.AudioSource != null)
        {
            Instance.AudioSource.Stop();
        }
    }

}

