using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] int damage = 5;
    [SerializeField] bool killInstantly = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        if (killInstantly)
            stats.ReceiveDamage(stats.CurrentHealth);
        else
            stats.ReceiveDamage(damage);
    }
}