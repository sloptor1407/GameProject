using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int checkpointId;

    Animator animator;
    bool activated = false;

    public bool Activated => activated;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        animator?.SetTrigger("Activated");
        AudioManager.Instance?.PlayCheckpoint();

        // Guarda en GameSession para que persista
        GameSession.RespawnPoint = transform.position;
        GameSession.HasRespawnPoint = true;

        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        respawn?.SetRespawnPoint(transform.position);
        Debug.Log($"Checkpoint {checkpointId} activado");
    }
}