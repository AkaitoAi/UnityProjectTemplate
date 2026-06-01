using AkaitoAi.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private DamageDealerConfigSO config;
    [SerializeField] private Transform emojiTransform;
    private readonly HashSet<IDamageable> cooldownTargets = new();


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IDamageable damageable)) return;

        TryDealDamage(damageable);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.TryGetComponent(out IDamageable damageable)) return;

        TryDealDamage(damageable);
    }

    private void TryDealDamage(IDamageable damageable)
    {
        Debug.Log(damageable.inCooldown + " Damageable in cooldown");

        if (damageable.inCooldown || !damageable.CanReceiveDamageFrom(config.damageKey) || cooldownTargets.Contains(damageable))
            return;

        config.strategy?.ApplyDamage(damageable, config.damageAmount, config.damageKey, gameObject);

        //StartCoroutine(CooldownCoroutine(damageable));
    }

    //private IEnumerator CooldownCoroutine(IDamageable target)
    //{
    //    cooldownTargets.Add(target);
    //    yield return AkaitoAiExtensions.Seconds(config.cooldown);
    //    cooldownTargets.Remove(target);
    //}
}
