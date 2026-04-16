using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon")]
public class Weapon : ScriptableObject
{
    public int codWeapon;
    public string weaponName;
    public WeaponType weaponType;
    public int damage;
    public float cooldown;

    public int Attack()
    {
        return damage;
    }
}