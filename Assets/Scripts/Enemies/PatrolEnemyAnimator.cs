using UnityEngine;

public class PatrolEnemyAnimator : MonoBehaviour
{
    Animator animator;
    EnemyStats stats;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stats = GetComponentInParent<EnemyStats>();
    }

    void OnEnable()
    {
        stats.OnDamageReceived += TriggerHit;
    }

    void OnDisable()
    {
        stats.OnDamageReceived -= TriggerHit;
    }

    void TriggerHit(int currentHealth)
    {
        animator?.SetTrigger("Hit");
    }
}