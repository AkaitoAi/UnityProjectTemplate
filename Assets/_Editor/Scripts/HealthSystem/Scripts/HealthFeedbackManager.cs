// using RichTap;
using UnityEngine;
using UnityEngine.UI;
using AkaitoAi.GameBase;

public class HealthFeedbackManager : MonoBehaviour
{
    [SerializeField] private Text healthText;
    [SerializeField] private HealthSystemConfigSO config;

    private void UpdateHealthText(int currentHealth)
    {
        if (config == null) return;

        if (healthText == null) return;

        healthText.text = config.GetCurrentHealth.ToString();
    }

    private void OnDamageFeedback()
    {
        if (config == null) return;

        // RichtapEffectSource.Instance?.Play(RichTap.Common.RichtapPreset.RT_CLICK);
    }

    private void DeathEventCall()
    {
        EventBus<OnLevelFailed>.Raise(new OnLevelFailed { reason = "Death" });
    }

    private void OnEnable()
    {
        config.OnAwakeHealthChanged += UpdateHealthText;
        config.OnDeath += DeathEventCall;
        config.OnHealthChanged += (value) =>
        {
            UpdateHealthText(value);
            OnDamageFeedback();
        };
    }

    private void OnDisable()
    {
        config.OnAwakeHealthChanged -= UpdateHealthText;
        config.OnDeath -= DeathEventCall;
        config.OnHealthChanged -= (value) =>
        {
            UpdateHealthText(value);
            OnDamageFeedback();
        };
    }
}
