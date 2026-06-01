using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

public class RotateDoorBehaviour : MonoBehaviour, ICheckpointable
{
    [SerializeField] private Transform _transform;

    [ContextMenu("Enter Checkpoint")]
    public void OnEnterCheckpoint(GameObject go)
    {
#if DOTWEEN
        _transform.DOLocalRotate(
            new Vector3(0f, 0f, 0f), 
            2.5f, 
            RotateMode.Fast
            ).
            SetEase(Ease.OutBack);
#endif
    }

    [ContextMenu("Exit Checkpoint")]
    public void OnExitCheckpoint(GameObject go)
    {
#if DOTWEEN
        _transform.DOLocalRotate(
            new Vector3(-90f, 0f, 0f),
            2.5f,
            RotateMode.Fast
            ).
            SetEase(Ease.OutBack);
#endif
    }
}
