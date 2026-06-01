#if DOTWEEN
using UnityEngine;
using DG.Tweening;
using System;

public static class DOTweenExtensions
{
    public class IntCounterPunchBuilder
    {
        private int startValue = 0;
        private int endValue = 0;
        private float duration = 0.5f;
        private Ease ease = DG.Tweening.Ease.OutQuad;
        private Action<int> onValueChanged = null;

        private Transform punchTarget = null;
        private float punchStrength = 0.5f;     // as a relative scalar
        private float punchDuration = 0.2f;
        private int punchVibrato = 7;
        private float punchElasticity = 0.6f;

        private string tweenId = null;
        private bool autoKill = true;

        // Optional: manual base scale override (if you scale at runtime and want to lock to a specific base)
        private Vector3? explicitBaseScale = null;
        private bool usePunchTween = true; // Alternative is Yoyo scale for guaranteed return with a back ease

        public IntCounterPunchBuilder From(int value) { startValue = value; return this; }
        public IntCounterPunchBuilder To(int value) { endValue = value; return this; }
        public IntCounterPunchBuilder Duration(float value) { duration = value; return this; }
        public IntCounterPunchBuilder Ease(Ease value) { ease = value; return this; }
        public IntCounterPunchBuilder OnValueChanged(Action<int> callback) { onValueChanged = callback; return this; }

        /// <summary>
        /// Use DOPunchScale (default). Always resets to base scale before/after each punch.
        /// </summary>
        public IntCounterPunchBuilder Punch(Transform target, float strength = 0.5f, float punchDur = 0.2f, int vibrato = 7, float elasticity = 0.6f)
        {
            punchTarget = target;
            punchStrength = strength;
            punchDuration = punchDur;
            punchVibrato = vibrato;
            punchElasticity = elasticity;
            usePunchTween = true;
            return this;
        }

        /// <summary>
        /// Alternative to Punch: a simple scale up/down (yoyo) which is mathematically guaranteed to end at base scale.
        /// </summary>
        public IntCounterPunchBuilder Yoyo(Transform target, float strength = 0.1f, float upDur = 0.08f, float downDur = 0.12f)
        {
            punchTarget = target;
            punchStrength = strength;
            // Reuse fields: up = punchDuration, down = elasticity (we’ll store separately)
            punchDuration = upDur;
            punchElasticity = downDur;
            usePunchTween = false;
            return this;
        }

        public IntCounterPunchBuilder Id(string id) { tweenId = id; return this; }
        public IntCounterPunchBuilder AutoKill(bool value) { autoKill = value; return this; }
        public IntCounterPunchBuilder BaseScale(Vector3 baseScale) { explicitBaseScale = baseScale; return this; }

        public Tween Start()
        {
            int currentValue = startValue;
            int lastDisplayedValue = startValue;

            if (!string.IsNullOrEmpty(tweenId))
                DOTween.Kill(tweenId);

            Vector3 capturedBaseScale = Vector3.one;
            bool baseScaleCaptured = false;

            string punchId = string.IsNullOrEmpty(tweenId) ? null : $"{tweenId}_Punch";

            return DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                int intValue = currentValue;

                if (intValue != lastDisplayedValue)
                {
                    lastDisplayedValue = intValue;
                    onValueChanged?.Invoke(intValue);

                    if (punchTarget != null)
                    {
                        // Capture baseScale once (or use explicit override if provided)
                        if (explicitBaseScale.HasValue)
                        {
                            capturedBaseScale = explicitBaseScale.Value;
                            baseScaleCaptured = true;
                        }
                        else if (!baseScaleCaptured)
                        {
                            capturedBaseScale = punchTarget.localScale; // true original scale
                            baseScaleCaptured = true;
                        }

                        // Stop any previous effect and hard reset to base before starting
                        if (!string.IsNullOrEmpty(punchId)) DOTween.Kill(punchId);
                        punchTarget.localScale = capturedBaseScale;

                        if (usePunchTween)
                        {
                            // Scale-relative punch: delta proportional to base scale
                            Vector3 delta = capturedBaseScale * punchStrength;

                            punchTarget
                                .DOPunchScale(delta, punchDuration, punchVibrato, punchElasticity)
                                .SetId(punchId)
                                .OnKill(() =>
                                {
                                    // If killed mid-punch, force scale back to base
                                    if (punchTarget != null) punchTarget.localScale = capturedBaseScale;
                                })
                                .OnComplete(() =>
                                {
                                    // Ensure exact base at the end (no drift)
                                    if (punchTarget != null) punchTarget.localScale = capturedBaseScale;
                                });
                        }
                        else
                        {
                            // Yoyo style: guaranteed base at the end without any drift
                            float upDur = punchDuration;
                            float downDur = punchElasticity;
                            Sequence s = DOTween.Sequence().SetId(punchId);
                            s.Append(punchTarget
                                    .DOScale(capturedBaseScale * (1f + punchStrength), upDur)
                                    .SetEase(DG.Tweening.Ease.OutQuad));
                            s.Append(punchTarget
                                    .DOScale(capturedBaseScale, downDur)
                                    .SetEase(DG.Tweening.Ease.OutBack));
                            s.OnKill(() =>
                            {
                                if (punchTarget != null) punchTarget.localScale = capturedBaseScale;
                            });
                        }
                    }
                }
            }, endValue, duration)
            .SetEase(ease)
            .SetId(tweenId)
            .SetAutoKill(autoKill);
        }
    }

    public static IntCounterPunchBuilder DOIntCounterPunch(this MonoBehaviour _) => new IntCounterPunchBuilder();
}
#endif