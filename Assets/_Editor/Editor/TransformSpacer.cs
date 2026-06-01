using UnityEditor;
using UnityEngine;

public class TransformSpacer : EditorWindow
{
    private float spacing = 1.0f;
    private Axis selectedAxis = Axis.X;

    private enum Axis { X, Y, Z }

    [MenuItem("AkaitoAi/Tools/GameObject/Transform Spacer")]
    private static void ShowWindow()
    {
        GetWindow<TransformSpacer>("Transform Spacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Space Transforms", EditorStyles.boldLabel);

        Axis newAxis = (Axis)EditorGUILayout.EnumPopup("Axis", selectedAxis);
        if (newAxis != selectedAxis)
        {
            selectedAxis = newAxis;
            ApplySpacing();
        }

        float newSpacing = EditorGUILayout.FloatField("Spacing", spacing);
        if (!Mathf.Approximately(newSpacing, spacing))
        {
            spacing = newSpacing;
            ApplySpacing();
        }
    }

    private void ApplySpacing()
    {
        Transform[] selectedTransforms = Selection.transforms;
        if (selectedTransforms.Length < 2)
        {
            Debug.LogWarning("Select at least two transforms to apply spacing.");
            return;
        }

        Undo.RecordObjects(selectedTransforms, "Apply Transform Spacing");

        System.Array.Sort(selectedTransforms, (a, b) => string.Compare(a.name, b.name));

        Vector3 startPosition = selectedTransforms[0].position;
        for (int i = 1; i < selectedTransforms.Length; i++)
        {
            Vector3 newPosition = startPosition;
            switch (selectedAxis)
            {
                case Axis.X: newPosition.x += spacing * i; break;
                case Axis.Y: newPosition.y += spacing * i; break;
                case Axis.Z: newPosition.z += spacing * i; break;
            }
            selectedTransforms[i].position = newPosition;
        }
    }
}
