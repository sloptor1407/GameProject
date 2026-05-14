using UnityEngine;
using System;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] EnemyType enemyType;
    [SerializeField] int baseHealth = 3;
    [SerializeField] int baseDamage = 1;

    public int CurrentHealth { get; private set; }
    public int BaseDamage => baseDamage;
    public bool IsAlive => CurrentHealth > 0;

    public event Action<int> OnDamageReceived;
    public event Action OnDeath;

    void Awake()
    {
        CurrentHealth = baseHealth;
    }

    public void ReceiveDamage(int amount)
    {
        if (!IsAlive) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamageReceived?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }
}