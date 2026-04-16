using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    Animator animator;
    PlayerController controller;
    PlayerStats stats;
    Rigidbody2D rb;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Los scripts están en el padre (Player), este script va en Sprite
        controller = GetComponentInParent<PlayerController>();
        stats = GetComponentInParent<PlayerStats>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void OnEnable()
    {
        stats.OnDamageReceived += TriggerHurt;
        stats.OnDeath += TriggerDeath;
    }

    void OnDisable()
    {
        stats.OnDamageReceived -= TriggerHurt;
        stats.OnDeath -= TriggerDeath;
    }

    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsGrounded", controller.IsGrounded);
        animator.SetBool("IsJumping", !controller.IsGrounded && rb.linearVelocity.y > 0.1f);
        animator.SetBool("IsFalling", !controller.IsGrounded && rb.linearVelocity.y < -0.5f);
        animator.SetBool("IsDashing", controller.IsDashing);
        animator.SetBool("IsBraking", controller.IsBraking);
    }

    public void TriggerMeleeAttack() => animator.SetTrigger("MeleeAttack");
    public void TriggerRangeAttack() => animator.SetTrigger("RangeAttack");
    void TriggerHurt() => animator.SetTrigger("Hurt");
    void TriggerDeath() => animator.SetTrigger("Death");
}