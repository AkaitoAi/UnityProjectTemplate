using UnityEngine;

namespace AkaitoAi.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        private static bool m_applicationIsQuitting = false;

        public static T GetInstance()
        {
            if (m_applicationIsQuitting) { return null; }
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
        private void OnApplicationQuit()
        {
            m_applicationIsQuitting = true;
        }
    }
}