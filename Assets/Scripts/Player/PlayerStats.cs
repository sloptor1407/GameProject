using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 5;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0;

    // Eventos para que la UI y el Animator se suscriban
    public event Action<int, int> OnHealthChanged;  // (currentHP, maxHP)
    public event Action OnDeath;
    public event Action OnDamageReceived;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public bool ReceiveDamage(int amount)
    {
        if (!IsAlive) return false;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        else
        {
            OnDamageReceived?.Invoke();
        }

        return CurrentHealth <= 0;
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}