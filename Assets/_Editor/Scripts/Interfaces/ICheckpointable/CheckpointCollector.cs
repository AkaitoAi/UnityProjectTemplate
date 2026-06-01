using UnityEngine;

public class CheckpointCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out ICheckpointable cb))
            return;

        cb.OnEnterCheckpoint(gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out ICheckpointable cb))
            return;

        cb.OnExitCheckpoint(gameObject);
    }
}
