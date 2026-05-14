using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip level1Music;
    [SerializeField] AudioClip level2Music;
    [SerializeField] AudioClip level3Music;

    [Header("SFX")]
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip meleeSFX;
    [SerializeField] AudioClip rangeSFX;
    [SerializeField] AudioClip damageSFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip coinSFX;
    [SerializeField] AudioClip checkpointSFX;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Música

    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayLevel1Music() => PlayMusic(level1Music);
    public void PlayLevel2Music() => PlayMusic(level2Music);
    public void PlayLevel3Music() => PlayMusic(level3Music);

    void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // SFX

    public void PlayJump() => PlaySFX(jumpSFX);
    public void PlayDash() => PlaySFX(dashSFX);
    public void PlayMelee() => PlaySFX(meleeSFX);
    public void PlayRange() => PlaySFX(rangeSFX);
    public void PlayDamage() => PlaySFX(damageSFX);
    public void PlayDeath() => PlaySFX(deathSFX);
    public void PlayCoin() => PlaySFX(coinSFX);
    public void PlayCheckpoint() => PlaySFX(checkpointSFX);

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float v) => musicSource.volume = v;
    public void SetSFXVolume(float v) => sfxSource.volume = v;
}