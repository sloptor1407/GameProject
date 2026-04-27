using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        // Carga valores guardados
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumes();
    }

    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        AudioManager.Instance?.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        AudioManager.Instance?.SetSFXVolume(value);
    }

    void ApplyVolumes()
    {
        AudioManager.Instance?.SetMusicVolume(musicSlider.value);
        AudioManager.Instance?.SetSFXVolume(sfxSlider.value);
    }
}