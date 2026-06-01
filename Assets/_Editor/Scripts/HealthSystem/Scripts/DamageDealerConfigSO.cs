using UnityEngine;

[CreateAssetMenu(fileName = "DamageDealerConfig", menuName = "HealthSystem/ScriptableObjects/Damage Dealer Config")]
public class DamageDealerConfigSO : ScriptableObject
{
    public int damageAmount = 10;
    public DamageKeySO damageKey;
    //public float cooldown = 1f;
    public DamageStrategyBase strategy;
}

public abstract class DamageStrategyBase : ScriptableObject
{
    public abstract void ApplyDamage(IDamageable target, int amount, DamageKeySO key, GameObject source);
}
