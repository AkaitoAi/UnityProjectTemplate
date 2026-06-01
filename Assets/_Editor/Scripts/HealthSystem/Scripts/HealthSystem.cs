using UnityEngine;
using UnityEngine.Events;
using System;
using AkaitoAi.Extensions;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthSystemConfigSO config;
    [SerializeField] private DamageKeySO[] allowedKeys;

    public static Action<int> OnHealthChanged;
    public static Action OnDeath;

    public UnityEvent onHealthChangedEvent;
    public UnityEvent onDeathEvent;

    internal bool _inCooldown = false;
    public bool inCooldown => _inCooldown;

    private void Awake()
    {
        config.ResetHealth();

        config.OnAwakeHealthChanged?.Invoke(config.GetCurrentHealth);
        OnHealthChanged?.Invoke(config.GetCurrentHealth);
    }

    public void TakeDamage(int amount, DamageKeySO key, GameObject source)
    {
        if (config.isImmortal) return;

        if (_inCooldown)
            return;

        _inCooldown = true;
        StartCoroutine(AkaitoAiExtensions.SimpleDelay(config.cooldownDuration, () => _inCooldown = false));

        config.SetCurrentHealth(config.GetCurrentHealth - amount);

        config.OnHealthChanged?.Invoke(config.GetCurrentHealth);
        OnHealthChanged?.Invoke(config.GetCurrentHealth);
        onHealthChangedEvent?.Invoke();

        if (config.GetCurrentHealth <= 0)
        {
            config.OnDeath?.Invoke();
            OnDeath?.Invoke();
            onDeathEvent?.Invoke();
        }
    }

    public bool CanReceiveDamageFrom(DamageKeySO key)
    {
        foreach (DamageKeySO allowed in allowedKeys)
        {
            if (allowed == key)
                return true;
        }
        return false;
    }
}
