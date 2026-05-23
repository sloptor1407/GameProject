using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee")]
    [SerializeField] Transform meleeHitPoint;
    [SerializeField] float meleeRange = 0.8f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Animator meleeSlashAnimator;

    [Header("Range")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    [Header("Weapons")]
    [SerializeField] Weapon meleeWeapon;
    [SerializeField] Weapon rangeWeapon;

    bool canMeleeAttack = true;
    bool canRangeAttack = true;
    bool facingRight = true;

    PlayerAnimatorController animatorController;
    PlayerController playerController;

    void Awake()
    {
        animatorController = GetComponentInChildren<PlayerAnimatorController>();
        playerController = GetComponent<PlayerController>();
    }

    // Llamados desde PlayerInputHandler

    public void MeleeAttack()
    {
        if (!canMeleeAttack || meleeWeapon == null) return;
        StartCoroutine(MeleeRoutine());
    }

    public void RangeAttack()
    {
        if (!canRangeAttack || rangeWeapon == null) return;
        StartCoroutine(RangeRoutine());
    }

    // Rutinas

    IEnumerator MeleeRoutine()
    {
        canMeleeAttack = false;
        AudioManager.Instance?.PlayMelee();

        animatorController?.TriggerMeleeAttack();

        meleeHitPoint.GetComponent<SpriteRenderer>().enabled = true;
        meleeSlashAnimator.Play("SlashAnimation", 0, 0f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            meleeHitPoint.position, meleeRange,
            LayerMask.GetMask("Enemy"));

        Debug.Log($"Hits detectados: {hits.Length}");
        foreach (Collider2D hit in hits)
            Debug.Log($"Hit: {hit.gameObject.name}");

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<EnemyStats>()?.ReceiveDamage(meleeWeapon.damage);
            hit.GetComponent<BossController>()?.ReceiveDamage(meleeWeapon.damage);
        }

        yield return new WaitForSeconds(meleeWeapon.cooldown);
        meleeHitPoint.GetComponent<SpriteRenderer>().enabled = false;
        canMeleeAttack = true;
    }

    IEnumerator RangeRoutine()
    {
        AudioManager.Instance?.PlayRange();
        canRangeAttack = false;
        animatorController?.TriggerRangeAttack();

        yield return new WaitForSeconds(0.1f);

        if (projectilePrefab != null && firePoint != null)
        {
            Vector2 dir = facingRight ? Vector2.right : Vector2.left;
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>()?.Init(dir, rangeWeapon.Attack());
        }

        yield return new WaitForSeconds(rangeWeapon.cooldown);
        canRangeAttack = true;
    }

    // Sincronizar dirección con PlayerController

    void Update()
    {
        facingRight = playerController.FacingRight;
    }

    // Gizmo del hitbox melee

    void OnDrawGizmosSelected()
    {
        if (meleeHitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeHitPoint.position, meleeRange);
        }
    }

    public void UpgradeWeapon(WeaponType type, int damageBonus)
    {
        if (type == WeaponType.MELEE && meleeWeapon != null)
        {
            meleeWeapon.damage += damageBonus;
            Debug.Log($"Arma melee mejorada: {meleeWeapon.damage} de daño");
        }
        else if (type == WeaponType.RANGE && rangeWeapon != null)
        {
            rangeWeapon.damage += damageBonus;
            Debug.Log($"Arma a distancia mejorada: {rangeWeapon.damage} de daño");
        }
    }
}