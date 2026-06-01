using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AkaitoAi
{
    public class RigidbodyColliderToggler : MonoBehaviour
    {
        [SerializeField] private bool kinematicState = true;
        [SerializeField] private bool colliderState = true;

        void Reset()
        {
            UpdateRigidbodyStates();
            UpdateColliderStates();
        }

        public void ToggleRigidbodyKinematic()
        {
            kinematicState = !kinematicState;
            UpdateRigidbodyStates();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void ToggleColliders()
        {
            colliderState = !colliderState;
            UpdateColliderStates();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void RemoveJoints()
        {
            Joint[] joints = GetComponentsInChildren<Joint>();
            foreach (Joint joint in joints)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(joint);
                }
            }
        }

        public void RemoveColliders()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(col);
                }
            }
        }

        public void RemoveRigidbodies()
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(rb);
                }
            }
        }

        private void UpdateRigidbodyStates()
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = kinematicState;
            }
        }

        private void UpdateColliderStates()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = colliderState;
            }
        }

#if UNITY_EDITOR
    [CustomEditor(typeof(RigidbodyColliderToggler))]
    public class RigidbodyColliderTogglerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RigidbodyColliderToggler toggler = (RigidbodyColliderToggler)target;

            if (GUILayout.Button("Toggle Rigidbody Kinematic"))
            {
                toggler.ToggleRigidbodyKinematic();
            }

            if (GUILayout.Button("Toggle Colliders"))
            {
                toggler.ToggleColliders();
            }

            GUILayout.Space(10); // Add some spacing

            if (GUILayout.Button("Remove All Joints"))
            {
                if (EditorUtility.DisplayDialog("Confirm Removal",
                    "Are you sure you want to remove all joints from children?",
                    "Yes", "No"))
                {
                    toggler.RemoveJoints();
                }
            }

            if (GUILayout.Button("Remove All Colliders"))
            {
                if (EditorUtility.DisplayDialog("Confirm Removal",
                    "Are you sure you want to remove all colliders from children?",
                    "Yes", "No"))
                {
                    toggler.RemoveColliders();
                }
            }

            if (GUILayout.Button("Remove All Rigidbodies"))
            {
                if (EditorUtility.DisplayDialog("Confirm Removal",
                    "Are you sure you want to remove all rigidbodies from children?",
                    "Yes", "No"))
                {
                    toggler.RemoveRigidbodies();
                }
            }
        }
    }
#endif
    }
}