using UnityEngine;
using UnityEditor;

namespace AkaitoAi.GridGenerator
{
    public class GridGeneratorEditor : MonoBehaviour
    {
        public Transform prefab;
        public int width, depth;
        public float width_Space = 1f, depth_Space = 1f;

        internal Vector3 Space;

        private void Start() => Recreate();

        public void Recreate()
        {
            while (transform.childCount > 0)
                DestroyImmediate(transform.GetChild(0).gameObject);

            if (width_Space < Mathf.Epsilon || depth_Space < Mathf.Epsilon) return;

            int name = 0;

            for (int i = 0; i < width * depth; i++)
            {
                EvaluateGrid(i);
                Transform cell = Instantiate(prefab, transform.position + Space, prefab.rotation);
                cell.parent = transform;
                cell.name = "Cell " + name;
                name++;
            }
        }

        public void Destroy() => DestroyImmediate(this);

        private Vector3 EvaluateGrid(int i)
        {
            Space = new Vector3(width_Space + (width_Space * (i % width)), 0f, depth_Space + (depth_Space * (i / width)));

            return Space;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GridGeneratorEditor))]
    public class GridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (GridGeneratorEditor)target;

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck() || GUILayout.Button("Recreate"))
                grid.Recreate();

            if (GUILayout.Button("Delete"))
                grid.Destroy();
        }
    }
#endif
}
