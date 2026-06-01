#if Cinemachine
using AkaitoAi.Extensions;
using Cinemachine;
using UnityEngine;

namespace AkaitoAi.Cinemachine
{
    public class CinemachineUtility : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain cBrain;

        private void Start()
        {
            cBrain.GetOrAddComponent<CinemachineUtility>();
        }
        public void ChangeBlendTime(float _time) => cBrain.m_DefaultBlend.m_Time = _time;
    }
}
#endif
