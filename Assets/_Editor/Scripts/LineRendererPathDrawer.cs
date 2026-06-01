using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AkaitoAi
{
    public class LineRendererPathDrawer : MonoBehaviour
    {
        public Transform player;

        [SerializeField] private bool blinkPath = false;
        [SerializeField] private bool disableOnEnd = false;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private float PathHeightOffset = 0f;
        [SerializeField] private float distance = 2f;

        [SerializeField] private Color rayColor = Color.white;
        public List<Transform> points_objs = new List<Transform>();
        private Transform[] points;

        [Header("Material Lerp")]
        [SerializeField] private float pingPongDuration = 1f;
        private Material material;

        internal float lerp;
        internal int updateRate = 3;

        private void OnEnable()
        {
            SetupUpdateRate();

            lineRenderer.positionCount = points_objs.Count;

            material = lineRenderer.material;

            StartCoroutine(MaterialBlinker());
        }

        public void SetUpLine(List<Transform> points) //To Setup waypoints from script
        {
            lineRenderer.positionCount = points.Count;
            this.points_objs = points;
        }

        private void Update()
        {
            if (Time.frameCount % updateRate != 0) return;

            for (int i = 0; i < points_objs.Count; i++)
            {
                ReachPoint(i);

                lineRenderer.SetPosition(i, points_objs[i].position + Vector3.up * PathHeightOffset);
            }
        }

        private void ReachPoint(int _pointIndex)
        {
            float dist = Vector3.Distance(player.position, points_objs[_pointIndex].position);

            if (dist <= distance)
            {
                points_objs[_pointIndex].transform.position = player.position;

                for (int i = 0; i < _pointIndex; i++) points_objs[i].transform.position = player.position;

                if (disableOnEnd && _pointIndex == points_objs.Count - 1) gameObject.SetActive(false);
            }
        }

        IEnumerator MaterialBlinker()
        {
            while (blinkPath)
            {
                lerp = Mathf.PingPong(Time.time, pingPongDuration) / pingPongDuration;

                material.color = Color.Lerp(Color.white, Color.grey, lerp);

                yield return null;
            }
        }

        private void SetupUpdateRate()
        {
            switch (PlayerPrefs.GetInt("QualitySetting"))
            {
                case 0: { updateRate = 3; break; }

                case 1: { updateRate = 2; break; }

                case 2: { updateRate = 1; break; }

                default: break;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = rayColor;
            points = GetComponentsInChildren<Transform>();
            points_objs.Clear();
            foreach (Transform point_obj in points)
            {
                if (point_obj != this.transform)
                    points_objs.Add(point_obj);
            }
            for (int i = 0; i < points_objs.Count; i++)
            {
                Vector3 position = points_objs[i].position;
                if (i > 0)
                {
                    Vector3 previous = points_objs[i - 1].position;
                    Gizmos.DrawLine(previous, position);
                    Gizmos.DrawWireSphere(position, 0.3f);
                }
            }
        }
    }
}
