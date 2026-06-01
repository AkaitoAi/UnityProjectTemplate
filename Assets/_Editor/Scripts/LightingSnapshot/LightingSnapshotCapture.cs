#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class LightingSnapshotCapture : MonoBehaviour
{
    public LightingSnapshot snapshot;

    [ContextMenu("Capture Lighting From Scene")]
    public void Capture()
    {
        if (snapshot == null)
        {
            Debug.LogError("LightingSnapshot is missing.");
            return;
        }

        snapshot.skybox = RenderSettings.skybox;

        snapshot.ambientMode = RenderSettings.ambientMode;
        snapshot.ambientLight = RenderSettings.ambientLight;
        snapshot.ambientIntensity = RenderSettings.ambientIntensity;
        snapshot.ambientSkyColor = RenderSettings.ambientSkyColor;
        snapshot.ambientEquatorColor = RenderSettings.ambientEquatorColor;
        snapshot.ambientGroundColor = RenderSettings.ambientGroundColor;

        snapshot.fog = RenderSettings.fog;
        snapshot.fogMode = RenderSettings.fogMode;
        snapshot.fogColor = RenderSettings.fogColor;
        snapshot.fogDensity = RenderSettings.fogDensity;
        snapshot.fogStart = RenderSettings.fogStartDistance;
        snapshot.fogEnd = RenderSettings.fogEndDistance;

        snapshot.sun = RenderSettings.sun;

        snapshot.lightmapsMode = LightmapSettings.lightmapsMode;
        snapshot.lightmaps = LightmapSettings.lightmaps;

        EditorUtility.SetDirty(snapshot);
        AssetDatabase.SaveAssets();

        Debug.Log($"[Lighting] Captured lighting from scene '{gameObject.scene.name}'");
    }
}
#endif
