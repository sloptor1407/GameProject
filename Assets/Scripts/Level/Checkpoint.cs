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
        AudioManager.Instance?.PlayCheckpoint();
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        animator?.SetTrigger("Activated");

        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        respawn?.SetRespawnPoint(transform.position);

        Debug.Log($"Checkpoint {checkpointId} activado");
    }
}