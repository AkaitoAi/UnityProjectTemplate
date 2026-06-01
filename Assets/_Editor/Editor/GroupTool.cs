using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{
    [EditorTool("Group Tool")]
    public class GroupTool : EditorWindow
    {
        private string toolTip = "This tool takes the selected GameObjects and group them under a new GameObject.";

        public string groupName = "Group";

        [MenuItem("AkaitoAi/Tools/GameObject/Group Tool")]//Adds it to the toolbar: Tools -> Replace Materials Tool
                                               //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(GroupTool), false, "Group Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip(); //Uncomment this to remove: ? button

            groupName = EditorGUILayout.TextField(new GUIContent("Group Name", "Name of the group"), groupName);

            if (GUILayout.Button("Group") && Selection.gameObjects.Length > 0) //When button is clicked, check if GameObjects are selected
            {
                GroupSelectedGameObjects();
            }
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void GroupSelectedGameObjects()
        {
            Transform parent = new GameObject().transform;
            parent.gameObject.name = groupName == "" ? "Group" : groupName;

            //Go throught the Selected GameObjects
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.transform.SetParent(parent);
            }
        }
    }
}
