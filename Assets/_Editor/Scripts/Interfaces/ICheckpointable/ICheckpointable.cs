using UnityEngine;
using UnityEngine.Events;

#if DOTWEEN
using DG.Tweening;
#endif

public interface ICheckpointable
{
    public void OnEnterCheckpoint(GameObject collector);
    public void OnExitCheckpoint(GameObject collector);
}

public interface IDoor
{
    void Enter(Door door);
    void Exit(Door door);
}

[System.Serializable]
public struct Door
{
    public Transform _door;
    public Vector3 _to;
    public Vector3 _from;
    public float _duration;

#if DOTWEEN
    public Ease _ease;
#endif

    public UnityEvent toStartedEvent, toCompletedEvent;
    public UnityEvent fromStartedEvent, fromCompletedEvent;
}

public abstract class DoorSO : ScriptableObject, IDoor
{
    public abstract void Enter(Door door);
    public abstract void Exit(Door door);
}