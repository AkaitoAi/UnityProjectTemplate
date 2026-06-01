using UnityEngine;
using UnityEditor;

namespace AkaitoAi
{
    public class DeleteAndModifyChildrenEditor : EditorWindow
    {
        private Vector3 newPosition = Vector3.zero;
        private Vector3 newRotation = Vector3.zero;
        private Vector3 newScale = Vector3.one;

        [MenuItem("AkaitoAi/Tools/GameObject/Delete and Modify Children")]
        static void Init()
        {
            DeleteAndModifyChildrenEditor window = (DeleteAndModifyChildrenEditor)EditorWindow.GetWindow(typeof(DeleteAndModifyChildrenEditor), true, "Delete and Modify Children");
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("Modify Children Transforms", EditorStyles.boldLabel);

            newPosition = EditorGUILayout.Vector3Field("New Position", newPosition);
            newRotation = EditorGUILayout.Vector3Field("New Rotation (Euler Angles)", newRotation);
            newScale = EditorGUILayout.Vector3Field("New Scale", newScale);

            if (GUILayout.Button("Modify and Delete Children of Selected GameObject(s)"))
            {
                if (EditorUtility.DisplayDialog("Confirm Action", "Are you sure you want to modify and delete all children of the selected GameObject(s)?", "Yes", "No"))
                {
                    ModifyAndDeleteChildren();
                }
            }
        }

        void ModifyAndDeleteChildren()
        {
            Undo.RecordObjects(Selection.gameObjects, "Modify and Delete Children");

            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                Transform parentTransform = selectedObject.transform;
                int childCount = parentTransform.childCount;

                for (int i = childCount - 1; i >= 0; i--)
                {
                    Transform child = parentTransform.GetChild(i);
                    // Modify child's transform
                    child.localPosition = newPosition;
                    child.localEulerAngles = newRotation;
                    child.localScale = newScale;

                    // Optionally, you can log the transformation for debugging
                    Debug.Log($"Modified Child: {child.name}, Position: {newPosition}, Rotation: {newRotation}, Scale: {newScale}");

                    // Delete the child
                    DestroyImmediate(child.gameObject);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Children modified and deleted from selected GameObject(s).");
        }
    }
}
