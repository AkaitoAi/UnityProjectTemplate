using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{

    [EditorTool("Randomize Material Tool")]
    public class RandomizeMaterialTool : EditorWindow
    {
        private string toolTip = "This tool will randomly choose a Material from given materials and apply it to selected GameObjects.";

        private int numberMaterials = 0;
        public Material[] materials;

        [MenuItem("AkaitoAi/Tools/Materials/Randomize Material Tool")]//Adds it to the toolbar: Tools -> Materials -> Replace Materials Tool
                                                                      //Opens window when tool is opened
        public static void ShowWindow()
        {
            GetWindow(typeof(RandomizeMaterialTool), false, "Randomize Material Tool");
        }

        //All gui magic happens here
        private void OnGUI()
        {
            ToolTip(); //Uncomment this to remove: ? button

            EditorGUI.BeginChangeCheck();
            numberMaterials = Mathf.Max(0, EditorGUILayout.IntField(new GUIContent("Number of Materials", "Number materials to randomly select"), numberMaterials));
            if (EditorGUI.EndChangeCheck())
            {
                Material[] temp = materials;
                materials = new Material[numberMaterials];
                for (int i = 0; i < materials.Length; ++i)
                {
                    if (i < temp.Length)
                        materials[i] = temp[i];
                    else
                        break;
                }
            }

            for (int i = 0; i < numberMaterials; ++i)
            {
                materials[i] = (Material)EditorGUILayout.ObjectField(new GUIContent("Material " + i, "Material that can be randomly chosen"), materials[i], typeof(Material), true);
            }

            if (GUILayout.Button("Randomize") && numberMaterials > 0) //When button is clicked, check if GameObjects are selected
            {
                RandomizeMaterials();
            }
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void RandomizeMaterials()
        {
            //Go throught the Selected GameObjects
            foreach (GameObject obj in Selection.gameObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                renderer.sharedMaterial = materials[Random.Range(0, materials.Length)];
            }
        }
    }
}
