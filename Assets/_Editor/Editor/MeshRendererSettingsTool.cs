using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace AkaitoAi
{
    public class MeshRendererSettingsTool : EditorWindow
    {
        private bool applyLightProbes = true;
        private LightProbeUsage lightProbeUsage = LightProbeUsage.Off;

        private bool applyReflectionProbes = true;
        private ReflectionProbeUsage reflectionProbeUsage = ReflectionProbeUsage.Off;

        private bool applyAnchorOverride = false;
        private Transform anchorOverride;

        private bool applyMotionVectors = true;
        private MotionVectorGenerationMode motionVectors = MotionVectorGenerationMode.Camera;

        private bool applyReceiveShadows = true;
        private bool receiveShadows = false;

        private bool applyShadowCastingMode = true;
        private ShadowCastingMode shadowCastingMode = ShadowCastingMode.Off;

        private bool applyLightmapScaleOffset = false;
        private Vector4 lightmapScaleOffset = Vector4.one;

        private bool applyContributeToGI = true;
        private bool contributeToGI = false;

        [MenuItem("AkaitoAi/Tools/MeshRenderer Settings")]
        public static void ShowWindow()
        {
            GetWindow<MeshRendererSettingsTool>("MeshRenderer Settings Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Set MeshRenderer Settings (Excluding Materials)", EditorStyles.boldLabel);

            // Light Probes Usage
            applyLightProbes = EditorGUILayout.Toggle("Apply Light Probes", applyLightProbes);
            if (applyLightProbes)
            {
                lightProbeUsage = (LightProbeUsage)EditorGUILayout.EnumPopup("Light Probe Usage", lightProbeUsage);
            }

            // Reflection Probes Usage
            applyReflectionProbes = EditorGUILayout.Toggle("Apply Reflection Probes", applyReflectionProbes);
            if (applyReflectionProbes)
            {
                reflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probe Usage", reflectionProbeUsage);
            }

            // Anchor Override
            applyAnchorOverride = EditorGUILayout.Toggle("Apply Anchor Override", applyAnchorOverride);
            if (applyAnchorOverride)
            {
                anchorOverride = (Transform)EditorGUILayout.ObjectField("Anchor Override", anchorOverride, typeof(Transform), true);
            }

            // Motion Vectors
            applyMotionVectors = EditorGUILayout.Toggle("Apply Motion Vectors", applyMotionVectors);
            if (applyMotionVectors)
            {
                motionVectors = (MotionVectorGenerationMode)EditorGUILayout.EnumPopup("Motion Vector Generation", motionVectors);
            }

            // Receive Shadows
            applyReceiveShadows = EditorGUILayout.Toggle("Apply Receive Shadows", applyReceiveShadows);
            if (applyReceiveShadows)
            {
                receiveShadows = EditorGUILayout.Toggle("Receive Shadows", receiveShadows);
            }

            // Shadow Casting Mode
            applyShadowCastingMode = EditorGUILayout.Toggle("Apply Shadow Casting Mode", applyShadowCastingMode);
            if (applyShadowCastingMode)
            {
                shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", shadowCastingMode);
            }

            // Lightmap Scale Offset
            applyLightmapScaleOffset = EditorGUILayout.Toggle("Apply Lightmap Scale Offset", applyLightmapScaleOffset);
            if (applyLightmapScaleOffset)
            {
                lightmapScaleOffset = EditorGUILayout.Vector4Field("Lightmap Scale Offset", lightmapScaleOffset);
            }

            // Contribute to Global Illumination
            applyContributeToGI = EditorGUILayout.Toggle("Apply Contribute to GI", applyContributeToGI);
            if (applyContributeToGI)
            {
                contributeToGI = EditorGUILayout.Toggle("Contribute to Global Illumination", contributeToGI);
            }

            // Apply Button
            if (GUILayout.Button("Apply to Selected MeshRenderers"))
            {
                ApplyToSelectedMeshRenderers();
            }
        }

        private void ApplyToSelectedMeshRenderers()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            foreach (GameObject obj in selectedObjects)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    Undo.RecordObject(meshRenderer, "Modify MeshRenderer Settings");

                    if (applyLightProbes)
                    {
                        meshRenderer.lightProbeUsage = lightProbeUsage;
                    }

                    if (applyReflectionProbes)
                    {
                        meshRenderer.reflectionProbeUsage = reflectionProbeUsage;
                    }

                    if (applyAnchorOverride)
                    {
                        meshRenderer.probeAnchor = anchorOverride;
                    }

                    if (applyMotionVectors)
                    {
                        meshRenderer.motionVectorGenerationMode = motionVectors;
                    }

                    if (applyReceiveShadows)
                    {
                        meshRenderer.receiveShadows = receiveShadows;
                    }

                    if (applyShadowCastingMode)
                    {
                        meshRenderer.shadowCastingMode = shadowCastingMode;
                    }

                    if (applyLightmapScaleOffset)
                    {
                        meshRenderer.lightmapScaleOffset = lightmapScaleOffset;
                    }

                    // Apply Contribute to Global Illumination
                    if (applyContributeToGI)
                    {
                        // Change static flags to enable or disable contributing to global illumination
                        StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(obj);
                        if (contributeToGI)
                        {
                            flags |= StaticEditorFlags.ContributeGI;
                        }
                        else
                        {
                            flags &= ~StaticEditorFlags.ContributeGI;
                        }
                        GameObjectUtility.SetStaticEditorFlags(obj, flags);
                    }

                    EditorUtility.SetDirty(meshRenderer);
                }
            }
        }
    }
}
