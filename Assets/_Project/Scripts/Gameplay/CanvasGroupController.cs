#if DOTWEEN
using System.Collections.Generic;
using UnityEngine;
using AkaitoAi.GameBase;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Tween currentTween;

    [SerializeField] private float fadeDuration = 0.3f;

    EventBinding<OnGameplayScreen> gameplayScreenEventBinding;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        gameplayScreenEventBinding = new EventBinding<OnGameplayScreen>((screen) => {

            HashSet<GameplayScreens> allowedScreens = new HashSet<GameplayScreens>
            { GameplayScreens.Win, GameplayScreens.Fail, GameplayScreens.CutScene, GameplayScreens.None};

            if (allowedScreens.Contains(screen.screen))
            {
                FadeOut();
            }
            else
            {
                FadeIn();
            }

        });
        EventBus<OnGameplayScreen>.Register(gameplayScreenEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnGameplayScreen>.Deregister(gameplayScreenEventBinding);
    }

    public void FadeIn(float duration = -1f)
    {
        StopCurrentTween();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float fadeTime = duration > 0f ? duration : fadeDuration;
        currentTween = canvasGroup.DOFade(1f, fadeTime);
    }

    public void FadeOut(float duration = -1f)
    {
        StopCurrentTween();

        float fadeTime = duration > 0f ? duration : fadeDuration;
        currentTween = canvasGroup.DOFade(0f, fadeTime).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    public void ToggleFade(float duration = -1f)
    {
        if (canvasGroup.alpha > 0.5f)
            FadeOut(duration);
        else
            FadeIn(duration);
    }

    private void StopCurrentTween()
    {
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();
    }
}
#endif