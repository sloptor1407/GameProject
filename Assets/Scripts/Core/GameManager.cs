using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentLevel { get; private set; } = 1;
    public int MaxLevelUnlocked { get; private set; } = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(int levelIndex)
    {
        CurrentLevel = levelIndex;
        SceneManager.LoadScene(levelIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void UnlockNextLevel()
    {
        if (CurrentLevel >= MaxLevelUnlocked)
            MaxLevelUnlocked = CurrentLevel + 1;
    }

    public void GameOver()
    {
        SceneManager.LoadScene(4);
    }
}