using UnityEngine;

[CreateAssetMenu(fileName = "DirectDamageStrategy", menuName = "HealthSystem/Damage Strategies/Direct")]
public class DirectDamageStrategy : DamageStrategyBase
{
    public override void ApplyDamage(IDamageable target, int amount, DamageKeySO key, GameObject source)
    {
        target.TakeDamage(amount, key, source);
    }
}
