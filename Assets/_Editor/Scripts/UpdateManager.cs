using System;
using UnityEngine;

namespace AkaitoAi
{
    public class UpdateManager : MonoBehaviour
    {
        #region Singleton
        //private static UpdateManager instance;
        //public static UpdateManager Instance { get { if (instance == null) instance = GameObject.FindObjectOfType<UpdateManager>(); return instance; } }
        #endregion

        [Range(1, 10)][SerializeField] private int interval = 3;
        public int Interval { get { return interval; } set { interval = value; } }

        public static event Action OnUpdate;

        private void Update()
        {
            if (Time.frameCount % interval != 0) return;

            OnUpdate?.Invoke();
        }
    }
}
