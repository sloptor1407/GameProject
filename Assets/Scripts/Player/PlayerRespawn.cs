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

        // Si hay checkpoint guardado en sesión, úsalo
        if (GameSession.HasRespawnPoint)
        {
            respawnPoint = GameSession.RespawnPoint;
            transform.position = respawnPoint;
        }
        else
        {
            respawnPoint = transform.position;
        }
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
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // Desactiva el input para que no se pueda mover
        GetComponent<PlayerInputHandler>().enabled = false;
        GetComponent<PlayerController>().enabled = false;

        // Espera a que termine la animación de muerte
        yield return new WaitForSeconds(1.5f);

        // Muestra el game over
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