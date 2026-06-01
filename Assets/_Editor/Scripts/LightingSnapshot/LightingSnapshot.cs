using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Lighting/Lighting Snapshot")]
public class LightingSnapshot : ScriptableObject
{
    [Header("Skybox")]
    public Material skybox;

    [Header("Ambient")]
    public AmbientMode ambientMode;
    public Color ambientLight;
    public float ambientIntensity;
    public Color ambientSkyColor;
    public Color ambientEquatorColor;
    public Color ambientGroundColor;

    [Header("Fog")]
    public bool fog;
    public FogMode fogMode;
    public Color fogColor;
    public float fogDensity;
    public float fogStart;
    public float fogEnd;

    [Header("Sun")]
    public Light sun;

    [Header("Lightmaps")]
    public LightmapData[] lightmaps;
    public LightmapsMode lightmapsMode;
}
