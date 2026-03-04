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

    private bool musicMuted = false;
    private bool sfxMuted = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ================== SFX ==================
    public void PlaySFX(AudioClip clip)
    {
        if (sfxMuted || clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void ToggleSFX()
    {
        sfxMuted = !sfxMuted;
        sfxSource.mute = sfxMuted;
    }

    // ================== MUSIC ==================
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        if (!musicSource.enabled) 
            musicSource.enabled = true;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        musicSource.mute = musicMuted;
    }
}