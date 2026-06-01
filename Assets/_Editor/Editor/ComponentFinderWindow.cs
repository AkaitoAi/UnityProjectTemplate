using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkaitoAi
{
    public class ComponentFinderWindow : EditorWindow
    {
        private string componentName = "";
        private List<GameObject> foundObjects = new List<GameObject>();
        private HashSet<GameObject> exactMatchSet = new HashSet<GameObject>();
        private List<GameObject> partialMatchObjects = new List<GameObject>();

        [MenuItem("AkaitoAi/Tools/Component Finder")]
        public static void ShowWindow()
        {
            GetWindow<ComponentFinderWindow>("Component Finder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find GameObjects by Component", EditorStyles.boldLabel);

            componentName = EditorGUILayout.TextField("Component Name", componentName);

            if (GUILayout.Button("Find Components"))
            {
                FindComponents();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Exact Matches:", EditorStyles.boldLabel);

            if (foundObjects.Count == 0)
            {
                GUILayout.Label("No Object Found");
            }
            else
            {
                foreach (var obj in foundObjects)
                {
                    if (GUILayout.Button(obj.name))
                    {
                        Selection.activeGameObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("Partial Matches:", EditorStyles.boldLabel);

            if (partialMatchObjects.Count == 0)
            {
                GUILayout.Label("No Partial Matches Found");
            }
            else
            {
                foreach (var obj in partialMatchObjects)
                {
                    if (GUILayout.Button(obj.name))
                    {
                        Selection.activeGameObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
        }

        private void FindComponents()
        {
            foundObjects.Clear();
            partialMatchObjects.Clear();
            exactMatchSet.Clear();

            if (string.IsNullOrWhiteSpace(componentName))
            {
                Debug.LogWarning("Please enter a component name.");
                return;
            }

            string lowerComponentName = componentName.ToLower();
            Type exactType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));

            if (exactType != null && typeof(Component).IsAssignableFrom(exactType))
            {
                foreach (var obj in GameObject.FindObjectsOfType(exactType))
                {
                    GameObject go = ((Component)obj).gameObject;
                    foundObjects.Add(go);
                    exactMatchSet.Add(go);
                }
            }

            var matchingTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.Name.ToLower().Contains(lowerComponentName) && typeof(Component).IsAssignableFrom(t));

            foreach (var type in matchingTypes)
            {
                foreach (var obj in GameObject.FindObjectsOfType(type))
                {
                    GameObject go = ((Component)obj).gameObject;
                    if (!exactMatchSet.Contains(go))
                    {
                        partialMatchObjects.Add(go);
                    }
                }
            }
        }
    }
}
