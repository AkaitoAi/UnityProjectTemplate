using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{

    [EditorTool("Fixed Step Movement Tool")]
    public class FixedStepMovementTool : EditorWindow
    {
        private string toolTip = "This tool will move GameObjects a fixed step instead of smooth moving.";

        public float step = 0;
        private bool enabled = false;
        private GameObject selectedObject;
        private Vector3 currentPosition;

        [MenuItem("AkaitoAi/Tools/GameObject/Fixed Step Movement Tool")]//Adds it to the toolbar: Tools -> Replace Materials Tool
                                                             //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(FixedStepMovementTool), false, "Fixed Step Movement Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip(); //Uncomment this to remove: ? button

            enabled = EditorGUILayout.Toggle(enabled);
            EditorGUI.BeginDisabledGroup(!enabled);
            step = EditorGUILayout.FloatField(new GUIContent("Step", "Step amount"), step);
            EditorGUI.EndDisabledGroup();
        }

        private void Update()
        {
            //if (enabled) {
            if (Selection.gameObjects.Length == 0)
            {
                selectedObject = null;
            }
            else if (selectedObject)
            {
                if (selectedObject != Selection.gameObjects[0])
                {
                    selectedObject = Selection.gameObjects[0];
                    currentPosition = selectedObject.transform.position;
                }
                else
                {
                    if (currentPosition != selectedObject.transform.position)
                    {
                        if (enabled)
                            MoveObjects();

                        currentPosition = selectedObject.transform.position;
                    }
                }
            }
            else
            {
                selectedObject = Selection.gameObjects[0];
            }
            //}
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void MoveObjects()
        {
            //Vector3 movement = currentPosition - selectedObject.transform.position;
            //Vector3 stepMovement = movement.normalized * step;

            //Debug.Log(movement + " " + stepMovement);
            //Go throught the Selected GameObjects
            foreach (GameObject obj in Selection.gameObjects)
            {
                //obj.transform.position -= movement + stepMovement;
                Vector3 position = obj.transform.position;
                obj.transform.position = new Vector3(Mathf.RoundToInt(position.x / step), Mathf.RoundToInt(position.y / step), Mathf.RoundToInt(position.z / step)) * step;
            }

        }
    }
}