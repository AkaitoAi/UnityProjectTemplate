using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

[CreateAssetMenu(menuName = "Door/Sliding Door", fileName = "SlidingDoor")]
public class SlidingDoorSO : DoorSO
{
    public override void Enter(Door door)
    {
#if DOTWEEN
        door._door.DOLocalMove(door._to, door._duration)
            .SetEase(door._ease).OnStart(() => door.toStartedEvent?.Invoke()).OnComplete(() => door.toCompletedEvent?.Invoke());
#endif
    }

    public override void Exit(Door door)
    {
#if DOTWEEN
        door._door.DOLocalMove(door._from, door._duration)
            .SetEase(door._ease).OnStart(() => door.fromStartedEvent?.Invoke()).OnComplete(() => door.fromCompletedEvent?.Invoke());
#endif
    }
}