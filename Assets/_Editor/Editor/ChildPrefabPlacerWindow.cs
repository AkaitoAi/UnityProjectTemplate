using UnityEditor;
using UnityEngine;

namespace AkaitoAi
{
    public class ChildPrefabPlacerWindow : EditorWindow
    {
        private GameObject prefabToPlace;
        private Vector3 customPosition = Vector3.zero;
        private Vector3 customRotation = Vector3.zero;
        private Vector3 customScale = Vector3.one;

        [MenuItem("AkaitoAi/Tools/GameObject/Child Prefab Placer")]
        public static void ShowWindow()
        {
            GetWindow<ChildPrefabPlacerWindow>("Child Prefab Placer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Prefab Placer Tool", EditorStyles.boldLabel);

            prefabToPlace = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToPlace, typeof(GameObject), false);

            if (prefabToPlace == null)
            {
                EditorGUILayout.HelpBox("Please assign a prefab.", MessageType.Warning);
            }
            else
            {
                GUILayout.Label("Transform Settings", EditorStyles.boldLabel);
                customPosition = EditorGUILayout.Vector3Field("Position", customPosition);
                customRotation = EditorGUILayout.Vector3Field("Rotation", customRotation);
                customScale = EditorGUILayout.Vector3Field("Scale", customScale);

                EditorGUI.BeginDisabledGroup(prefabToPlace == null || Selection.gameObjects.Length == 0);
                if (GUILayout.Button("Place Prefab as Child"))
                {
                    PlacePrefabAsChild();
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void PlacePrefabAsChild()
        {
            if (prefabToPlace == null)
            {
                Debug.LogWarning("No prefab selected.");
                return;
            }

            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No GameObject selected in the hierarchy.");
                return;
            }

            foreach (GameObject selectedObject in selectedObjects)
            {
                GameObject newPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
                Undo.RegisterCreatedObjectUndo(newPrefabInstance, "Place Prefab");

                newPrefabInstance.transform.SetParent(selectedObject.transform);
                newPrefabInstance.transform.localPosition = customPosition;
                newPrefabInstance.transform.localEulerAngles = customRotation;
                newPrefabInstance.transform.localScale = customScale;
            }

            Debug.Log($"{selectedObjects.Length} prefabs placed as children.");
        }
    }
}

