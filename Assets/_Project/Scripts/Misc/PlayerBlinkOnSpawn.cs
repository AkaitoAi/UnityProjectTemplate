using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

public class PlayerBlinkOnSpawn : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float totalDuration = 2f;
    [SerializeField] private int blinkCount = 6;

    private void OnEnable()
    {
        BlinkFlickerEffect();
    }

    private void BlinkFlickerEffect()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Material[][] originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].materials;

#if DOTWEEN
        Sequence seq = DOTween.Sequence();
        float blinkDuration = totalDuration / (blinkCount * 2f);

        for (int i = 0; i < blinkCount; i++)
        {
            seq.AppendCallback(() =>
            {
                foreach (Renderer r in renderers)
                {
                    Material[] transparentMats = new Material[r.materials.Length];
                    for (int j = 0; j < r.materials.Length; j++)
                        transparentMats[j] = transparentMaterial;
                    r.materials = transparentMats;
                }
            });
            seq.AppendInterval(blinkDuration);

            seq.AppendCallback(() =>
            {
                for (int k = 0; k < renderers.Length; k++)
                    renderers[k].materials = originalMaterials[k];
            });
            seq.AppendInterval(blinkDuration);
        }
#endif
    }
}
