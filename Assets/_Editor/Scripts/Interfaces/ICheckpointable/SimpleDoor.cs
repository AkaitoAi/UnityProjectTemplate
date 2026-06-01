using UnityEngine;
using UnityEngine.Events;

#if AkaitoAi_Dirveable
using Driveable;
#endif

public class SimpleDoor : MonoBehaviour, ICheckpointable
{
#if AkaitoAi_Dirveable
    [SerializeField] private bool driveableCanInteract = false;
#endif

    public UnityEvent OnEnterEvent, OnExitEvent;

    [ContextMenu("Enter")]
    public void OnEnterCheckpoint(GameObject collector)
    {
#if AkaitoAi_Dirveable
        if (!driveableCanInteract)
        {
            IDriveable driveable = collector.GetComponentInChildren<IDriveable>()
                    ?? collector.GetComponentInParent<IDriveable>();

            if (driveable == null)
            {
                OnEnterEvent?.Invoke();

                return;
            }

            return;
        }
#endif

        OnEnterEvent?.Invoke();
    }

    [ContextMenu("Exit")]
    public void OnExitCheckpoint(GameObject collector)
    {
#if AkaitoAi_Dirveable
        if (!driveableCanInteract)
        {
            IDriveable driveable = collector.GetComponentInChildren<IDriveable>()
                    ?? collector.GetComponentInParent<IDriveable>();

            if (driveable == null)
            {
                OnExitEvent?.Invoke();

                return;
            }

            return;
        }
#endif

        OnExitEvent?.Invoke();
    }
}
