using UnityEngine;

[CreateAssetMenu(fileName = "MPPConfig", menuName = "MobilePostProcessing/MPPConfig")]
public class MobilePostProcessingConfigSO : ScriptableObject
{
    [Header("Editor Preview")]
    [Tooltip("If enabled, the effect renders in the Scene view as well as the Game view.")]
    public bool PreviewInSceneView = false;

    public bool Blur = false;
    [Range(0, 1)]
    public float BlurAmount = 1f;
    public Texture2D BlurMask;
    public bool Bloom = false;
    public Color BloomColor = Color.white;
    [Range(0, 5)]
    public float BloomAmount = 1f;
    [Range(0, 1)]
    public float BloomDiffuse = 1f;
    [Range(0, 1)]
    public float BloomThreshold = 0f;
    [Range(0, 1)]
    public float BloomSoftness = 0f;

    public bool LUT = false;
    [Range(0, 1)]
    public float LutAmount = 0.0f;
    public Texture2D SourceLut = null;

    public bool ImageFiltering = false;
    public Color Color = Color.white;
    [Range(0, 1)]
    public float Contrast = 0f;
    [Range(-1, 1)]
    public float Brightness = 0f;
    [Range(-1, 1)]
    public float Saturation = 0f;
    [Range(-1, 1)]
    public float Exposure = 0f;
    [Range(-1, 1)]
    public float Gamma = 0f;
    [Range(0, 1)]
    public float Sharpness = 0f;

    public bool ChromaticAberration = false;
    public float Offset = 0;
    [Range(-1, 1)]
    public float FishEyeDistortion = 0;
    [Range(0, 1)]
    public float GlitchAmount = 0;

    public bool Distortion = false;
    [Range(0, 1)]
    public float LensDistortion = 0;

    public bool Vignette = false;
    public Color VignetteColor = Color.black;
    [Range(0, 1)]
    public float VignetteAmount = 0f;
    [Range(0.001f, 1)]
    public float VignetteSoftness = 0.0001f;

    public Material material;

#if AkaitoAi_MobilePP
    public void Init(MobilePostProcessing mobilePostProcessing)
    { 
        mobilePostProcessing.PreviewInSceneView = PreviewInSceneView;
        
        mobilePostProcessing.Blur = Blur;
        mobilePostProcessing.BlurAmount = BlurAmount;
        mobilePostProcessing.BlurMask = BlurMask;

        mobilePostProcessing.Bloom = Bloom;
        mobilePostProcessing.BloomColor = BloomColor;
        mobilePostProcessing.BloomAmount = BloomAmount;
        mobilePostProcessing.BloomDiffuse = BloomDiffuse;
        mobilePostProcessing.BloomThreshold = BloomThreshold;
        mobilePostProcessing.BloomSoftness = BloomSoftness;

        mobilePostProcessing.LUT = LUT;
        mobilePostProcessing.LutAmount = LutAmount;
        mobilePostProcessing.SourceLut = SourceLut;

        mobilePostProcessing.ImageFiltering = ImageFiltering;
        mobilePostProcessing.Color = Color;
        mobilePostProcessing.Contrast = Contrast;
        mobilePostProcessing.Brightness = Brightness;
        mobilePostProcessing.Saturation = Saturation;
        mobilePostProcessing.Exposure = Exposure;
        mobilePostProcessing.Gamma = Gamma;
        mobilePostProcessing.Sharpness = Sharpness;

        mobilePostProcessing.ChromaticAberration = ChromaticAberration;
        mobilePostProcessing.Offset = Offset;
        mobilePostProcessing.FishEyeDistortion = FishEyeDistortion;
        mobilePostProcessing.GlitchAmount = GlitchAmount;
        mobilePostProcessing.Distortion = Distortion;
        mobilePostProcessing.LensDistortion = LensDistortion;
        
        mobilePostProcessing.Vignette = Vignette;
        mobilePostProcessing.VignetteColor = VignetteColor;
        mobilePostProcessing.VignetteAmount = VignetteAmount;
        mobilePostProcessing.VignetteSoftness = VignetteSoftness;

        mobilePostProcessing.material = material;
    }
#endif
}
