using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, ITriggerEnterable, ITriggerExitable
{
    [SerializeField] private UnityEvent OnInteractEvent;
    [SerializeField] private UnityGameEventSO OnInteractEventSO;
    
    [SerializeField] private UnityEvent OnInteractExitEvent;
    [SerializeField] private UnityGameEventSO OnInteractExitEventSO;

    public void TriggerEnter(Interactor interactor)
    {
        OnInteractEvent?.Invoke();
        OnInteractEventSO?.Raise();
    }

    public void TriggerExit(Interactor interactor)
    {
        OnInteractExitEvent?.Invoke();
        OnInteractExitEventSO?.Raise();
    }
}
