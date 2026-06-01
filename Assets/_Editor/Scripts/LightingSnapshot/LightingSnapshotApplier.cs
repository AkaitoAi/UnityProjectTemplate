using AkaitoAi.GameBase;
using UnityEngine;
using UnityEngine.Rendering;

public class LightingSnapshotApplier : MonoBehaviour
{
    public LightingSnapshot snapshot;
    public bool applyOnStart = true;


    EventBinding<OnLightingChanged> lightingSnapshotEventBinding;
    private void Awake()
    {
        lightingSnapshotEventBinding = new EventBinding<OnLightingChanged>(UpdateLighting);
        EventBus<OnLightingChanged>.Register(lightingSnapshotEventBinding);
        CachedEvent<OnLightingChanged>.Subscribe(UpdateLighting, invokeWithLast: true);
    }

    private void OnDestroy()
    {
        CachedEvent<OnLightingChanged>.Unsubscribe(UpdateLighting);
    }

    private void UpdateLighting(OnLightingChanged lss)
    { 
        snapshot = lss.lightingSnapshot;
        Apply();
    }

    private void Start()
    {
        if (applyOnStart && snapshot != null)
            Apply();
    }

    public void Apply()
    {
        if (snapshot == null)
            return;

        RenderSettings.skybox = snapshot.skybox;

        RenderSettings.ambientMode = snapshot.ambientMode;
        RenderSettings.ambientLight = snapshot.ambientLight;
        RenderSettings.ambientIntensity = snapshot.ambientIntensity;
        RenderSettings.ambientSkyColor = snapshot.ambientSkyColor;
        RenderSettings.ambientEquatorColor = snapshot.ambientEquatorColor;
        RenderSettings.ambientGroundColor = snapshot.ambientGroundColor;

        RenderSettings.fog = snapshot.fog;
        RenderSettings.fogMode = snapshot.fogMode;
        RenderSettings.fogColor = snapshot.fogColor;
        RenderSettings.fogDensity = snapshot.fogDensity;
        RenderSettings.fogStartDistance = snapshot.fogStart;
        RenderSettings.fogEndDistance = snapshot.fogEnd;

        RenderSettings.sun = snapshot.sun;

        if (snapshot.lightmaps != null && snapshot.lightmaps.Length > 0)
        {
            LightmapSettings.lightmapsMode = snapshot.lightmapsMode;
            LightmapSettings.lightmaps = snapshot.lightmaps;
        }

        DynamicGI.UpdateEnvironment();

        Debug.Log("[Lighting] Lighting snapshot applied.");
    }
}
