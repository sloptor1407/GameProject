using UnityEngine;

public class ChaserEnemyAnimator : MonoBehaviour
{
    Animator animator;
    EnemyStats stats;
    ChaserEnemy chaser;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stats = GetComponentInParent<EnemyStats>();
        chaser = GetComponentInParent<ChaserEnemy>();
    }

    void OnEnable()
    {
        stats.OnDamageReceived += TriggerHurt;
        stats.OnDeath += TriggerDie;
    }

    void OnDisable()
    {
        stats.OnDamageReceived -= TriggerHurt;
        stats.OnDeath -= TriggerDie;
    }

    void Update()
    {
        animator.SetBool("IsChasing", chaser.IsChasing);

        if (chaser.IsAttacking)
            animator?.SetTrigger("Attack");
    }

    void TriggerHurt(int currentHealth)
    {
        Debug.Log("TriggerHurt llamado");
        animator?.SetTrigger("Hurt");
    }
    void TriggerDie() => animator?.SetTrigger("Die");
}