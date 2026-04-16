using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int checkpointId;

    bool activated = false;

    public bool Activated => activated;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        respawn?.SetRespawnPoint(transform.position);

        // Aquí después activaremos la animación del checkpoint
        Debug.Log($"Checkpoint {checkpointId} activado");
    }
}