using UnityEngine;
using UnityEngine.Events;

#if DOTWEEN
using DG.Tweening;
#endif

public class LevelCompleteInteractable : MonoBehaviour, ITriggerEnterable
{
    [SerializeField] private Transform setTransformTo;
    [SerializeField] private float setTransformDuration = 1f;

    [Space(10)]
    public UnityEvent OnWinEvent;

    private bool isDead = false;

    public void TriggerEnter(Interactor interactor)
    {
        if (isDead) return;

        if (interactor.TryGetComponent(out HealthSystem health))
        {
            health._inCooldown = true;
            health.enabled = false;
        }

        ControllerBehaviour(interactor);

        if (setTransformTo != null)
            LerpToTransform(interactor);

        //EventBus<OnLevelWon>.Raise(new OnLevelWon());

        OnWinEvent?.Invoke();
    }

    private void LerpToTransform(Interactor interactor)
    {
#if DOTWEEN
        interactor.transform.DOMove(new Vector3(setTransformTo.position.x, interactor.transform.position.y, setTransformTo.position.z), setTransformDuration);
#endif
    }

    private void ControllerBehaviour(Interactor interactor)
    {

    }

    private void OnEnable()
    {
        HealthSystem.OnDeath += () => isDead = true;
    }

    private void OnDisable()
    {
        HealthSystem.OnDeath -= () => isDead = true;
    }
}
