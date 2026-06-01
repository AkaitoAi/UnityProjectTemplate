using AkaitoAi.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN
using DG.Tweening;
#endif

namespace AkaitoAi.GameBase
{
    #region Events
    //Menu
    public struct OnCoinsCharged : IEvent { }
    public struct OnModeSelected : IEvent { }
    public struct OnModeSkipLevelSelected : IEvent { }
    public struct OnSwitchPanel : IEvent { public ScreenType type; }

    //Gameplay
    public struct OnLevelWon : IEvent { }
    public struct OnLevelFailed : IEvent { public string reason; }
    public struct OnGameplayScreen : IEvent { public GameplayScreens screen; }
    public struct OnEnvironmentSpawned : IEvent { public GameObject currentEnvironment; }
    public struct OnLightingChanged : IEvent { public LightingSnapshot lightingSnapshot; }
    public struct OnPlayerSpawned : IEvent { public GameObject currentPlayer; }
    public struct OnPlayerCamera : IEvent { public GameObject currentCamera; }
    public struct OnLevelSpawned : IEvent { public GameObject levelsParentGO; public GameObject currentLevel; public int winReward; public int timeReward; }
    #endregion

    #region Enums
    //Menu
    public enum ScreenType
    {
        MainMenu,
        Settings,
        Selection,
        ModeSelection,
        LevelSelection,
        Loading,
        QuitGame,
        PlayerProfile,
        PlayerProfileAvatar,
        PlayerProfileCountry,
        Shop,
        DailyReward
    }

    public enum ButtonType
    {
        Play_Game,
        Settings,
        Settings_Close,
        Quit_Game,
        Quit_Game_No,
        Quit_Game_Yes,
        Back_From_Selection,
        Selection,
        Mode_Select,
        Back_From_Mode,
        Level_Select,
        Back_From_level,
        Skip_Level_Select,
        Player_Profile,
        Back_From_Player_Profile,
        Player_Profile_Avatar,
        Back_From_Player_Profile_Avatar,
        Player_Profile_Country,
        Back_From_Player_Profile_Country,
        Shop,
        Back_From_Shop,
        DailyReward,
        Back_From_DailyReward,
        Back_From_Profile
    }

    [System.Serializable]
    public enum SpecsUIType
    {
        None,
        Lerp,
        Tween
    }

    //Gameplay
    public enum GameplayScreens
    {
        None,
        Resume,
        Pause,
        Win,
        Fail,
        CutScene,
        Loading,
        Setting
    };
    #endregion

    #region Structs
    [Serializable]
    public struct BoolIndex
    {
        public bool use;
        public int childIndex;
    }

    [Serializable]
    public struct Selectable
    {
        public SelectableDetails[] details;
        public SpecsUI ui;
    }

    [Serializable]
    public struct SpecsUI
    {
        public SpecsUIType type;
        public Image[] specsFillers;
        public TMP_Text[] specsTexts;

        [Header("Settings")]
        public float lerpDuration;

        public void UpdateFiller(SelectSpecsSO specs)
        {
            if (specsFillers == null || specsFillers.Length == 0) return;

            for (int i = 0; i < specsFillers.Length; i++)
                specsFillers[i].fillAmount = (float)specs.specs[i] / 100f;
        }

        public void UpdateText(SelectSpecsSO specs)
        {
            if (specsTexts == null || specsTexts.Length == 0) return;

            for (int i = 0; i < specsTexts.Length; i++)
                specsTexts[i].text = specs.specs[i].ToString();
        }

        public IEnumerator FillerLerp(SelectSpecsSO specs)
        {
            if (specsFillers == null || specsFillers.Length == 0) yield break;

            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                for (int i = 0; i < specsFillers.Length; i++)
                {
                    specsFillers[i].fillAmount = Mathf.LerpUnclamped((float)specsFillers[i].fillAmount * 100f
                        , specs.specs[i], timeElapsed / lerpDuration) / 100f;
                }

                timeElapsed += Time.deltaTime;
                yield return null;
            }
            for (int i = 0; i < specsFillers.Length; i++)
                specsFillers[i].fillAmount = (float)specs.specs[i] / 100f;
        }

        public IEnumerator TextLerp(SelectSpecsSO specs)
        {
            if (specsTexts == null || specsTexts.Length == 0) yield break;

            float timeElapsed = 0;
            while (timeElapsed < lerpDuration)
            {
                for (int i = 0; i < specsTexts.Length; i++)
                {
                    specsTexts[i].text =
                        Mathf.FloorToInt(Mathf.LerpUnclamped(
                            0f
                            , specs.specs[i]
                            , timeElapsed / lerpDuration)).ToString();
                }

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < specsTexts.Length; i++)
                specsTexts[i].text = specs.specs[i].ToString();
        }

