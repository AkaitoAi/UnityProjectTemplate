using UnityEngine;
using UnityEngine.EventSystems;

namespace AkaitoAi
{
    public class OnDragCameraRotater : MonoBehaviour
    {
        [SerializeField]
        private float _dragSensitivity = 3.0f;

        private float _rotationY;
        private float _rotationX;

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _distanceFromTarget = 3.0f;

        private Vector3 _currentRotation;
        private Vector3 _smoothVelocity = Vector3.zero;

        [SerializeField]
        private float _smoothTime = 0.2f;

        [SerializeField]
        private Vector2 _rotationXMinMax = new Vector2(-40, 40);

        [SerializeField]
        private float rotationSpeed = 5f;

        public bool useCameraRotation = false;
        public GameObject mainCamera;
        public GameObject dragCamera;

        private int interval = 1;
        private bool isPositive = true;
        private float recentDrag;

        public void OnDrag(PointerEventData pointerData)
        {
            float orbitX = pointerData.delta.x * _dragSensitivity;
            float orbitY = pointerData.delta.y * _dragSensitivity;

            _rotationY += orbitX;
            _rotationX += orbitY;

            recentDrag = _rotationY;

            if (_rotationY > recentDrag) isPositive = true;
            else if (_rotationY < recentDrag) isPositive = false;

            CameraRotation();

            useCameraRotation = true;
            ToggleCameras(true);
        }


        private void Update()
        {
            if (Time.frameCount % interval == 0)
            {
                if (!useCameraRotation) return;

                float orbitX = Time.deltaTime * rotationSpeed;

                if (isPositive)
                    _rotationY += orbitX;
                else if (!isPositive)
                    _rotationY -= orbitX;

                CameraRotation();
            }
        }

        private void CameraRotation()
        {
            // Apply clamping for x rotation 
            _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

            // Apply damping between rotation changes
            _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
            transform.localEulerAngles = _currentRotation;

            // Substract forward vector of the GameObject to point its forward vector to the target
            transform.position = _target.position - transform.forward * _distanceFromTarget;
        }

        public void ToggleCameras(bool _dragCameraState)
        {
            dragCamera.SetActive(_dragCameraState);
            mainCamera.SetActive(!_dragCameraState);
        }

    }
}
