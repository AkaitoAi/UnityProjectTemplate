using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public class ReplaceGameObject : EditorWindow
    {
        private GameObject[] selectedObjects;
        private GameObject replacementObject;
        private bool copyRotation;
        private bool copyScale;

        [MenuItem("AkaitoAi/Tools/GameObject/Quick Object Replacer")]
        public static void ShowWindow()
        {
            // Create a new window instance each time the menu item is clicked
            ReplaceGameObject window = CreateInstance<ReplaceGameObject>();
            window.titleContent = new GUIContent("Quick Object Replacer");
            window.maxSize = new Vector2(float.MaxValue, 200); // Increased height for preview
            window.minSize = new Vector2(250, 180);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(15);

            // Replacement object field
            replacementObject = (GameObject)EditorGUILayout.ObjectField("Replacement object", replacementObject, typeof(GameObject), true);

            // Preview of the replacement object
            if (replacementObject != null)
            {
                GUILayout.Space(5);
                Texture2D preview = AssetPreview.GetAssetPreview(replacementObject);
                if (preview == null)
                {
                    // Fallback to icon if preview is not available
                    preview = EditorGUIUtility.ObjectContent(replacementObject, typeof(GameObject)).image as Texture2D;
                }
                if (preview != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("No preview available", EditorStyles.centeredGreyMiniLabel);
                }
            }
            else
            {
                GUILayout.Label("No object selected", EditorStyles.centeredGreyMiniLabel);
            }

            GUILayout.Space(5);
            copyRotation = EditorGUILayout.Toggle("Copy Rotation", copyRotation);
            copyScale = EditorGUILayout.Toggle("Copy Scale", copyScale);

            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Replace", GUILayout.Width(75)))
            {
                Replace();
            }
            GUILayout.EndHorizontal();
        }

        private void Replace()
        {
            selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length == 0)
            {
                Debug.LogWarning("No GameObjects selected for replacement.");
                return;
            }

            if (replacementObject == null)
            {
                Debug.LogWarning("No replacement object assigned.");
                return;
            }

            string prefabType = PrefabUtility.GetPrefabAssetType(replacementObject).ToString();
            string instanceStatus = null;
            if (prefabType == "Regular" || prefabType == "Variant")
                instanceStatus = PrefabUtility.GetPrefabInstanceStatus(replacementObject).ToString();

            List<GameObject> newSelected = new List<GameObject>();

            foreach (GameObject gameObject in selectedObjects)
            {
                if (gameObject == replacementObject)
                {
                    newSelected.Add(gameObject);
                    continue;
                }

                bool hasParent = false;
                Transform parent = null;
                if (gameObject.transform.parent != null)
                {
                    hasParent = true;
                    parent = gameObject.transform.parent;
                }

                GameObject newGameObject = null;

                if (prefabType == "Regular" || prefabType == "Variant")
                {
                    if (instanceStatus == "Connected")
                    {
                        Object newPrefab = PrefabUtility.GetCorrespondingObjectFromSource(replacementObject);
                        newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);
                        PrefabUtility.SetPropertyModifications(newGameObject, PrefabUtility.GetPropertyModifications(replacementObject));
                        if (hasParent)
                            newGameObject.transform.parent = parent;
                    }
                    else
                    {
                        newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(replacementObject);
                        if (hasParent)
                            newGameObject.transform.parent = parent;
                    }
                }
                else
                {
                    newGameObject = Instantiate(replacementObject);
                    newGameObject.name = gameObject.name;
                    if (hasParent)
                        newGameObject.transform.parent = parent;
                }

                Undo.RegisterCreatedObjectUndo(newGameObject, "Created object");

                newGameObject.transform.position = gameObject.transform.position;
                if (copyRotation)
                    newGameObject.transform.rotation = gameObject.transform.rotation;
                if (copyScale)
                    newGameObject.transform.localScale = gameObject.transform.localScale;

                Undo.DestroyObjectImmediate(gameObject);
                newSelected.Add(newGameObject);
            }

            Selection.objects = newSelected.ToArray();

            string goString = (newSelected.Count > 1) ? " GameObjects have " : " GameObject has ";
            if (prefabType == "Regular" || prefabType == "Variant")
            {
                if (instanceStatus == "Connected")
                    prefabType = "Prefab Instance";
                else
                    prefabType = "Prefab";
            }
            else
                prefabType = "None";

            Debug.Log($"{newSelected.Count}{goString}been replaced with {prefabType}.");
        }
    }
}