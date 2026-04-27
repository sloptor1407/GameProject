using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] float respawnDelay = 2f;

    PlayerStats stats;
    PlayerController controller;

    Vector2 respawnPoint;
    int deathCount = 0;

    public int DeathCount => deathCount;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<PlayerController>();
        respawnPoint = transform.position;
    }

    void OnEnable()
    {
        stats.OnDeath += HandleDeath;
    }

    void OnDisable()
    {
        stats.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        deathCount++;
        GameSession.MuertesTotales++;
        controller.enabled = false;
        LevelResultsManager.Instance?.ShowGameOver();
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        transform.position = respawnPoint;
        stats.ResetHealth();
        controller.enabled = true;
    }
}