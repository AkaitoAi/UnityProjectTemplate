using UnityEngine;

namespace AkaitoAi
{
    public class DrawGizmos : MonoBehaviour
    {
        [SerializeField] private Color g_Color = Color.white;
        [SerializeField] private float g_Radius = 1.0f;
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;

            Gizmos.color = g_Color;
            Gizmos.DrawSphere(new Vector3
                (transform.localPosition.x, transform.localPosition.y, transform.localPosition.z),
                g_Radius);
        }
    }
}
