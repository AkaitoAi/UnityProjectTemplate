using UnityEngine;
using AkaitoAi.GameBase;

public class CameraObstructionTriggerHandler : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float transparentAlpha = 0.3f;

    private MaterialFader materialFader;

    private void Awake()
    {
        materialFader = new MaterialFader(transparentMaterial, fadeDuration, transparentAlpha);
    }

    private void OnTriggerEnter(Collider other)
    {
        materialFader.FadeToTransparent(other.GetComponentsInChildren<Renderer>());
    }

    private void OnTriggerExit(Collider other)
    {
        materialFader.RestoreOriginal(other.GetComponentsInChildren<Renderer>());
    }

    private void OnDisable()
    {
        materialFader.ForceRestoreAll();
    }
}
