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

    public bool IsWalking => !isAttacking && isAlive && player != null &&
                     Vector2.Distance(transform.position, player.position) > meleeRange;

    // Cached animator
    BossAnimator bossAnimator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossAnimator = GetComponentInChildren<BossAnimator>();
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

        StartCoroutine(BossIntro());
    }

    void Update()
    {
        if (!isAlive || player == null || isCharging) return;
        HandleFacing();
        if (currentPhase == 1) HandlePhase1();
        else HandlePhase2();
        meleeTimer -= Time.deltaTime;
        rangeTimer -= Time.deltaTime;
        slashTimer -= Time.deltaTime;
        chargeTimer -= Time.deltaTime;
    }

    void HandlePhase1()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (!isAttacking)
        {
            if (dist > meleeRange + 0.5f) Move();
            else rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (dist <= meleeRange && meleeTimer <= 0)
                StartCoroutine(MeleeAttack());
            else if (dist > meleeRange + 2f && rangeTimer <= 0)
                StartCoroutine(RangeAttack());
        }
    }

    void HandlePhase2()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (!isAttacking)
        {
            if (dist > meleeRange + 0.5f) Move();
            else rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            if (slashTimer <= 0)
                StartCoroutine(SlashAttack());
            else if (chargeTimer <= 0 && dist > 3f)
                StartCoroutine(ChargeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;
        bossAnimator?.TriggerAttack(); // animación
        meleeTimer = meleeCooldown;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            meleeHitPoint.position, meleeRange,
            LayerMask.GetMask("Player"));
        foreach (Collider2D hit in hits)
            hit.GetComponent<PlayerStats>()?.ReceiveDamage(meleeDamage);

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator RangeAttack()
    {
        isAttacking = true;
        bossAnimator?.TriggerSpell(); // animación
        rangeTimer = rangeCooldown;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.8f);

        if (projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<EnemyProjectile>()?.Init(dir, rangeDamage);
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator SlashAttack()
    {
        isAttacking = true;
        bossAnimator?.TriggerCast(); // animación
        slashTimer = slashCooldown;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);

        if (slashPrefab != null)
        {
            GameObject slash = Instantiate(slashPrefab, transform.position, Quaternion.identity);
            Destroy(slash, 1f);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null && pc.IsGrounded)
                playerObj.GetComponent<PlayerStats>()?.ReceiveDamage(slashDamage);
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator ChargeAttack()
    {
        isAttacking = true;
        isCharging = true;
        chargeTimer = chargeCooldown;
        yield return new WaitForSeconds(1f);

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

    public void ReceiveDamage(int amount)
    {
        if (!isAlive) return;
        bossAnimator?.TriggerHurt(); // animación

        currentHealth -= amount;

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
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.5f);

        if (bossNameText != null)
            bossNameText.text = "JEFE FINAL - FASE 2";

        isAttacking = false;
    }

    IEnumerator BossDeath()
    {
        isAlive = false;
        bossAnimator?.TriggerDeath(); // animación
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        GameObject door = GameObject.FindGameObjectWithTag("BossDoor");
        if (door != null)
        {
            door.GetComponent<Collider2D>().enabled = false;
            door.GetComponent<SpriteRenderer>().enabled = false;
        }

        int muertes = FindFirstObjectByType<PlayerRespawn>()?.DeathCount ?? 0;
        LevelResultsManager.Instance?.ShowGameComplete(muertes);

        Destroy(gameObject, 0.5f);
    }

    IEnumerator BossIntro()
    {
        isAttacking = true;
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

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
                facingRight ? -transform.localScale.x : Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z);
        }
    }

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