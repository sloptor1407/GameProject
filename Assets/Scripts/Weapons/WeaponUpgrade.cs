using UnityEngine;

public class WeaponUpgrade : MonoBehaviour
{
    [SerializeField] WeaponType weaponTypeToUpgrade = WeaponType.MELEE;
    [SerializeField] int damageBonus = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerCombat combat = other.GetComponent<PlayerCombat>();
        if (combat == null) return;

        combat.UpgradeWeapon(weaponTypeToUpgrade, damageBonus);

        // Guardar en BD
        if (GameSession.HasActiveSession)
        {
            int codObjeto = weaponTypeToUpgrade == WeaponType.MELEE ? 2 : 3;
            DatabaseManager.Instance?.GuardarObjeto(
                GameSession.CodJugador,
                codObjeto,
                true
            );
            DatabaseManager.Instance?.EquiparArma(
                GameSession.CodJugador,
                weaponTypeToUpgrade == WeaponType.MELEE ? 1 : 2
            );
        }

        Destroy(gameObject);
    }
}