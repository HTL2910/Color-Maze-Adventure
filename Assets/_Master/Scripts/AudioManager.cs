using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource musicSource;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip buttonClickSound;
    public AudioClip dragAndDropSound;
    public AudioClip backggroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic()
    {
        musicSource.volume = 0.5f;
        musicSource.clip = backggroundMusic;
        musicSource.Play();
    }
    public void PlayWinSound()
    {
        musicSource.PlayOneShot(winSound);
    }
    public void PlayLoseSound()
    {
        musicSource.PlayOneShot(loseSound);
    }
    public void PlayButtonClickSound()
    {
        musicSource.PlayOneShot(buttonClickSound);
    }
    public void PlayDragAndDropSound()
    {
        musicSource.volume = 0.5f;
        musicSource.PlayOneShot(dragAndDropSound);
    }
    
}
