using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public void ReceiveDamage(int amount)
    {
        Debug.Log($"Enemigo recibe {amount} de daño");
    }
}