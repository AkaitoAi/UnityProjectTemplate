using UnityEngine;

[CreateAssetMenu(fileName = "LightSetting", menuName = "ScriptableObjects/LightSetting", order = 1)]
public class LightSettingSO : ScriptableObject
{
    public LightSettings[] lightSettings;

    [System.Serializable]
    public struct LightSettings
    {
        public bool gradientColor;
        public Color skyColor;
        public Color equatorColor;
        public Color groundColor;
        public float colorIntensity;
        public FogMode fogMode;
        public bool fog;
        public Color fogColor;
        public float fogDensity;
        public Vector2 fogStartEnd;
        public Material skyBox;
        public bool changeColor;
        public Light directionalLight;
        public Color directionalLightColor;
        public GameObject reflectionProbeObj;
    }

    public void SceneLightSetup(LightSettings _lightSettings) //! Setups light settings
    {
        if (_lightSettings.gradientColor)
        {
            RenderSettings.ambientSkyColor = _lightSettings.skyColor;
            RenderSettings.ambientEquatorColor = _lightSettings.equatorColor;
            RenderSettings.ambientGroundColor = _lightSettings.groundColor;
            RenderSettings.ambientIntensity = _lightSettings.colorIntensity;
            RenderSettings.skybox = _lightSettings.skyBox;
        }
        else
        {
            RenderSettings.ambientSkyColor = _lightSettings.skyColor;
            RenderSettings.ambientIntensity = _lightSettings.colorIntensity;
            RenderSettings.skybox = _lightSettings.skyBox;
        }

        if (_lightSettings.fog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = _lightSettings.fogColor;
            RenderSettings.fogDensity = _lightSettings.fogDensity;
            RenderSettings.fogMode = _lightSettings.fogMode;
            RenderSettings.fogStartDistance = _lightSettings.fogStartEnd.x;
            RenderSettings.fogEndDistance = _lightSettings.fogStartEnd.y;
        }
        else RenderSettings.fog = false;

        if (_lightSettings.changeColor)
        {
            _lightSettings.changeColor = true;
            _lightSettings.directionalLight.color = _lightSettings.directionalLightColor;
        }
        else _lightSettings.changeColor = false;

        if (_lightSettings.reflectionProbeObj) _lightSettings.reflectionProbeObj.SetActive(true);
    }
}
