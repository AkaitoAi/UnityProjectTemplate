using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AkaitoAi
{
    public class ComponentCopyWindow : EditorWindow
    {
        public GameObject source;
        public List<GameObject> targets = new List<GameObject>();
        public bool withValues = true; // Default: Copy with values
        private List<Component> allComponents = new List<Component>();
        private List<bool> includeComponents = new List<bool>();

        [MenuItem("AkaitoAi/Tools/Component Copier")]
        public static void ShowWindow()
        {
            GetWindow<ComponentCopyWindow>("Component Copier");
        }

        void OnGUI()
        {
            // Source selection
            source = EditorGUILayout.ObjectField("Source", source, typeof(GameObject), true) as GameObject;

            // Set targets from current selection
            if (GUILayout.Button("Set Targets from Selection"))
            {
                targets.Clear();
                foreach (var obj in Selection.gameObjects)
                {
                    if (obj != source)
                        targets.Add(obj);
                }
            }

            // Display targets
            EditorGUILayout.LabelField("Targets:");
            foreach (var t in targets)
            {
                EditorGUILayout.LabelField(t.name);
            }

            // Copy options
            withValues = EditorGUILayout.Toggle("Copy with values", withValues);

            // Components to copy
            if (source != null)
            {
                allComponents = new List<Component>(source.GetComponents<Component>());

                // Initialize includeComponents if necessary
                if (includeComponents.Count != allComponents.Count)
                {
                    includeComponents = new List<bool>(new bool[allComponents.Count]);
                    for (int i = 0; i < includeComponents.Count; i++)
                    {
                        includeComponents[i] = !(allComponents[i] is Transform); // Exclude Transform by default
                    }
                }

                EditorGUILayout.LabelField("Components to copy:");
                for (int i = 0; i < allComponents.Count; i++)
                {
                    includeComponents[i] = EditorGUILayout.Toggle(allComponents[i].GetType().Name, includeComponents[i]);
                }
            }

            // Copy button
            if (GUILayout.Button("Copy"))
            {
                PerformCopy();
            }
        }

        void PerformCopy()
        {
            if (source == null || targets.Count == 0) return;

            for (int t = 0; t < targets.Count; t++)
            {
                GameObject target = targets[t];
                for (int c = 0; c < allComponents.Count; c++)
                {
                    if (!includeComponents[c]) continue;

                    Component sourceComp = allComponents[c];
                    Type type = sourceComp.GetType();

                    // Check if target already has the component
                    Component targetComp = target.GetComponent(type);
                    if (targetComp == null)
                    {
                        targetComp = target.AddComponent(type);
                    }

                    if (withValues)
                    {
                        // Copy values from source to target
                        targetComp.CopyValuesFrom(sourceComp);
                    }
                }
            }
        }
    }

    public static class ComponentExtensions
    {
        public static Component CopyValuesFrom(this Component target, Component source)
        {
            if (target == null || source == null || target.GetType() != source.GetType()) return target;

            Type type = target.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;

            // Copy properties
            var pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(target, pinfo.GetValue(source, null), null);
                    }
                    catch { } // Ignore exceptions for non-writable properties
                }
            }

            // Copy fields
            var finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(target, finfo.GetValue(source));
            }

            return target;
        }
    }
}