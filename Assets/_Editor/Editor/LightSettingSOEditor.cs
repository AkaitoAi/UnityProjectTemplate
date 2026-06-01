using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightSettingSO))]
public class LightSettingSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LightSettingSO lightSettingSO = (LightSettingSO)target;

        // Display the default inspector for the array (Unity handles adding/removing elements)
        SerializedProperty lightSettingsProp = serializedObject.FindProperty("lightSettings");
        EditorGUILayout.PropertyField(lightSettingsProp, true);

        // Loop through each light setting and add custom buttons
        for (int i = 0; i < lightSettingSO.lightSettings.Length; i++)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Light Setting {i + 1}", EditorStyles.boldLabel);

            // Button to apply the light settings to the scene
            if (GUILayout.Button($"Apply Light Setting {i + 1} to Scene"))
            {
                ApplyLightSettingsToScene(lightSettingSO.lightSettings[i]);
            }

            // Button to copy the current scene settings into this LightSetting
            if (GUILayout.Button($"Copy Scene Settings to Light Setting {i + 1}"))
            {
                CopySceneSettingsToLightSetting(ref lightSettingSO.lightSettings[i]);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ApplyLightSettingsToScene(LightSettingSO.LightSettings lightSettings)
    {
        // Properly applying the settings to RenderSettings for ambient and fog
        if (lightSettings.gradientColor)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight; // Gradient color mode
            RenderSettings.ambientSkyColor = lightSettings.skyColor;
            RenderSettings.ambientEquatorColor = lightSettings.equatorColor;
            RenderSettings.ambientGroundColor = lightSettings.groundColor;
        }
        else
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; // Single color mode
            RenderSettings.ambientSkyColor = lightSettings.skyColor;
        }

        RenderSettings.ambientIntensity = lightSettings.colorIntensity;

        // Assign the skybox material if any
        RenderSettings.skybox = lightSettings.skyBox;

        // Handle fog settings
        RenderSettings.fog = lightSettings.fog;
        if (lightSettings.fog)
        {
            RenderSettings.fogMode = lightSettings.fogMode;
            RenderSettings.fogColor = lightSettings.fogColor;
            RenderSettings.fogDensity = lightSettings.fogDensity;
            RenderSettings.fogStartDistance = lightSettings.fogStartEnd.x;
            RenderSettings.fogEndDistance = lightSettings.fogStartEnd.y;
        }

        // Handle directional light settings
        if (lightSettings.changeColor && lightSettings.directionalLight != null)
        {
            lightSettings.directionalLight.color = lightSettings.directionalLightColor;
        }

        // Activate reflection probe if assigned
        if (lightSettings.reflectionProbeObj)
        {
            lightSettings.reflectionProbeObj.SetActive(true);
        }

        Debug.Log("Applied Light Settings to the scene.");
    }

    private void CopySceneSettingsToLightSetting(ref LightSettingSO.LightSettings lightSettings)
    {
        // Copy ambient settings from the scene
        if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight)
        {
            lightSettings.gradientColor = true;
            lightSettings.skyColor = RenderSettings.ambientSkyColor;
            lightSettings.equatorColor = RenderSettings.ambientEquatorColor;
            lightSettings.groundColor = RenderSettings.ambientGroundColor;
        }
        else
        {
            lightSettings.gradientColor = false;
            lightSettings.skyColor = RenderSettings.ambientSkyColor;
        }

        lightSettings.colorIntensity = RenderSettings.ambientIntensity;

        // Copy fog settings
        lightSettings.fog = RenderSettings.fog;
        lightSettings.fogColor = RenderSettings.fogColor;
        lightSettings.fogDensity = RenderSettings.fogDensity;
        lightSettings.fogMode = RenderSettings.fogMode;
        lightSettings.fogStartEnd = new Vector2(RenderSettings.fogStartDistance, RenderSettings.fogEndDistance);

        // Copy skybox material
        lightSettings.skyBox = RenderSettings.skybox;

        // Copy directional light settings
        Light directionalLight = RenderSettings.sun; // Assuming the main directional light is set as Sun in Lighting settings
        if (directionalLight != null)
        {
            lightSettings.directionalLight = directionalLight;
            lightSettings.directionalLightColor = directionalLight.color;
        }

        Debug.Log("Copied scene settings to the LightSetting entry.");
    }
}
