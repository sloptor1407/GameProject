using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Hearts")]
    [SerializeField] List<Image> heartImages;
    [SerializeField] Color heartFullColor = Color.red;
    [SerializeField] Color heartEmptyColor = new Color(1, 0, 0, 0.2f);

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Coins")]
    [SerializeField] TextMeshProUGUI coinsText;
    int totalCoins = 40;
    int collectedCoins = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Se suscribe a los eventos del jugador
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        if (stats != null)
            stats.OnHealthChanged += UpdateHearts;

        UpdateCoins(0);
    }

    // Corazones

    void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < heartImages.Count; i++)
            heartImages[i].color = i < current ? heartFullColor : heartEmptyColor;
    }

    // Timer

    public void UpdateTimer(float totalSeconds)
    {
        int hours = (int)(totalSeconds / 3600);
        int minutes = (int)(totalSeconds % 3600 / 60);
        int seconds = (int)(totalSeconds % 60);
        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    // Monedas

    public void UpdateCoins(int amount)
    {
        collectedCoins += amount;
        coinsText.text = $"{collectedCoins} / {totalCoins}";
    }
}