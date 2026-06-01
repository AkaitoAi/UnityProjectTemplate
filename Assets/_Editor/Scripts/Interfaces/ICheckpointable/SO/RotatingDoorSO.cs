using System;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

[CreateAssetMenu(menuName = "Door/Rotating Door", fileName = "RotatingDoor")]
public class RotatingDoorSO : DoorSO
{
    public event Action OnEnterAction, OnExitAction;

    public override void Enter(Door door)
    {
        OnEnterAction?.Invoke();
#if DOTWEEN
        door._door.DOLocalRotate(door._to, door._duration)
            .SetEase(door._ease).OnStart(() => door.toStartedEvent?.Invoke()).OnComplete(() => door.toCompletedEvent?.Invoke());
#endif
    }

    public override void Exit(Door door)
    {
        OnExitAction?.Invoke();
#if DOTWEEN
        door._door.DOLocalRotate(door._from, door._duration)
            .SetEase(door._ease).OnStart(() => door.fromStartedEvent?.Invoke()).OnComplete(() => door.fromCompletedEvent?.Invoke());
#endif
    }
}