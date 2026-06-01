using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount, DamageKeySO key, GameObject source);
    bool CanReceiveDamageFrom(DamageKeySO key);
    bool inCooldown { get; }
}
