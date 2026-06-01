using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour, ICheckpointable
{
    [SerializeField] private Door[] _door;
    [SerializeField] private DoorSO _doorSO;

#if AkaitoAi_Dirveable
    [SerializeField] private bool driveableCanInteract = false;
#endif
    public UnityEvent OnEnterEvent, OnExitEvent;

    [ContextMenu("Enter Checkpoint")]
    public void OnEnterCheckpoint(GameObject collector)
    {
        OnEnterEvent?.Invoke();

#if AkaitoAi_Dirveable
        if (!driveableCanInteract)
        {
            IDriveable driveable = collector.GetComponentInChildren<IDriveable>()
                    ?? collector.GetComponentInParent<IDriveable>();

            if (driveable == null)
            {
                foreach (var door in _door)
                    _doorSO.Enter(door);

                return;
            }

            return;
        }
#endif

        foreach (var door in _door)
            _doorSO.Enter(door);
    }

    [ContextMenu("Exit Checkpoint")]
    public void OnExitCheckpoint(GameObject collector)
    {
        OnExitEvent?.Invoke();

#if AkaitoAi_Dirveable
        if (!driveableCanInteract)
        {
            IDriveable driveable = collector.GetComponentInChildren<IDriveable>()
                    ?? collector.GetComponentInParent<IDriveable>();

            if (driveable == null || !canInteract)
            {
                foreach (var door in _door)
                    _doorSO.Exit(door);

                return;
            }

            return;
        }
#endif

        foreach (var door in _door)
            _doorSO.Exit(door);
    }
}
