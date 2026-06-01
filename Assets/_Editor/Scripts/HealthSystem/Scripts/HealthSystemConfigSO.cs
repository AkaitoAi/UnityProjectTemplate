using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthSystemConfig", menuName = "HealthSystem/ScriptableObjects/Health System Config")]
public class HealthSystemConfigSO : ScriptableObject
{
    public bool isImmortal = false;
    public int maxHealth = 100;
    public int currentHealth;
    public float cooldownDuration = 1f;

    public Action<int> OnAwakeHealthChanged, OnHealthChanged;
    public Action OnDeath;

    public int GetCurrentHealth => currentHealth;

    public void SetCurrentHealth(int value) => currentHealth = Mathf.Clamp(value, 0, maxHealth);

    public void ResetHealth() => currentHealth = maxHealth;
}
