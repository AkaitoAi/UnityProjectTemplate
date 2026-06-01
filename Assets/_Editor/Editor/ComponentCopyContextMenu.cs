using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AkaitoAi
{
    public static class ComponentCopyData
    {
        public static List<Component> sourceComponents = new List<Component>();
        public static bool withValues = true;
    }

    public static class ComponentCopyContextMenu
    {
        // Copy all components with values
        [MenuItem("CONTEXT/GameObject/Copy All Components With Values")]
        static void CopyAllWithValues(MenuCommand command)
        {
            GameObject source = (GameObject)command.context;
            ComponentCopyData.sourceComponents = new List<Component>(source.GetComponents<Component>());
            ComponentCopyData.withValues = true;
        }

        [MenuItem("CONTEXT/GameObject/Copy All Components With Values", true)]
        static bool ValidateCopyAllWithValues(MenuCommand command)
        {
            return Selection.gameObjects.Length == 1;
        }

        // Copy all components without values
        [MenuItem("CONTEXT/GameObject/Copy All Components Without Values")]
        static void CopyAllWithoutValues(MenuCommand command)
        {
            GameObject source = (GameObject)command.context;
            ComponentCopyData.sourceComponents = new List<Component>(source.GetComponents<Component>());
            ComponentCopyData.withValues = false;
        }

        [MenuItem("CONTEXT/GameObject/Copy All Components Without Values", true)]
        static bool ValidateCopyAllWithoutValues(MenuCommand command)
        {
            return Selection.gameObjects.Length == 1;
        }

        // Copy all components without Transform with values
        [MenuItem("CONTEXT/GameObject/Copy All Components Without Transform With Values")]
        static void CopyAllWithoutTransformWithValues(MenuCommand command)
        {
            GameObject source = (GameObject)command.context;
            ComponentCopyData.sourceComponents = new List<Component>(source.GetComponents<Component>()).Where(c => !(c is Transform)).ToList();
            ComponentCopyData.withValues = true;
        }

        [MenuItem("CONTEXT/GameObject/Copy All Components Without Transform With Values", true)]
        static bool ValidateCopyAllWithoutTransformWithValues(MenuCommand command)
        {
            return Selection.gameObjects.Length == 1;
        }

        // Copy all components without Transform without values
        [MenuItem("CONTEXT/GameObject/Copy All Components Without Transform Without Values")]
        static void CopyAllWithoutTransformWithoutValues(MenuCommand command)
        {
            GameObject source = (GameObject)command.context;
            ComponentCopyData.sourceComponents = new List<Component>(source.GetComponents<Component>()).Where(c => !(c is Transform)).ToList();
            ComponentCopyData.withValues = false;
        }

        [MenuItem("CONTEXT/GameObject/Copy All Components Without Transform Without Values", true)]
        static bool ValidateCopyAllWithoutTransformWithoutValues(MenuCommand command)
        {
            return Selection.gameObjects.Length == 1;
        }

        // Paste components
        [MenuItem("CONTEXT/GameObject/Paste Components")]
        static void PasteComponentsMenu(MenuCommand command)
        {
            PasteComponents();
        }

        [MenuItem("CONTEXT/GameObject/Paste Components", true)]
        static bool ValidatePasteComponents(MenuCommand command)
        {
            return ComponentCopyData.sourceComponents.Count > 0 && Selection.gameObjects.Length > 0;
        }

        static void PasteComponents()
        {
            if (ComponentCopyData.sourceComponents.Count == 0) return;

            foreach (GameObject target in Selection.gameObjects)
            {
                if (target == null) continue;

                foreach (Component sourceComp in ComponentCopyData.sourceComponents)
                {
                    Type type = sourceComp.GetType();

                    Component targetComp = target.GetComponent(type);
                    if (targetComp == null)
                    {
                        targetComp = target.AddComponent(type);
                    }

                    if (ComponentCopyData.withValues && targetComp != null)
                    {
                        targetComp.CopyValuesFrom(sourceComp);
                    }
                }
            }
        }
    }
}