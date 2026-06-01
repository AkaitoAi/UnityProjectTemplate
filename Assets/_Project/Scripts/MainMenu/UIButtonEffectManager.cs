#if DOTWEEN
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonEffectManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerPress != null)
        {
            eventData.pointerPress.transform.DOScale(Vector3.one * 1.1f, 0.1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerPress != null)
        {
            eventData.pointerPress.transform.DOScale(Vector3.one, 0.1f);
        }
    }
}
#endif
