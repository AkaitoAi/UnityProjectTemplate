using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectUIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ObjectRotator rotator;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rotator != null) rotator.BeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rotator != null) rotator.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rotator != null) rotator.EndDrag();
    }
}