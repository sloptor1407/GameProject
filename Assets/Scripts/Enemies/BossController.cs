using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int maxHealthPhase1 = 20;
    [SerializeField] int maxHealthPhase2 = 20;
    [SerializeField] int contactDamage = 1;

    [Header("Phase 1 - Melee")]
    [SerializeField] Transform meleeHitPoint;
    [SerializeField] float meleeRange = 1.5f;
    [SerializeField] int meleeDamage = 2;
    [SerializeField] float meleeCooldown = 2f;

    [Header("Phase 1 - Range")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] int rangeDamage = 1;
    [SerializeField] float rangeCooldown = 3f;

    [Header("Phase 2 - Slash")]
    [SerializeField] GameObject slashPrefab;
    [SerializeField] int slashDamage = 3;
    [SerializeField] float slashCooldown = 4f;

    [Header("Phase 2 - Charge")]
    [SerializeField] float chargeSpeed = 15f;
    [SerializeField] float chargeDuration = 0.5f;
    [SerializeField] int chargeDamage = 2;
    [SerializeField] float chargeCooldown = 5f;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float detectionRange = 10f;

    [Header("UI")]
    [SerializeField] UnityEngine.UI.Slider bossHealthBar;
    [SerializeField] TMPro.TextMeshProUGUI bossNameText;

    // State
    int currentPhase = 1;
    int currentHealth;
    int totalMaxHealth;
    bool isAlive = true;
    bool isAttacking = false;
    bool isCharging = false;

    float meleeTimer;
    float rangeTimer;
    float slashTimer;
    float chargeTimer;

    Rigidbody2D rb;
    Transform player;
    bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        totalMaxHealth = maxHealthPhase1 + maxHealthPhase2;
        currentHealth = maxHealthPhase1;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (bossHealthBar != null)
        {
            bossHealthBar.maxValue = totalMaxHealth;
            bossHealthBar.value = totalMaxHealth;
        }

        if (bossNameText != null)
            bossNameText.text = "JEFE FINAL";

        // Inicia la batalla
        StartCoroutine(BossIntro());
    }

    void Update()
    {
        if (!isAlive || player == null || isCharging) return;

        HandleFacing();

        if (currentPhase == 1)
            HandlePhase1();
        else
            HandlePhase2();

        // Timers
        meleeTimer -= Time.deltaTime;
        rangeTimer -= Time.deltaTime;
        slashTimer -= Time.deltaTime;
        chargeTimer -= Time.deltaTime;
    }

    // Fases

    void HandlePhase1()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (!isAttacking)
        {
            // Se acerca al jugador
            if (dist > meleeRange + 0.5f)
                Move();
            else
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // Ataque melee si está cerca
            if (dist <= meleeRange && meleeTimer <= 0)
                StartCoroutine(MeleeAttack());

            // Ataque a distancia si está lejos
            else if (dist > meleeRange + 2f && rangeTimer <= 0)
                StartCoroutine(RangeAttack());
        }
    }

    void HandlePhase2()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (!isAttacking)
        {
            if (dist > meleeRange + 0.5f)
                Move();
            else
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            // Tajo que ocupa toda la pantalla
            if (slashTimer <= 0)
                StartCoroutine(SlashAttack());

            // Embestida
            else if (chargeTimer <= 0 && dist > 3f)
                StartCoroutine(ChargeAttack());
        }
    }

    // Ataques Fase 1

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        meleeTimer = meleeCooldown;

        rb.linearVelocity = Vector2.zero;

        // Pequeño anticipation
        yield return new WaitForSeconds(0.3f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            meleeHitPoint.position, meleeRange,
            LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
            hit.GetComponent<PlayerStats>()?.ReceiveDamage(meleeDamage);

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    IEnumerator RangeAttack()
    {
        isAttacking = true;
        rangeTimer = rangeCooldown;

        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);

        if (projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject proj = Instantiate(projectilePrefab,
                firePoint.position, Quaternion.identity);
            proj.GetComponent<EnemyProjectile>()?.Init(dir, rangeDamage);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // Ataques Fase 2

    IEnumerator SlashAttack()
    {
        isAttacking = true;
        slashTimer = slashCooldown;

        rb.linearVelocity = Vector2.zero;

        // Aviso visual antes del tajo
        yield return new WaitForSeconds(0.5f);

        if (slashPrefab != null)
        {
            // Instancia el tajo centrado en el boss
            GameObject slash = Instantiate(slashPrefab,
                transform.position, Quaternion.identity);
            Destroy(slash, 1f);
        }

        // Daña al jugador si está en el suelo
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            // Solo daña si el jugador está en el suelo (no saltando)
            if (pc != null && pc.IsGrounded)
                playerObj.GetComponent<PlayerStats>()?.ReceiveDamage(slashDamage);
        }

        yield return new WaitForSeconds(0.8f);
        isAttacking = false;
    }

    IEnumerator ChargeAttack()
    {
        isAttacking = true;
        isCharging = true;
        chargeTimer = chargeCooldown;

        // Pequeño aviso
        yield return new WaitForSeconds(0.4f);

        float dir = facingRight ? 1f : -1f;
        float timer = 0f;

        while (timer < chargeDuration)
        {
            rb.linearVelocity = new Vector2(dir * chargeSpeed, rb.linearVelocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isCharging = false;
        isAttacking = false;
    }

    // Daño y fases

    public void ReceiveDamage(int amount)
    {
        if (!isAlive) return;

        currentHealth -= amount;

        // Actualiza la barra de vida
        if (bossHealthBar != null)
            bossHealthBar.value = currentHealth +
                (currentPhase == 2 ? maxHealthPhase1 : 0);

        if (currentHealth <= 0)
        {
            if (currentPhase == 1)
                StartCoroutine(TransitionToPhase2());
            else
                StartCoroutine(BossDeath());
        }
    }

    IEnumerator TransitionToPhase2()
    {
        isAttacking = true;
        currentPhase = 2;
        currentHealth = maxHealthPhase2;

        // Efecto de transición
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);

        if (bossNameText != null)
            bossNameText.text = "JEFE FINAL - FASE 2";

        isAttacking = false;
        Debug.Log("Boss entra en Fase 2");
    }

    IEnumerator BossDeath()
    {
        isAlive = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        // Desactiva colliders
        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        // Muestra pantalla de juego completado
        int muertes = FindFirstObjectByType<PlayerRespawn>()?.DeathCount ?? 0;
        LevelResultsManager.Instance?.ShowGameComplete(muertes);

        Destroy(gameObject, 0.5f);
    }

    IEnumerator BossIntro()
    {
        // Pequeña pausa antes de que el boss empiece a atacar
        isAttacking = true;
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    // Movimiento

    void Move()
    {
        float dir = player.position.x > transform.position.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    void HandleFacing()
    {
        if (player == null) return;
        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != facingRight)
        {
            facingRight = shouldFaceRight;
            transform.localScale = new Vector3(
                facingRight ? 1 : -1,
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    // Contacto

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isAlive || isCharging) return;
        if (other.gameObject.CompareTag("Player"))
            other.gameObject.GetComponent<PlayerStats>()?
                .ReceiveDamage(isCharging ? chargeDamage : contactDamage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (meleeHitPoint != null)
            Gizmos.DrawWireSphere(meleeHitPoint.position, meleeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}