using UnityEngine;
using UnityEngine.EventSystems;

namespace AkaitoAi
{
    public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private OnDragCameraRotater cameraRotator;

        [SerializeField] private Transform cameraTransform;
        private Transform defaultCamPosition;
        private Transform defaultCamRotation;

        private bool isPressing = false;

        void Start() => cameraRotator = cameraRotator.GetComponent<OnDragCameraRotater>();

        public void OnDrag(PointerEventData eventData)
        {
            cameraRotator.OnDrag(eventData);

            //TODO On Camera Start Rotate

            isPressing = true;
        }

        public void OnEndDrag(PointerEventData eventData) => isPressing = false;

        public void OnBackButton()
        {
            //TODO On Camera Stop Rotate

            cameraRotator.useCameraRotation = false;
            cameraRotator.ToggleCameras(false);
        }
    }
}