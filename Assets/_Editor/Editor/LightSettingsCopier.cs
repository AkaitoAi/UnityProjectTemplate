using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace AkaitoAi
{
    public class LightSettingsCopier : EditorWindow
    {
        private SceneAsset sourceSceneAsset;
        private SceneAsset targetSceneAsset;

        private RenderSettingsData originalRenderSettings;
        private LightmapSettingsData originalLightmapSettings;
        private EnvironmentLightingData originalEnvironmentLightingSettings;

        private bool canRevert = false;

        [MenuItem("AkaitoAi/Tools/Light Settings Copier")]
        public static void ShowWindow()
        {
            GetWindow<LightSettingsCopier>("Light Settings Copier");
        }

        private void OnGUI()
        {
            GUILayout.Label("Copy Light Settings from One Scene to Another", EditorStyles.boldLabel);

            sourceSceneAsset = EditorGUILayout.ObjectField("Source Scene", sourceSceneAsset, typeof(SceneAsset), false) as SceneAsset;
            targetSceneAsset = EditorGUILayout.ObjectField("Target Scene", targetSceneAsset, typeof(SceneAsset), false) as SceneAsset;

            if (GUILayout.Button("Copy Light Settings"))
            {
                if (sourceSceneAsset == null || targetSceneAsset == null)
                {
                    Debug.LogError("Please assign both Source and Target scenes.");
                }
                else
                {
                    CopyLightSettings();
                }
            }

            GUI.enabled = canRevert;
            if (GUILayout.Button("Revert Light Settings"))
            {
                if (targetSceneAsset == null)
                {
                    Debug.LogError("Please assign the Target scene to revert settings.");
                }
                else
                {
                    RevertLightSettings();
                }
            }
            GUI.enabled = true;
        }

        private void CopyLightSettings()
        {
            string sourceScenePath = AssetDatabase.GetAssetPath(sourceSceneAsset);
            string targetScenePath = AssetDatabase.GetAssetPath(targetSceneAsset);

            if (string.IsNullOrEmpty(sourceScenePath) || string.IsNullOrEmpty(targetScenePath))
            {
                Debug.LogError("Invalid scene paths.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // Open the target scene first to store its original settings for reverting
            Scene targetScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
            StoreOriginalLightingSettings();

            // Open the source scene to copy settings
            Scene sourceScene = EditorSceneManager.OpenScene(sourceScenePath, OpenSceneMode.Single);
            CopyLightingSettingsFromScene();

            // Apply copied settings to the target scene
            EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);
            ApplyLightingSettingsToScene();

            Debug.Log("Lighting settings copied successfully!");

            // Save the modified target scene
            EditorSceneManager.SaveScene(targetScene);

            // Enable the revert option
            canRevert = true;
        }

        private void StoreOriginalLightingSettings()
        {
            // Store original RenderSettings of the target scene
            originalRenderSettings = new RenderSettingsData
            {
                ambientMode = RenderSettings.ambientMode,
                ambientSkyColor = RenderSettings.ambientSkyColor,
                ambientEquatorColor = RenderSettings.ambientEquatorColor,
                ambientGroundColor = RenderSettings.ambientGroundColor,
                ambientIntensity = RenderSettings.ambientIntensity,
                fog = RenderSettings.fog,
                fogColor = RenderSettings.fogColor,
                fogMode = RenderSettings.fogMode,
                fogDensity = RenderSettings.fogDensity,
                fogStartDistance = RenderSettings.fogStartDistance,
                fogEndDistance = RenderSettings.fogEndDistance,
                reflectionIntensity = RenderSettings.reflectionIntensity,
                skyboxMaterial = RenderSettings.skybox,
                defaultReflectionMode = RenderSettings.defaultReflectionMode,
                defaultReflectionResolution = RenderSettings.defaultReflectionResolution,
                reflectionBounces = RenderSettings.reflectionBounces,
            };

            // Store original Lightmap settings
            originalLightmapSettings = new LightmapSettingsData
            {
                lightingDataAsset = Lightmapping.lightingDataAsset,
                lightmapsMode = LightmapSettings.lightmapsMode,
                lightProbes = LightmapSettings.lightProbes,
                lightmaps = LightmapSettings.lightmaps
            };

            // Store original Environment lighting settings
            originalEnvironmentLightingSettings = new EnvironmentLightingData
            {
                sunSource = RenderSettings.sun,
                ambientMode = RenderSettings.ambientMode,
                ambientSkyColor = RenderSettings.ambientSkyColor,
                ambientEquatorColor = RenderSettings.ambientEquatorColor,
                ambientGroundColor = RenderSettings.ambientGroundColor,
                ambientIntensity = RenderSettings.ambientIntensity
            };
        }

        private void RevertLightSettings()
        {
            string targetScenePath = AssetDatabase.GetAssetPath(targetSceneAsset);

            if (string.IsNullOrEmpty(targetScenePath))
            {
                Debug.LogError("Invalid scene path.");
                return;
            }

            // Open the target scene
            Scene targetScene = EditorSceneManager.OpenScene(targetScenePath, OpenSceneMode.Single);

            // Apply stored original RenderSettings
            RenderSettings.ambientMode = originalRenderSettings.ambientMode;
            RenderSettings.ambientSkyColor = originalRenderSettings.ambientSkyColor;
            RenderSettings.ambientEquatorColor = originalRenderSettings.ambientEquatorColor;
            RenderSettings.ambientGroundColor = originalRenderSettings.ambientGroundColor;
            RenderSettings.ambientIntensity = originalRenderSettings.ambientIntensity;
            RenderSettings.fog = originalRenderSettings.fog;
            RenderSettings.fogColor = originalRenderSettings.fogColor;
            RenderSettings.fogMode = originalRenderSettings.fogMode;
            RenderSettings.fogDensity = originalRenderSettings.fogDensity;
            RenderSettings.fogStartDistance = originalRenderSettings.fogStartDistance;
            RenderSettings.fogEndDistance = originalRenderSettings.fogEndDistance;
            RenderSettings.reflectionIntensity = originalRenderSettings.reflectionIntensity;
            RenderSettings.skybox = originalRenderSettings.skyboxMaterial;
            RenderSettings.defaultReflectionMode = originalRenderSettings.defaultReflectionMode;
            RenderSettings.defaultReflectionResolution = originalRenderSettings.defaultReflectionResolution;
            RenderSettings.reflectionBounces = originalRenderSettings.reflectionBounces;

            // Apply original Lightmap settings
            LightmapSettings.lightmapsMode = originalLightmapSettings.lightmapsMode;
            LightmapSettings.lightProbes = originalLightmapSettings.lightProbes;
            LightmapSettings.lightmaps = originalLightmapSettings.lightmaps;

            // Apply original environment lighting settings
            RenderSettings.sun = originalEnvironmentLightingSettings.sunSource;
            RenderSettings.ambientMode = originalEnvironmentLightingSettings.ambientMode;
            RenderSettings.ambientSkyColor = originalEnvironmentLightingSettings.ambientSkyColor;
            RenderSettings.ambientEquatorColor = originalEnvironmentLightingSettings.ambientEquatorColor;
            RenderSettings.ambientGroundColor = originalEnvironmentLightingSettings.ambientGroundColor;
            RenderSettings.ambientIntensity = originalEnvironmentLightingSettings.ambientIntensity;

            // Save the target scene after reverting
            EditorSceneManager.SaveScene(targetScene);

            Debug.Log("Lighting settings reverted successfully!");
        }

        private void CopyLightingSettingsFromScene()
        {
            // Copy RenderSettings from the source scene
            sourceRenderSettings = new RenderSettingsData
            {
                ambientMode = RenderSettings.ambientMode,
                ambientSkyColor = RenderSettings.ambientSkyColor,
                ambientEquatorColor = RenderSettings.ambientEquatorColor,
                ambientGroundColor = RenderSettings.ambientGroundColor,
                ambientIntensity = RenderSettings.ambientIntensity,
                fog = RenderSettings.fog,
                fogColor = RenderSettings.fogColor,
                fogMode = RenderSettings.fogMode,
                fogDensity = RenderSettings.fogDensity,
                fogStartDistance = RenderSettings.fogStartDistance,
                fogEndDistance = RenderSettings.fogEndDistance,
                reflectionIntensity = RenderSettings.reflectionIntensity,
                skyboxMaterial = RenderSettings.skybox,
                defaultReflectionMode = RenderSettings.defaultReflectionMode,
                defaultReflectionResolution = RenderSettings.defaultReflectionResolution,
                reflectionBounces = RenderSettings.reflectionBounces,
            };

            // Copy Lightmap settings
            lightmapSettings = new LightmapSettingsData
            {
                lightingDataAsset = Lightmapping.lightingDataAsset,
                lightmapsMode = LightmapSettings.lightmapsMode,
                lightProbes = LightmapSettings.lightProbes,
                lightmaps = LightmapSettings.lightmaps
            };

            // Copy environment lighting settings
            environmentLightingSettings = new EnvironmentLightingData
            {
                sunSource = RenderSettings.sun,
                ambientMode = RenderSettings.ambientMode,
                ambientSkyColor = RenderSettings.ambientSkyColor,
                ambientEquatorColor = RenderSettings.ambientEquatorColor,
                ambientGroundColor = RenderSettings.ambientGroundColor,
                ambientIntensity = RenderSettings.ambientIntensity
            };
        }

        private void ApplyLightingSettingsToScene()
        {
            // Apply RenderSettings from source to target
            RenderSettings.ambientMode = sourceRenderSettings.ambientMode;
            RenderSettings.ambientSkyColor = sourceRenderSettings.ambientSkyColor;
            RenderSettings.ambientEquatorColor = sourceRenderSettings.ambientEquatorColor;
            RenderSettings.ambientGroundColor = sourceRenderSettings.ambientGroundColor;
            RenderSettings.ambientIntensity = sourceRenderSettings.ambientIntensity;
            RenderSettings.fog = sourceRenderSettings.fog;
            RenderSettings.fogColor = sourceRenderSettings.fogColor;
            RenderSettings.fogMode = sourceRenderSettings.fogMode;
            RenderSettings.fogDensity = sourceRenderSettings.fogDensity;
            RenderSettings.fogStartDistance = sourceRenderSettings.fogStartDistance;
            RenderSettings.fogEndDistance = sourceRenderSettings.fogEndDistance;
            RenderSettings.reflectionIntensity = sourceRenderSettings.reflectionIntensity;
            RenderSettings.skybox = sourceRenderSettings.skyboxMaterial;
            RenderSettings.defaultReflectionMode = sourceRenderSettings.defaultReflectionMode;
            RenderSettings.defaultReflectionResolution = sourceRenderSettings.defaultReflectionResolution;
            RenderSettings.reflectionBounces = sourceRenderSettings.reflectionBounces;

            // Apply Lightmap settings from source to target
            LightmapSettings.lightmapsMode = lightmapSettings.lightmapsMode;
            LightmapSettings.lightProbes = lightmapSettings.lightProbes;
            LightmapSettings.lightmaps = lightmapSettings.lightmaps;

            // Apply environment lighting settings
            RenderSettings.sun = environmentLightingSettings.sunSource;
            RenderSettings.ambientMode = environmentLightingSettings.ambientMode;
            RenderSettings.ambientSkyColor = environmentLightingSettings.ambientSkyColor;
            RenderSettings.ambientEquatorColor = environmentLightingSettings.ambientEquatorColor;
            RenderSettings.ambientGroundColor = environmentLightingSettings.ambientGroundColor;
            RenderSettings.ambientIntensity = environmentLightingSettings.ambientIntensity;
        }

        // Data structures to store lighting settings
        private RenderSettingsData sourceRenderSettings;
        private LightmapSettingsData lightmapSettings;
        private EnvironmentLightingData environmentLightingSettings;

        [System.Serializable]
        private struct RenderSettingsData
        {
            public AmbientMode ambientMode;
            public Color ambientSkyColor;
            public Color ambientEquatorColor;
            public Color ambientGroundColor;
            public float ambientIntensity;
            public bool fog;
            public Color fogColor;
            public FogMode fogMode;
            public float fogDensity;
            public float fogStartDistance;
            public float fogEndDistance;
            public float reflectionIntensity;
            public Material skyboxMaterial;
            public DefaultReflectionMode defaultReflectionMode;
            public int defaultReflectionResolution;
            public int reflectionBounces;
        }

        [System.Serializable]
        private struct LightmapSettingsData
        {
            public Object lightingDataAsset;
            public LightmapsMode lightmapsMode;
            public LightProbes lightProbes;
            public LightmapData[] lightmaps;
        }

        [System.Serializable]
        private struct EnvironmentLightingData
        {
            public Light sunSource;
            public AmbientMode ambientMode;
            public Color ambientSkyColor;
            public Color ambientEquatorColor;
            public Color ambientGroundColor;
            public float ambientIntensity;
        }
    }
}
