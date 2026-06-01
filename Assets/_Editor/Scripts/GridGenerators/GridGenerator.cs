using UnityEngine;

namespace AkaitoAi.GridGenerator
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Transform prefab;
        [SerializeField] private int width, depth;
        [SerializeField] private float width_Space = .83f, depth_Space = .83f;
        private GameObject[] grids;

        internal Vector3 Space;
        internal bool gotGrid;
        internal int updateRate = 3;

        public GameObject[] Grids => grids;

        private void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
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

        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;

            for (int i = 0; i < width * depth; i++)
            {
                EvaluateGrid(i);

                Gizmos.DrawWireCube(transform.position + Space, new Vector3(1, 0, 1));
            }
        }

        private Vector3 EvaluateGrid(int i)
        {
            Space = new Vector3(width_Space + (width_Space * (i % width)), 0f, depth_Space + (depth_Space * (i / width)));

            return Space;
        }

        private void Update()
        {
            if (Time.frameCount % updateRate != 0) return;

            if (gotGrid) return;
            grids = GameObject.FindGameObjectsWithTag("Grid");
            gotGrid = true;
        }
    }
}
