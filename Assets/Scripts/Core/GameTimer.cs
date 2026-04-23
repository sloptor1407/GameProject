using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    float elapsedTime;
    bool isRunning;

    public float ElapsedTime => elapsedTime;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (!isRunning) return;
        elapsedTime += Time.deltaTime;
        HUDManager.Instance?.UpdateTimer(elapsedTime);
    }

    public void StartTimer() => isRunning = true;
    public void StopTimer() => isRunning = false;
    public void ResetTimer() => elapsedTime = 0;
}