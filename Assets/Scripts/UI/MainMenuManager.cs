using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject levelSelectPanel;

    [Header("Player Name")]
    [SerializeField] TMP_InputField nameInputField;

    void Start()
    {
        ShowMainMenu();
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
        string nombre = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.Log("Introduce un nombre");
            return;
        }

        // Crea o reutiliza el jugador
        if (!GameSession.HasActiveSession)
        {
            int codJugador = DatabaseManager.Instance.CrearJugador(nombre);
            GameSession.CodJugador = codJugador;
            GameSession.NombreJugador = nombre;
        }

        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
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