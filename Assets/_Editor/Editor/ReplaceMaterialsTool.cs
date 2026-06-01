using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{

    [EditorTool("Replace Materials Tool")]
    public class ReplaceMaterialsTool : EditorWindow
    {
        private string toolTip = "It will replace the materials of Game Objects that matches the Material input in the Find... field with the Material from the Replace... field.";

        public Material findMaterial; //Material that will be replaced
        public Material replaceMaterial; //Replacement material
        private bool selected = true; //true = replacing materials of selected GameObjects only, false = replacing materials of all GameObjects

        [MenuItem("AkaitoAi/Tools/Materials/Replace Materials Tool")]//Adds it to the toolbar: Tools -> Materials -> Replace Materials Tool
                                                                     //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(ReplaceMaterialsTool), false, "Replace Materials Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip();

            findMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Find...", "Material to replace"), findMaterial, typeof(Material), true); //Get Material from user
            replaceMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Replace...", "Material to replace with"), replaceMaterial, typeof(Material), true); //Get Material from user
            selected = EditorGUILayout.Toggle(new GUIContent("Selected GameObjects only", "Checked = replace Material of selected GameObjects only\n\nUnchecked = replace Material of all GameObjects"), selected);

            if (GUILayout.Button("Replace") && findMaterial != null && replaceMaterial != null) //When button is clicked, check if materials aren't null
            {
                if (selected)
                {
                    ReplaceMaterialsGameObjects(Selection.gameObjects); //Get selected GameObjects
                }
                else
                {
                    ReplaceMaterialsGameObjects(GameObject.FindObjectsOfType<GameObject>()); //Get all the GameObjects
                }
            }
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        //Replace Materials Magic
        private void ReplaceMaterialsGameObjects(GameObject[] objects)
        {
            //Go throught the inputed objects
            foreach (GameObject obj in objects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();//Get any renderer from the GameObjects
                if (renderer)
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; ++i) //Go throught all the Materials, some Renderers have multiple Materials
                    {
                        if (renderer.sharedMaterials[i] == findMaterial) //If Material matches the Find... Material
                        {
                            Material[] sharedMaterials = renderer.sharedMaterials; //Make copy of the Materials
                            sharedMaterials[i] = replaceMaterial; //Replace the material
                            renderer.sharedMaterials = sharedMaterials; //Assign the edited materials to the renderer
                        }
                    }
                }
            }
        }
    }
}
