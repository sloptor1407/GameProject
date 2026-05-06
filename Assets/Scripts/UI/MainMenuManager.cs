using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject levelSelectPanel;

    [Header("Level Buttons")]
    [SerializeField] Button level1Button;
    [SerializeField] Button level2Button;
    [SerializeField] Button level3Button;

    void Start()
    {
        // Todo desactivado — ProfileManager se encarga de activar lo correcto
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }

    // Navegación

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        levelSelectPanel.SetActive(true);

        int maxNivel = GameSession.MaxNivelDesbloqueado;
        if (level1Button != null) level1Button.interactable = maxNivel >= 1;
        if (level2Button != null) level2Button.interactable = maxNivel >= 2;
        if (level3Button != null) level3Button.interactable = maxNivel >= 3;
    }

    // Niveles

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    // Salir

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}