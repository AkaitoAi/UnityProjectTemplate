using UnityEngine;
using UnityEngine.Events;

namespace AkaitoAi.Events
{
    public class UnityGameEventListener : MonoBehaviour
    {
        public UnityGameEventSO _event;
        public UnityEvent _response;

        private void OnEnable() =>
            _event.RegisterListener(this);

        private void OnDisable() =>
            _event.UnRegisterListener(this);

        public void OnEventRaised() =>
            _response?.Invoke();
    }
}
