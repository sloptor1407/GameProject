using UnityEngine;

public class ChaserEnemy : Enemy
{
    [Header("Chaser")]
    [SerializeField] float chaseSpeed = 5f;

    bool isChasing;
    public bool IsChasing => isChasing;
    bool isAttacking;
    public bool IsAttacking => isAttacking;

    protected override void HandleBehavior()
    {
        if (PlayerInRange(detectionRange))
        {
            isChasing = true;
            moveSpeed = chaseSpeed;
        }
        else
        {
            isChasing = false;
            moveSpeed = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        if (isChasing && player != null)
        {
            if (!PlayerInRange(attackRange))
            {
                isAttacking = false;
                Vector2 direction = (player.position - transform.position).normalized;
                Move(direction);
            }
            else
            {
                isAttacking = true;
                rb.linearVelocity = Vector2.zero;
                Attack();
            }
        }
        else
        {
            isAttacking = false;
        }
    }

    public override void Move(Vector2 direction)
    {
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public override void Attack()
    {
        // El daño lo aplica OnCollisionEnter2D de la clase base
    }
}