using UnityEngine;

public class ShooterEnemyAnimator : MonoBehaviour
{
    Animator animator;
    EnemyStats stats;
    ShooterEnemy shooter;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stats = GetComponentInParent<EnemyStats>();
        shooter = GetComponentInParent<ShooterEnemy>();
    }

    void OnEnable()
    {
        stats.OnDamageReceived += TriggerHurt;
    }

    void OnDisable()
    {
        stats.OnDamageReceived -= TriggerHurt;
    }

    public void TriggerAttack() => animator?.SetTrigger("Attack");
    void TriggerHurt(int currentHealth) => animator?.SetTrigger("Hurt");
}