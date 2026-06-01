using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
namespace AkaitoAi
{
    [EditorTool("Rename Tool")]
    public class RenameTool : EditorWindow
    {
        private string toolTip = "With this tool you rename Selected GameObjects or Replace a specific part of the name.";

        private string findText;
        private string replaceText;
        private bool selected = true;

        private string renameText;

        [MenuItem("AkaitoAi/Tools/GameObject/Rename Tool")]
        public static void ShowWindow()
        {
            GetWindow(typeof(RenameTool), false, "Rename GameObjects Tool");
        }

        private void OnGUI()
        {
            ToolTip();
            ReplaceTextWindow();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            RenameSelectedObjectsWindow();

        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void ReplaceTextWindow()
        {
            GUILayout.Label("Replace specific text (BE CAREFULL)", EditorStyles.boldLabel);

            findText = EditorGUILayout.TextField(new GUIContent("Find...", "String to replace"), findText);
            replaceText = EditorGUILayout.TextField(new GUIContent("Replace...", "String to replace with"), replaceText);
            selected = EditorGUILayout.Toggle(new GUIContent("Selected GameObjects only", "Checked = Replace string of selected GameObjects only\n\nUnchecked = Replace string of all GameObjects"), selected);

            if (GUILayout.Button("Replace"))
            {
                if (selected)
                {
                    ReplaceTextGameObjects(Selection.gameObjects);
                }
                else
                {
                    ReplaceTextGameObjects(GameObject.FindObjectsOfType<GameObject>());
                }
            }
        }

        private void RenameSelectedObjectsWindow()
        {
            GUILayout.Label("Rename selected GameObject(s)", EditorStyles.boldLabel);
            renameText = EditorGUILayout.TextField(new GUIContent("GameObject Name", "Name for the selected GameObjects"), renameText);

            if (GUILayout.Button("Rename") && renameText != "")
            {
                RenameSelectedGameObjects();
            }
        }

        private void ReplaceTextGameObjects(GameObject[] objects)
        {
            foreach (GameObject obj in objects)
            {
                obj.name = obj.name.Replace(findText, replaceText);
            }
        }

        private void RenameSelectedGameObjects()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.name = renameText;
            }
        }
    }
}
