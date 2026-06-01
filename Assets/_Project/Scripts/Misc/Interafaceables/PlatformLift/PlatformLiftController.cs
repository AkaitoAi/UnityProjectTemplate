using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AkaitoAi.GameBase;
#if DOTWEEN
using DG.Tweening;
#endif

public class PlatformLiftController : MonoBehaviour
{
    [SerializeField] private Vector3 moveOffset;
    [SerializeField] private float moveDuration = 2f;
#if DOTWEEN
    [SerializeField] private Ease easeType = Ease.InOutSine;
#endif

    [SerializeField] private GameObject[] entryTriggers;
    [SerializeField] private AudioSource liftAudioSource;

    private Vector3 originalPosition;
    private HashSet<GameObject> enteredTriggers = new HashSet<GameObject>();
    private bool isMoving = false;
#if DOTWEEN
    private Tween moveTween;
#endif

    internal bool isAtOffset = false;

    private void Start()
    {
        originalPosition = transform.position;
        SetTriggerStates();
    }

    public void NotifyTriggerEntered(GameObject trigger, Action onEnterAll = null, Action onComplete = null)
    {
        if (isMoving) return;
        if (!entryTriggers.Contains(trigger)) return;

        enteredTriggers.Add(trigger);

        if (enteredTriggers.Count != entryTriggers.Length)
            return;

        onEnterAll?.Invoke();

        StartLiftMovement(onComplete);
    }

    private void StartLiftMovement(Action onComplete = null)
    {
        if (isAtOffset) return;

        isMoving = true;

        GameManager.GetInstance().state = GameplayScreens.None;
        GameManager.GetInstance()?.UpdateGameplayState();

        Vector3 destination = isAtOffset ? originalPosition : originalPosition + moveOffset;
        isAtOffset = !isAtOffset;

        if (liftAudioSource != null) liftAudioSource.Play();

#if DOTWEEN
        moveTween = transform.DOMove(destination, moveDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                GameManager.GetInstance().state = GameplayScreens.Resume;
                GameManager.GetInstance()?.UpdateGameplayState();

                onComplete?.Invoke();

                if (liftAudioSource != null) liftAudioSource.Stop();

                SetTriggerStates();
                isMoving = false;
                enteredTriggers.Clear();
            });
#endif
    }

    private void ResetLift(Action onComplete = null)
    {
        if (!isAtOffset) return;

        isMoving = true;

        Vector3 destination = isAtOffset ? originalPosition : originalPosition + moveOffset;
        isAtOffset = !isAtOffset;

        if (liftAudioSource != null) liftAudioSource.Play();

#if DOTWEEN
        moveTween = transform.DOMove(destination, moveDuration)
            .SetEase(easeType)
            .OnComplete(() =>
            {
                onComplete?.Invoke();

                if (liftAudioSource != null) liftAudioSource.Stop();

                SetTriggerStates();
                isMoving = false;
                enteredTriggers.Clear();
            });
#endif
    }

    private void SetTriggerStates()
    {
        foreach (var trigger in entryTriggers)
        {
            if (trigger != null)
                trigger.SetActive(true);
        }
    }

    private void CanReset(bool c)
    {
        if (!c) return;

        ResetLift();
    }
}
