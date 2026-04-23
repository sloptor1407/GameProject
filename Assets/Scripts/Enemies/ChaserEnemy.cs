using UnityEngine;

public class ChaserEnemy : Enemy
{
    [Header("Chaser")]
    [SerializeField] float chaseSpeed = 5f;

    bool isChasing;

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
                Vector2 direction = (player.position - transform.position).normalized;
                Move(direction);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                Attack();
            }
        }
    }

    public override void Attack()
    {
        // El daño lo aplica OnCollisionEnter2D de la clase base
    }
}