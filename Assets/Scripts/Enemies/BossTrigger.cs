using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] GameObject bossHealthBarUI;
    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        bossHealthBarUI?.SetActive(true);
    }
}