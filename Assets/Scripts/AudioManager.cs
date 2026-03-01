using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Player Sounds")]
    public AudioClip jumpSound;
    public AudioClip playerDeathSound;
    public AudioClip playerHitSound;

    [Header("Enemy Sounds")]
    public AudioClip enemyDeathSound;

    [Header("Item Sounds")]
    public AudioClip gemSound;
    public AudioClip healthPickupSound;

    void Awake()
    {    Debug.Log("AudioManager Awake called.");
        if (Instance == null)
        {
            Debug.Log("AudioManager instance Is null.");
            Instance = this;
            
        }
        else
        {
            Debug.Log("AudioManager instance found.");
            
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        Debug.Log($"Playing sound: {clip.name}");
        sfxSource.PlayOneShot(clip);
    }
}
