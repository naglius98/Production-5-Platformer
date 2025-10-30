using UnityEngine;

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
    }
    else
    {
        Destroy(gameObject);
    }
  }

  void Start()
  {
    if (GameMusic != null)
    {
        PlayMusic(false, GameMusic);
    }

  }

  public static void PlayMusic(bool ResetSong, AudioClip audioClip = null)
  {
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
        Instance.AudioSource.Play();
    }
  }

  public static void PauseMusic()
  {
    Instance.AudioSource.Pause();
  }

}

