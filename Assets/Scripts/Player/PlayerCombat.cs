using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Melee")]
    [SerializeField] Transform meleeHitPoint;
    [SerializeField] float meleeRange = 0.8f;
    [SerializeField] LayerMask enemyLayer;

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
        animatorController?.TriggerMeleeAttack();

        // Espera un poco para que coincida con el frame de impacto
        yield return new WaitForSeconds(0.15f);

        // Detecta enemigos en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            meleeHitPoint.position, meleeRange, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<EnemyStats>()?.ReceiveDamage(meleeWeapon.Attack());
        }

        yield return new WaitForSeconds(meleeWeapon.cooldown);
        canMeleeAttack = true;
    }

    IEnumerator RangeRoutine()
    {
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
}