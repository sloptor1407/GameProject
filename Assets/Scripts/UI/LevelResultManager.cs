using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelResultsManager : MonoBehaviour
{
    public static LevelResultsManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameCompletePanel;

    [Header("Level Complete")]
    [SerializeField] TextMeshProUGUI levelCompleteTimeText;

    [Header("Game Over")]
    // Sin datos extra por ahora

    [Header("Game Complete")]
    [SerializeField] TextMeshProUGUI totalTimeText;
    [SerializeField] TextMeshProUGUI totalDeathsText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Mostrar pantallas

    public void ShowLevelComplete()
    {
        GameTimer.Instance?.StopTimer();
        Time.timeScale = 0f;

        float time = GameTimer.Instance?.ElapsedTime ?? 0f;
        int nivel = SceneManager.GetActiveScene().buildIndex;

        // Desbloquear siguiente nivel
        GameManager.Instance?.UnlockNextLevel();

        // Guardar en BD
        if (GameSession.HasActiveSession)
        {
            DatabaseManager.Instance?.GuardarPartida(
                GameSession.CodJugador,
                nivel,
                time,
                GameSession.MuertesTotales
            );
            DatabaseManager.Instance?.ActualizarTiempoTotal(
                GameSession.CodJugador,
                time
            );
        }

        levelCompleteTimeText.text = $"Tiempo: {FormatTime(time)}";
        levelCompletePanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        GameTimer.Instance?.StopTimer();
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void ShowGameComplete(int totalDeaths)
    {
        GameTimer.Instance?.StopTimer();
        Time.timeScale = 0f;

        float time = GameTimer.Instance?.ElapsedTime ?? 0f;
        totalTimeText.text = $"Tiempo total: {FormatTime(time)}";
        totalDeathsText.text = $"Muertes: {totalDeaths}";

        gameCompletePanel.SetActive(true);
    }

    // Botones

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next <= 3)
            SceneManager.LoadScene(next);
        else
            ShowGameComplete(0); // Sustituir por muertes reales cuando tengamos BD
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // Utilidad

    string FormatTime(float seconds)
    {
        int h = (int)(seconds / 3600);
        int m = (int)(seconds % 3600 / 60);
        int s = (int)(seconds % 60);
        return $"{h:00}:{m:00}:{s:00}";
    }
}