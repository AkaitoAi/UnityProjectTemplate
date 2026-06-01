using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public class PlaceOnCollider : ScriptableObject
    {
        private static Vector3 positionOffset = Vector3.zero;
        private static Vector3 rotationOffset = Vector3.zero;

        [MenuItem("AkaitoAi/Tools/GameObject/Place Selected Object(s) on Collider with Offset")]
        static void PlaceSelectedObjectsOnCollider()
        {
            OffsetWindow.ShowWindow();
        }

        private static void PlaceObjects(Vector3 posOffset, Vector3 rotOffset)
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.LogWarning("No GameObject selected. Please select one or more GameObjects to place.");
                return;
            }

            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                if (selectedObject == null)
                {
                    Debug.LogWarning("One or more selected GameObjects are null. Skipping...");
                    continue;
                }

                // Register the action with the Undo system
                Undo.RecordObject(selectedObject.transform, "Place Object on Collider");

                Ray ray = new Ray(selectedObject.transform.position, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Move the selected object to the hit point
                    selectedObject.transform.position = hit.point;

                    // Adjust position based on the collider's bounds
                    Renderer objectRenderer = selectedObject.GetComponent<Renderer>();
                    if (objectRenderer != null)
                    {
                        Vector3 boundsOffset = selectedObject.transform.position - objectRenderer.bounds.center;
                        selectedObject.transform.position += boundsOffset;
                    }

                    // Apply the position offset only on non-zero axes
                    Vector3 finalPositionOffset = Vector3.zero;
                    if (posOffset.x != 0)
                    {
                        finalPositionOffset.x = posOffset.x;
                    }
                    if (posOffset.y != 0)
                    {
                        finalPositionOffset.y = posOffset.y;
                    }
                    if (posOffset.z != 0)
                    {
                        finalPositionOffset.z = posOffset.z;
                    }
                    selectedObject.transform.position += finalPositionOffset;

                    // Apply the rotation offset only on non-zero axes
                    Vector3 finalRotationOffset = Vector3.zero;
                    if (rotOffset.x != 0)
                    {
                        finalRotationOffset.x = rotOffset.x;
                    }
                    if (rotOffset.y != 0)
                    {
                        finalRotationOffset.y = rotOffset.y;
                    }
                    if (rotOffset.z != 0)
                    {
                        finalRotationOffset.z = rotOffset.z;
                    }
                    if (finalRotationOffset != Vector3.zero)
                    {
                        selectedObject.transform.rotation = Quaternion.Euler(finalRotationOffset) * selectedObject.transform.rotation;
                    }

                    Debug.Log($"Moved {selectedObject.name} to {hit.point} on {hit.collider.name} with offset.");
                }
                else
                {
                    Debug.LogWarning($"No collider found beneath the selected GameObject: {selectedObject.name}.");
                }
            }
        }

        public class OffsetWindow : EditorWindow
        {
            private Vector3 localPositionOffset = Vector3.zero;
            private Vector3 localRotationOffset = Vector3.zero;

            private void OnEnable()
            {
                // Load saved offsets
                localPositionOffset = EditorPrefs.HasKey("PlaceOnCollider_PositionOffset")
                    ? StringToVector3(EditorPrefs.GetString("PlaceOnCollider_PositionOffset"))
                    : Vector3.zero;

                localRotationOffset = EditorPrefs.HasKey("PlaceOnCollider_RotationOffset")
                    ? StringToVector3(EditorPrefs.GetString("PlaceOnCollider_RotationOffset"))
                    : Vector3.zero;
            }

            private void OnGUI()
            {
                GUILayout.Label("Position Offset", EditorStyles.boldLabel);
                localPositionOffset = EditorGUILayout.Vector3Field("Position Offset", localPositionOffset);

                GUILayout.Label("Rotation Offset", EditorStyles.boldLabel);
                localRotationOffset = EditorGUILayout.Vector3Field("Rotation Offset", localRotationOffset);

                if (GUILayout.Button("Apply Offsets"))
                {
                    // Save offsets
                    EditorPrefs.SetString("PlaceOnCollider_PositionOffset", Vector3ToString(localPositionOffset));
                    EditorPrefs.SetString("PlaceOnCollider_RotationOffset", Vector3ToString(localRotationOffset));

                    // Close the window
                    //Close();

                    // Apply offsets to selected objects
                    PlaceObjects(localPositionOffset, localRotationOffset);
                }

                if (GUILayout.Button("Reset Offsets"))
                {
                    localPositionOffset = Vector3.zero;
                    localRotationOffset = Vector3.zero;

                    EditorPrefs.SetString("PlaceOnCollider_PositionOffset", Vector3ToString(localPositionOffset));
                    EditorPrefs.SetString("PlaceOnCollider_RotationOffset", Vector3ToString(localRotationOffset));
                }
            }

            public static void ShowWindow()
            {
                var window = GetWindow<OffsetWindow>("Set Offsets");
                window.Show();
            }

            private static string Vector3ToString(Vector3 vector)
            {
                return $"{vector.x},{vector.y},{vector.z}";
            }

            private static Vector3 StringToVector3(string sVector)
            {
                string[] values = sVector.Split(',');
                if (values.Length != 3)
                    return Vector3.zero;

                if (float.TryParse(values[0], out float x) &&
                    float.TryParse(values[1], out float y) &&
                    float.TryParse(values[2], out float z))
                {
                    return new Vector3(x, y, z);
                }
                return Vector3.zero;
            }
        }
    }
}
