using UnityEngine;

public class PlatformLiftInteractable : MonoBehaviour, ITriggerEnterable
{
    [SerializeField] private PlatformLiftController liftController;

    public void TriggerEnter(Interactor interactor)
    {
        if (liftController == null) return;

#if BCG_RCC
        if (!interactor.TryGetComponent(out RCC_CarControllerV3 controller)) return;
        liftController.NotifyTriggerEntered(gameObject, () => 
        {
            if (liftController.isAtOffset) return;

            controller.rigid.Sleep();
            controller.rigid.drag = 5f;
            controller.rigid.constraints = 
            RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }, () =>
        {
            controller.rigid.constraints = RigidbodyConstraints.None;
            //controller.Rigid.drag = controller.defaultDrag;
        });
        
#endif
    }
}
