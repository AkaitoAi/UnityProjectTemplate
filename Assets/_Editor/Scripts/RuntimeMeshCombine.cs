using UnityEngine;
using UnityEngine.Events;

namespace AkaitoAi
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RuntimeMeshCombine : MonoBehaviour
    {
        public UnityEvent OnStartEvent;
        
        private void Start()
        {

            // If you want to combine mesh on start then
            // reference Combine function in the inspector to the OnStartEvent
            OnStartEvent?.Invoke();
        }

        public void Combine()
        {
            CombineInstance[] combine
                = new CombineInstance[transform.childCount];

            int index = 0;
            foreach (Transform child in gameObject.transform)
            {
                //MeshFilter filter = child.GetComponent<MeshFilter>();

                if (!child.TryGetComponent<MeshFilter>(out MeshFilter childFilter))
                    index++;
                else
                {
                    combine[index].mesh = childFilter.sharedMesh;
                    combine[index].transform = child.localToWorldMatrix;
                    child.gameObject.SetActive(false);
                    index++;
                }
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);

            if (transform.TryGetComponent<MeshFilter>(out MeshFilter thisFilter))
                thisFilter.sharedMesh = mesh;

            transform.gameObject.SetActive(true);
        }
    }
}
