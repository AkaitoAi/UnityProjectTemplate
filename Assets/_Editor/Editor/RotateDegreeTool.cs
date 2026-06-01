using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{

    [EditorTool("Rotate Degree Tool")]
    public class RotateDegreeTool : EditorWindow
    {
        private string toolTip = "This tool will make it easier to Rotate x amount of degrees.";

        public float degrees = 90f;

        [MenuItem("AkaitoAi/Tools/GameObject/Rotate Degree Tool")]//Adds it to the toolbar: Tools -> Replace Materials Tool
                                                       //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(RotateDegreeTool), false, "Rotate Degree Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip(); //Uncomment this to remove: ? button

            degrees = EditorGUILayout.FloatField(new GUIContent("Degrees", "Amount of degrees to rotate GameObject"), degrees);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("X")) //When button is clicked, check if GameObjects are selected
            {
                RotateDegrees("x");
            }
            else if (GUILayout.Button("Y")) //When button is clicked, check if GameObjects are selected
            {
                RotateDegrees("y");
            }
            else if (GUILayout.Button("Z")) //When button is clicked, check if GameObjects are selected
            {
                RotateDegrees("z");
            }
            GUILayout.EndHorizontal();
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void RotateDegrees(string axis)
        {
            //Go throught the Selected GameObjects
            foreach (GameObject obj in Selection.gameObjects)
            {
                obj.transform.RotateAround(obj.transform.position, new Vector3(axis == "x" ? 1 : 0, axis == "y" ? 1 : 0, axis == "z" ? 1 : 0), degrees);
            }
        }
    }
}
