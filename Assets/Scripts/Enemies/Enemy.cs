using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyStats))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected LayerMask playerLayer;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 3f;

    protected Rigidbody2D rb;
    protected EnemyStats stats;
    protected Transform player;
    protected bool isAlive = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
    }

    protected virtual void Start()
    {
        // Busca al jugador en la escena
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        stats.OnDeath += HandleDeath;
    }

    protected virtual void OnDisable()
    {
        stats.OnDeath -= HandleDeath;
    }

    protected virtual void Update()
    {
        if (!isAlive) return;
        HandleBehavior();
    }

    protected abstract void HandleBehavior();
    public abstract void Attack();

    public virtual void Move(Vector2 direction)
    {
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    protected bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= range;
    }

    protected virtual void HandleDeath()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;
        Destroy(gameObject, 0.5f);
    }

    // Daño al jugador por contacto
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (!isAlive) return;
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<PlayerStats>()?.ReceiveDamage(stats.BaseDamage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}