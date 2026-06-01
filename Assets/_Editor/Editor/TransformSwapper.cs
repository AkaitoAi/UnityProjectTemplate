using UnityEngine;
using UnityEditor;
using System.Collections;

public class TransformSwapper : EditorWindow
{
    private GameObject object1;
    private GameObject object2;
    private bool swapPosition = true;
    private bool swapRotation = false;
    private bool swapScale = false;
    private bool swapRectTransform = true; // New option for UI-specific properties

    [MenuItem("AkaitoAi/Tools/GameObject/Transform Swapper")]
    public static void ShowWindow()
    {
        GetWindow<TransformSwapper>("Transform Swapper");
    }

    void OnGUI()
    {
        GUILayout.Label("Transform Swapper", EditorStyles.boldLabel);

        // Object selection fields
        object1 = EditorGUILayout.ObjectField("Object 1", object1, typeof(GameObject), true) as GameObject;
        object2 = EditorGUILayout.ObjectField("Object 2", object2, typeof(GameObject), true) as GameObject;

        // Swap options
        GUILayout.Space(10);
        GUILayout.Label("Swap Options", EditorStyles.boldLabel);
        swapPosition = EditorGUILayout.Toggle("Swap Position", swapPosition);
        swapRotation = EditorGUILayout.Toggle("Swap Rotation", swapRotation);
        swapScale = EditorGUILayout.Toggle("Swap Scale", swapScale);

        // Only show this option if both objects have RectTransforms
        bool showRectOption = (object1 != null && object1.GetComponent<RectTransform>() != null) &&
                            (object2 != null && object2.GetComponent<RectTransform>() != null);
        if (showRectOption)
        {
            swapRectTransform = EditorGUILayout.Toggle("Swap RectTransform Properties", swapRectTransform);
        }

        // Swap button
        GUILayout.Space(20);
        GUI.enabled = object1 != null && object2 != null &&
                     (swapPosition || swapRotation || swapScale || (showRectOption && swapRectTransform));
        if (GUILayout.Button("Swap Transforms"))
        {
            SwapTransforms();
        }
        GUI.enabled = true;

        // Validation messages
        if (object1 == null || object2 == null)
        {
            EditorGUILayout.HelpBox("Please select two GameObjects to swap transforms.", MessageType.Warning);
        }
        if (!swapPosition && !swapRotation && !swapScale && (!showRectOption || !swapRectTransform))
        {
            EditorGUILayout.HelpBox("Please select at least one transform property to swap.", MessageType.Warning);
        }
    }

    void SwapTransforms()
    {
        if (object1 == null || object2 == null) return;

        Undo.RecordObjects(new Object[] { object1.transform, object2.transform }, "Swap Transforms");

        // Handle regular Transform properties
        Vector3 tempPosition = object1.transform.position;
        Quaternion tempRotation = object1.transform.rotation;
        Vector3 tempScale = object1.transform.localScale;

        if (swapPosition)
        {
            object1.transform.position = object2.transform.position;
            object2.transform.position = tempPosition;
        }

        if (swapRotation)
        {
            object1.transform.rotation = object2.transform.rotation;
            object2.transform.rotation = tempRotation;
        }

        if (swapScale)
        {
            object1.transform.localScale = object2.transform.localScale;
            object2.transform.localScale = tempScale;
        }

        // Handle RectTransform-specific properties
        RectTransform rect1 = object1.GetComponent<RectTransform>();
        RectTransform rect2 = object2.GetComponent<RectTransform>();
        if (swapRectTransform && rect1 != null && rect2 != null)
        {
            // Store temporary values
            Vector2 tempAnchorMin = rect1.anchorMin;
            Vector2 tempAnchorMax = rect1.anchorMax;
            Vector2 tempAnchoredPosition = rect1.anchoredPosition;
            Vector2 tempSizeDelta = rect1.sizeDelta;
            Vector2 tempPivot = rect1.pivot;

            // Swap RectTransform properties
            rect1.anchorMin = rect2.anchorMin;
            rect1.anchorMax = rect2.anchorMax;
            rect1.anchoredPosition = rect2.anchoredPosition;
            rect1.sizeDelta = rect2.sizeDelta;
            rect1.pivot = rect2.pivot;

            rect2.anchorMin = tempAnchorMin;
            rect2.anchorMax = tempAnchorMax;
            rect2.anchoredPosition = tempAnchoredPosition;
            rect2.sizeDelta = tempSizeDelta;
            rect2.pivot = tempPivot;
        }
    }

    void OnSelectionChange()
    {
        GameObject[] selection = Selection.gameObjects;
        if (selection.Length >= 1) object1 = selection[0];
        if (selection.Length >= 2) object2 = selection[1];
        Repaint();
    }
}