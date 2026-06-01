using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

namespace AkaitoAi
{
    [EditorTool("Object Placement")]
    public class ObjectPlacementTool : EditorWindow
    {
        private string toolTip = "With this tool you can randomly place objects or place objects in a grid.";

        public GameObject prefab; //Prefab to spawn
        private bool inGrid = true; //Grid placement mode
        private bool random = false; //Random placement mode

        private Vector3Int numberObjectsGrid = new Vector3Int(1, 1, 1);
        private Vector3 offsetDistanceGrid;
        private Vector3 centerGrid;

        private int numberObjects = 1;
        private Bounds bounds;

        private bool randomRotation = false;
        private bool[] randomAxis = new bool[] { true, true, true };

        [MenuItem("AkaitoAi/Tools/GameObject/Object Placement Tool")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ObjectPlacementTool), false, "Place GameObjects Tool");
        }

        private void OnGUI()
        {
            ToolTip();

            prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "Prefab to spawn"), prefab, typeof(GameObject), true);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GridOptionsGUI();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            RandomOptionsGUI();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            RandomRotationGUI();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Place Objects") && prefab != null)
            {
                if (inGrid)
                    PlaceObjectsInGrid();
                else if (random)
                    PlaceObjectsRandomly();
            }
        }

        private void ToolTip()
        {
            GUILayout.Button(new GUIContent("?", toolTip), GUILayout.Width(20));
            //GUILayout.Label(GUI.tooltip);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void GridOptionsGUI()
        {
            inGrid = EditorGUILayout.BeginToggleGroup("Grid Mode", !random);
            //inGrid = EditorGUILayout.Toggle("In grid", true);

            //if (inGrid)
            {
                EditorGUI.BeginDisabledGroup(!inGrid);

                EditorGUI.BeginChangeCheck();
                numberObjectsGrid = EditorGUILayout.Vector3IntField("Number Objects", numberObjectsGrid);
                if (EditorGUI.EndChangeCheck())//Check if numbers are bigger then 0
                {
                    numberObjectsGrid.x = Mathf.Max(1, numberObjectsGrid.x);
                    numberObjectsGrid.y = Mathf.Max(1, numberObjectsGrid.y);
                    numberObjectsGrid.z = Mathf.Max(1, numberObjectsGrid.z);
                }

                offsetDistanceGrid = EditorGUILayout.Vector3Field(new GUIContent("Offset", "Distance between the GameObjects"), offsetDistanceGrid);
                centerGrid = EditorGUILayout.Vector3Field(new GUIContent("Center", "World position of the center of the grid"), centerGrid);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndToggleGroup();
        }

        private void RandomOptionsGUI()
        {
            random = EditorGUILayout.BeginToggleGroup("Random Mode", !inGrid);
            EditorGUI.BeginDisabledGroup(!random);
            EditorGUI.BeginChangeCheck();
            numberObjects = EditorGUILayout.IntField("Number Objects", numberObjects);
            if (EditorGUI.EndChangeCheck())
            {
                numberObjects = Mathf.Max(1, numberObjects);
            }
            bounds = EditorGUILayout.BoundsField(new GUIContent("Bounds", "Area where GameObjects will spawn in.\nExtends will be half the size the area should be. For example with x = 2 the width of the area will be 4"), bounds);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndToggleGroup();
        }

        private void RandomRotationGUI()
        {
            //GUILayout.Label("Random Rotation");
            randomRotation = EditorGUILayout.BeginToggleGroup("Random Rotation", randomRotation);
            EditorGUI.BeginDisabledGroup(!randomRotation);
            randomAxis[0] = EditorGUILayout.Toggle("X", randomAxis[0]);
            randomAxis[1] = EditorGUILayout.Toggle("Y", randomAxis[1]);
            randomAxis[2] = EditorGUILayout.Toggle("Z", randomAxis[2]);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndToggleGroup();
        }

        private void PlaceObjectsRandomly()
        {
            Transform parent = new GameObject().transform;
            parent.gameObject.name = "PlacedObjects";

            for (int i = 0; i < numberObjects; ++i)
            {
                //Instantiate(prefab, GetRandomPointInBounds(), GetRandomRotation(), parent);
                Transform obj = ((GameObject)PrefabUtility.InstantiatePrefab(prefab)).transform;
                obj.position = GetRandomPointInBounds();
                obj.rotation = GetRandomRotation();
                obj.SetParent(parent);
            }

            parent.position = bounds.center;
        }

        private void PlaceObjectsInGrid()
        {
            Transform parent = new GameObject().transform;
            parent.gameObject.name = "PlacedObjects";

            Vector3 offset = (Vector3.Scale((Vector3)numberObjectsGrid, offsetDistanceGrid) / 2f) - (offsetDistanceGrid / 2f);

            for (int x = 0; x < numberObjectsGrid.x; ++x)
            {
                for (int y = 0; y < numberObjectsGrid.y; ++y)
                {
                    for (int z = 0; z < numberObjectsGrid.z; ++z)
                    {
                        //Instantiate(prefab, Vector3.Scale(new Vector3(x, y, z), offsetDistanceGrid) - offset, GetRandomRotation(), parent);
                        Transform obj = ((GameObject)PrefabUtility.InstantiatePrefab(prefab)).transform;
                        obj.position = Vector3.Scale(new Vector3(x, y, z), offsetDistanceGrid) - offset;
                        obj.rotation = GetRandomRotation();
                        obj.SetParent(parent);
                    }
                }
            }

            parent.position = centerGrid;
        }

        private Vector3 GetRandomPointInBounds()
        {
            return new Vector3(Random.Range(-bounds.extents.x, bounds.extents.x), Random.Range(-bounds.extents.y, bounds.extents.y), Random.Range(-bounds.extents.z, bounds.extents.z));
        }

        private Quaternion GetRandomRotation()
        {
            if (randomRotation)
            {
                Vector3 rotation = new Vector3();
                rotation.x = randomAxis[0] ? Random.Range(0f, 360f) : 0;
                rotation.y = randomAxis[1] ? Random.Range(0f, 360f) : 0;
                rotation.z = randomAxis[2] ? Random.Range(0f, 360f) : 0;

                return Quaternion.Euler(rotation);
            }
            else
            {
                return Quaternion.identity;
            }
        }
    }
}
