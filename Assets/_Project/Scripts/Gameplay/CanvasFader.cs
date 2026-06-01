using AkaitoAi;
using AkaitoAi.Timeline;
using System;
using UnityEngine;
using UnityEngine.Events;
#if DOTWEEN
using DG.Tweening;
#endif

public class CanvasFader : MonoBehaviour
{
    private enum FadeState
    {
        None,
        FadingIn,
        FadingOut,
        FadingInOut
    }

    [GetComponent]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;
#if DOTWEEN
    [SerializeField] private Ease easeType = Ease.InOutQuad;
#endif

    [SerializeField] private UnityEvent onStartEvent;
    [SerializeField] private UnityEvent onFadeStartEvent;
    [SerializeField] private UnityEvent onFadeCompleteEvent;

#if DOTWEEN
    private Tween _fadeTween;
#endif
    private FadeState _state = FadeState.None;

    private void Start()
    {
        onStartEvent?.Invoke();
    }

    private void OnEnable()
    {
        TimelineCutsceneManager.OnFadeInOutScreenAction += FadeInOut;
        TimelineCutsceneManager.OnFadeInScreenAction += FadeIn;
        TimelineCutsceneManager.OnFadeOutScreenAction += FadeOut;
    }

    private void OnDisable()
    {
        TimelineCutsceneManager.OnFadeInOutScreenAction -= FadeInOut;
        TimelineCutsceneManager.OnFadeInScreenAction -= FadeIn;
        TimelineCutsceneManager.OnFadeOutScreenAction -= FadeOut;

        KillTween();
    }

    public void FadeInOut(float t)
    {
        if (_state == FadeState.FadingInOut) return;

        KillTween();
        _state = FadeState.FadingInOut;

        FadeIn(() => FadeOut(t * 0.5f), t * 0.5f);
    }

    public void FadeIn()
    {
        FadeIn(null, fadeDuration);
    }

    public void FadeIn(float duration = 1f)
    {
        FadeIn(null, duration);
    }

    public void FadeIn(Action onCompleteAction)
    {
        FadeIn(onCompleteAction, fadeDuration);
    }

    public void FadeIn(Action onCompleteAction, float duration = 1f)
    {
        if (canvasGroup == null) return;
        if (_state == FadeState.FadingIn) return;

        KillTween();
        _state = FadeState.FadingIn;

        onFadeStartEvent?.Invoke();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

#if DOTWEEN
        _fadeTween = canvasGroup
            .DOFade(1f, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                _state = FadeState.None;
                onFadeCompleteEvent?.Invoke();
                onCompleteAction?.Invoke();
            });
#endif
    }

    public void FadeOut()
    {
        FadeOut(null, fadeDuration);
    }

    public void FadeOut(float duration = 1f)
    {
        FadeOut(null, duration);
    }

    public void FadeOut(Action onCompleteAction)
    {
        FadeOut(onCompleteAction, fadeDuration);
    }

    public void FadeOut(Action onCompleteAction, float duration = 1f)
    {
        if (canvasGroup == null) return;
        if (_state == FadeState.FadingOut) return;

        KillTween();
        _state = FadeState.FadingOut;

        onFadeStartEvent?.Invoke();

#if DOTWEEN
        _fadeTween = canvasGroup
            .DOFade(0f, duration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                _state = FadeState.None;
                onFadeCompleteEvent?.Invoke();
                onCompleteAction?.Invoke();
            });
#endif
    }

    private void KillTween()
    {
#if DOTWEEN
        if (_fadeTween != null && _fadeTween.IsActive())
        {
            _fadeTween.Kill();
            _fadeTween = null;
        }
#endif

        _state = FadeState.None;
    }

    private void OnDestroy()
    {
        KillTween();
    }
}
