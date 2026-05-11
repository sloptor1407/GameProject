using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Controls Display")]
    [SerializeField] TextMeshProUGUI controlsText;

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumes();

        if (controlsText != null)
            controlsText.text =
                "MOVER:         A / D\n" +
                "SALTAR:        Space\n" +
                "DOBLE SALTO:   Space (en el aire)\n" +
                "DASH:          Left Shift\n" +
                "ATAQUE MELEE:  J\n" +
                "ATAQUE RANGO:  K\n" +
                "PAUSA:         Escape";   
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