using UnityEngine;
using System.Collections;

public class ShooterEnemy : Enemy
{
    [Header("Shooter")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 2f;

    bool canShoot = true;

    protected override void HandleBehavior()
    {
        if (PlayerInRange(detectionRange))
        {
            // Se gira hacia el jugador pero no se mueve
            if (player != null)
            {
                float dir = player.position.x - transform.position.x;
                transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
            }

            if (PlayerInRange(attackRange) && canShoot)
                Attack();
        }
    }

    public override void Attack()
    {
        if (!canShoot) return;
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        canShoot = false;

        // Activa animación de ataque
        GetComponentInChildren<ShooterEnemyAnimator>()?.TriggerAttack();

        if (projectilePrefab != null && firePoint != null && player != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<EnemyProjectile>()?.Init(direction, stats.BaseDamage);
        }

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}