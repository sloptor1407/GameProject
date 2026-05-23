using UnityEngine;

public class BossAnimator : MonoBehaviour
{
    Animator animator;
    BossController boss;

    void Awake()
    {
        animator = GetComponent<Animator>();
        boss = GetComponentInParent<BossController>();
    }

    void Update()
    {
        if (boss == null) return;
        animator.SetBool("IsWalking", boss.IsWalking);
    }

    public void TriggerAttack() => animator?.SetTrigger("Attack");
    public void TriggerSpell() => animator?.SetTrigger("Spell");
    public void TriggerCast() => animator?.SetTrigger("Cast");
    public void TriggerHurt() => animator?.SetTrigger("Hurt");
    public void TriggerDeath() => animator?.SetTrigger("Death");
}