using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("Patrol")]
    [SerializeField] float patrolDistance = 3f;
    [SerializeField] float waitTime = 1f;

    float leftLimit;
    float rightLimit;
    bool movingRight = true;
    float waitTimer;
    bool isWaiting;

    protected override void Start()
    {
        base.Start();
        // Calcula los límites basándose en la posición inicial
        leftLimit = transform.position.x - patrolDistance;
        rightLimit = transform.position.x + patrolDistance;
    }

    protected override void HandleBehavior()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0) isWaiting = false;
            return;
        }

        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Move(direction);

        float posX = transform.position.x;

        if (movingRight && posX >= rightLimit)
        {
            movingRight = false;
            isWaiting = true;
            waitTimer = waitTime;
            rb.linearVelocity = Vector2.zero;
        }
        else if (!movingRight && posX <= leftLimit)
        {
            movingRight = true;
            isWaiting = true;
            waitTimer = waitTime;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public override void Attack()
    {
        // El daño lo aplica OnCollisionEnter2D de la clase base
    }
}