using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine.UI;

namespace AkaitoAi
{

    [EditorTool("Font Replace Tool")]
    public class FontReplaceTool : EditorWindow
    {
        private string toolTip = "This tool will replace font of selected or all GameObjects in the scene.";

        public Font findFont;
        public Font replaceFont;
        private bool selected = true;

        [MenuItem("AkaitoAi/Tools/UI/Font Replace Tool")]//Adds it to the toolbar: Tools -> UI -> Replace Fonts Tool
                                                         //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(FontReplaceTool), false, "Replace Font Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip(); //Uncomment this to remove: ? button

            findFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Find...", "The font to replace, leaving null will replace all fonts"), findFont, typeof(Font), true);
            replaceFont = (Font)EditorGUILayout.ObjectField(new GUIContent("Replace...", "The font to replace with"), replaceFont, typeof(Font), true);
            selected = EditorGUILayout.Toggle(new GUIContent("Selected GameObjects Only", "Checked = replace Font of selected GameObjects only\n\nUnchecked = replace Font of all GameObjects"), selected);

            if (GUILayout.Button("Replace") && replaceFont != null) //When button is clicked and check if Replacement Font is given
            {
                ReplaceFonts(selected ? Selection.gameObjects : GameObject.FindObjectsOfType<GameObject>());
            }
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void ReplaceFonts(GameObject[] gameObjects)
        {
            //Go throught the GameObjects
            foreach (GameObject obj in gameObjects)
            {
                if (obj.GetComponent<Text>())
                {
                    if ((findFont && obj.GetComponent<Text>()?.font == findFont) || findFont == null)
                        obj.GetComponent<Text>().font = replaceFont;
                }
            }
        }
    }
}