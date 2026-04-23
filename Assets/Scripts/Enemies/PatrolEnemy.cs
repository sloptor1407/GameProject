using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("Patrol")]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float waitTime = 1f;

    Transform currentTarget;
    float waitTimer;
    bool isWaiting;

    protected override void Start()
    {
        base.Start();
        currentTarget = pointA;
    }

    protected override void HandleBehavior()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0) isWaiting = false;
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        Move(direction);

        // Llega al punto destino
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
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