        public void FillerTween(SelectSpecsSO specs)
        {
            if (specsFillers == null || specsFillers.Length == 0) return;

            var localFillers = specsFillers;

            for (int i = 0; i < localFillers.Length; i++)
            {
                float currentValue = localFillers[i].fillAmount * 100f;
                float targetValue = specs.specs[i];
                int index = i; // avoid closure issue

#if DOTWEEN
                DOVirtual.Float(currentValue, targetValue, lerpDuration, value =>
                {
                    localFillers[index].fillAmount = value / 100f;
                });
#endif
            }
        }


        public void TextTween(SelectSpecsSO specs)
        {
            if (specsTexts == null || specsTexts.Length == 0) return;

            var localTexts = specsTexts;

            for (int i = 0; i < localTexts.Length; i++)
            {
                int targetValue = Mathf.FloorToInt(specs.specs[i]);
                int index = i;

#if DOTWEEN
                DOVirtual.Int(0, targetValue, lerpDuration, value =>
                {
                    localTexts[index].text = value.ToString();
                });
#endif
            }
        }

    }

    [Serializable]
    public struct SelectableDetails
    {
        public GameObject Obj;
        public int price;
        internal SelectSpecsSO specs;
    }

    [Serializable]
    public struct Vignette
    {
        public Image vignetteImage;
        public Sprite blueSplash, greenSplash, redSplash;

        public void DisplayVignette(Sprite _color, Action afterShown = null)
        {
            if (vignetteImage.gameObject.activeInHierarchy) return;

            //TODO Sounds Calling
            SoundManager.Instance?.PlayCoinPickupSound();

            vignetteImage.sprite = _color;
            vignetteImage.gameObject.SetActive(true);

            if (!vignetteImage.gameObject.activeInHierarchy) return;

            //Hide 

            afterShown?.Invoke();
        }
    }

    [Serializable]
    public struct ModeLevelSetup
    {
        public GameObject environmentObj;
        public GameObject levelsParentObj;
        public GameObject playersParentObj;
        public TimerController timeController;
        public int levelWinReward;
        public int levelTimeReward;
    }

    [Serializable]
    public struct ObjectiveScreen
    {
        public GameObject objectiveScreen;
        public TMP_Text objectiveText;
        public Button okButton;

        public void DisplayDialogue(string dialogue, Action onShow = null, Action onHide = null)
        {
            if (objectiveScreen.activeInHierarchy) OnObjectiveOKButton(onHide);

            objectiveText.text = dialogue;
            objectiveScreen.SetActive(true);

            // Hide Resume Screen and Activate the Objective Screen Canvas

            onShow?.Invoke();
        }

        public void OnObjectiveOKButton(Action onHide = null)
        {
            //TODO Sound Calling
            //SoundManager.Instance?.PlayOnButtonSound();

            //TODO Sound Calling
            //SoundManager.Instance.ChangeVolume(musicMuteSound, sfxMuteSound);

            if (!objectiveScreen.activeInHierarchy) return;

            objectiveScreen.SetActive(false);

            // Show Resume Screen and Deactivate the Objective Screen Canvas

            onHide?.Invoke();

            //FadeScreen(1.5f);
        }
    }

    [Serializable]
    public struct LevelTimer
    {
        public GameObject timerContainer;
        internal TimerController timeController;
        internal bool useTimer;
        internal bool timeRunning;

        public void TimeToggle(bool _timeState, float _timeScale) //! Control's game time scale and level time
        {
            if (useTimer) timeRunning = _timeState;

            Time.timeScale = _timeScale;
        }
    }

    [Serializable]
    public struct CutSceneSetup
    {
        public GameObject cutsceneObj;
        public float duration;
    }

    #endregion

    #region Classes
    public class MaterialFader
    {
        private readonly Material transparentTemplate;
        private readonly float fadeDuration;
        private readonly float transparentAlpha;

        private readonly Dictionary<Renderer, Material[]> originalMaterials = new();
        private readonly Dictionary<Material, Material> transparentMaterialPool = new();
        private readonly HashSet<Renderer> affectedRenderers = new();

        public MaterialFader(Material transparentMaterial, float fadeDuration = 0.25f, float transparentAlpha = 0.3f)
        {
            this.transparentTemplate = transparentMaterial;
            this.fadeDuration = fadeDuration;
            this.transparentAlpha = transparentAlpha;
        }

        public void FadeToTransparent(Renderer[] renderers)
        {
            foreach (var rend in renderers)
            {
                if (rend == null || originalMaterials.ContainsKey(rend)) continue;

                Material[] originalMats = rend.materials;
                originalMaterials[rend] = originalMats;
                Material[] transparentMats = new Material[originalMats.Length];

                for (int i = 0; i < originalMats.Length; i++)
                {
                    Material original = originalMats[i];
                    if (original == null) continue;

                    if (!transparentMaterialPool.TryGetValue(original, out Material pooledTransparent))
                    {
                        pooledTransparent = new Material(transparentTemplate);
                        if (original.HasProperty("_MainTex"))
                            pooledTransparent.mainTexture = original.mainTexture;

                        transparentMaterialPool[original] = pooledTransparent;
                    }

                    transparentMats[i] = pooledTransparent;

                    if (pooledTransparent.HasProperty("_Color"))
                    {
                        Color c = pooledTransparent.color;
                        c.a = 1f;
                        pooledTransparent.color = c;
#if DOTWEEN
                        pooledTransparent.DOFade(transparentAlpha, fadeDuration);
#endif
                    }
                }

                rend.materials = transparentMats;
                affectedRenderers.Add(rend);
            }
        }

        public void RestoreOriginal(Renderer[] renderers)
        {
            foreach (var rend in renderers)
            {
                if (rend == null || !originalMaterials.TryGetValue(rend, out var originalMats)) continue;

                foreach (var mat in rend.materials)
                {
#if DOTWEEN
                    if (mat != null && mat.HasProperty("_Color"))
                        mat.DOFade(1f, fadeDuration);
#endif
                }

                rend.materials = originalMats;
                originalMaterials.Remove(rend);
                affectedRenderers.Remove(rend);
            }
        }

        public void ForceRestoreAll()
        {
            foreach (var rend in affectedRenderers)
            {
                if (rend == null || !originalMaterials.TryGetValue(rend, out var originalMats)) continue;
                rend.materials = originalMats;
            }

            affectedRenderers.Clear();
            originalMaterials.Clear();
        }
    }

    public class MaterialColorFader
    {
        private readonly List<Material> materials = new();
        private readonly List<Color> originalColors = new();

#if DOTWEEN
        private Tween currentTween;
#endif

        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public MaterialColorFader(Renderer renderer)
        {
            Material[] clonedMaterials = renderer.materials;

            foreach (var mat in clonedMaterials)
            {
                materials.Add(mat);
                originalColors.Add(mat.HasProperty(ColorID) ? mat.GetColor(ColorID) : Color.white);
            }
        }

        public void Blink(Color targetColor, float duration, int loopCount)
        {
            Kill();

#if DOTWEEN
            currentTween = DOTween.To(() => 0f, t => ApplyColorLerp(t, targetColor), 1f, duration)
                .SetLoops(loopCount, LoopType.Yoyo)
                .OnComplete(ResetColors);
#endif
        }

        public void Kill()
        {
#if DOTWEEN
            if (currentTween != null && currentTween.IsActive())
                currentTween.Kill();
#endif
        }

        private void ApplyColorLerp(float t, Color targetColor)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (!materials[i].HasProperty(ColorID)) continue;

                Color lerped = Color.Lerp(originalColors[i], targetColor, t);
                materials[i].SetColor(ColorID, lerped);
            }
        }

        public void ResetColors()
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (!materials[i].HasProperty(ColorID)) continue;

                materials[i].SetColor(ColorID, originalColors[i]);
            }
        }
    }

    public class MaterialColorFaderInChildren
    {
        private readonly List<Material> materials = new();
        private readonly List<Color> originalColors = new();

#if DOTWEEN
        private Tween currentTween;
#endif

        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public MaterialColorFaderInChildren(IEnumerable<Renderer> renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                Material[] clonedMaterials = renderer.materials;

                foreach (var mat in clonedMaterials)
                {
                    materials.Add(mat);
                    originalColors.Add(mat.HasProperty(ColorID) ? mat.GetColor(ColorID) : Color.white);
                }
            }
        }

        public void Blink(Color targetColor, float duration, int loopCount)
        {
            Kill();

#if DOTWEEN
            currentTween = DOTween.To(() => 0f, t => ApplyColorLerp(t, targetColor), 1f, duration)
                .SetLoops(loopCount, LoopType.Yoyo)
                .OnComplete(ResetColors);
#endif
        }

        public void Kill()
        {
#if DOTWEEN
            if (currentTween != null && currentTween.IsActive())
                currentTween.Kill();
#endif
        }

        private void ApplyColorLerp(float t, Color targetColor)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (!materials[i].HasProperty(ColorID)) continue;

                Color lerped = Color.Lerp(originalColors[i], targetColor, t);
                materials[i].SetColor(ColorID, lerped);
            }
        }

        public void ResetColors()
        {
            for (int i = 0; i < materials.Count; i++)
            {
                if (!materials[i].HasProperty(ColorID)) continue;

                materials[i].SetColor(ColorID, originalColors[i]);
            }
        }
    }

    #endregion
}